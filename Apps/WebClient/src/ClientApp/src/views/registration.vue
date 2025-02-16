<script lang="ts">
import { library } from "@fortawesome/fontawesome-svg-core";
import { faExclamationTriangle } from "@fortawesome/free-solid-svg-icons";
import Vue from "vue";
import { Component, Prop, Ref } from "vue-property-decorator";
import { email, helpers, requiredIf, sameAs } from "vuelidate/lib/validators";
import { Validation } from "vuelidate/vuelidate";
import { Action, Getter } from "vuex-class";

import HtmlTextAreaComponent from "@/components/htmlTextarea.vue";
import LoadingComponent from "@/components/loading.vue";
import { RegistrationStatus } from "@/constants/registrationStatus";
import BannerError from "@/models/bannerError";
import type { WebClientConfiguration } from "@/models/configData";
import type { OidcUserProfile } from "@/models/user";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.container";
import {
    IAuthenticationService,
    ILogger,
    IUserProfileService,
} from "@/services/interfaces";
import ErrorTranslator from "@/utility/errorTranslator";

library.add(faExclamationTriangle);

@Component({
    components: {
        LoadingComponent,
        HtmlTextAreaComponent,
    },
})
export default class RegistrationView extends Vue {
    @Prop() inviteKey?: string;
    @Prop() inviteEmail?: string;

    @Action("checkRegistration", { namespace: "user" })
    checkRegistration!: () => Promise<boolean>;

    @Ref("registrationForm") form!: HTMLFormElement;

    @Getter("webClient", { namespace: "config" })
    webClientConfig!: WebClientConfiguration;

    @Action("addError", { namespace: "errorBanner" })
    addError!: (error: BannerError) => void;

    private accepted = false;
    private email = "";
    private emailConfirmation = "";
    private smsNumber = "";

    private isEmailChecked = true;
    private isSMSNumberChecked = true;

    private oidcUser!: OidcUserProfile;
    private userProfileService!: IUserProfileService;
    private submitStatus = "";
    private loadingUserData = true;
    private loadingTermsOfService = true;
    private clientRegistryError = false;
    private errorMessage = "";

    private logger!: ILogger;
    private isValidAge = false;
    private minimumAge!: number;

    private termsOfService = "";

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        this.minimumAge = this.webClientConfig.minPatientAge;

        if (
            this.webClientConfig.registrationStatus == RegistrationStatus.Open
        ) {
            this.email = "";
            this.emailConfirmation = "";
        } else {
            this.email = this.inviteEmail || "";
            this.emailConfirmation = this.email;
        }

        this.userProfileService = container.get(
            SERVICE_IDENTIFIER.UserProfileService
        );

        // Load the user name
        this.loadingUserData = true;
        var authenticationService: IAuthenticationService = container.get(
            SERVICE_IDENTIFIER.AuthenticationService
        );
        authenticationService
            .getOidcUserProfile()
            .then((oidcUser) => {
                if (oidcUser) {
                    this.oidcUser = oidcUser;

                    if (this.oidcUser.email !== null) {
                        this.email = this.oidcUser.email;
                        this.emailConfirmation = this.oidcUser.email;
                    }

                    return this.userProfileService
                        .validateAge(oidcUser.hdid)
                        .then((isValid) => {
                            this.isValidAge = isValid;
                            this.loadingUserData = false;
                        })
                        .catch((err) => {
                            this.loadingUserData = false;
                            this.clientRegistryError = true;
                            this.addError(
                                ErrorTranslator.toBannerError(
                                    "Validating user",
                                    err
                                )
                            );
                        });
                } else {
                    this.loadingUserData = false;
                }
            })
            .catch((err) => {
                this.loadingUserData = false;
                this.addError(
                    ErrorTranslator.toBannerError(
                        "Retrieving User profile",
                        err
                    )
                );
            });

        this.loadTermsOfService();
    }

    private validations() {
        const sms = helpers.regex("sms", /^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$/);
        return {
            smsNumber: {
                required: requiredIf(() => {
                    return this.isSMSNumberChecked;
                }),
                sms,
            },
            email: {
                required: requiredIf(() => {
                    return this.isEmailChecked;
                }),
                email,
            },
            emailConfirmation: {
                required: requiredIf(() => {
                    return this.isEmailChecked;
                }),
                sameAsEmail: sameAs("email"),
                email,
            },
            accepted: { isChecked: sameAs(() => true) },
        };
    }

    private get isLoading(): boolean {
        return this.loadingTermsOfService || this.loadingUserData;
    }

    private get fullName(): string {
        return this.oidcUser.given_name + " " + this.oidcUser.family_name;
    }
    private get isRegistrationClosed(): boolean {
        return (
            this.webClientConfig.registrationStatus == RegistrationStatus.Closed
        );
    }
    private get isPredefinedEmail() {
        if (
            this.webClientConfig.registrationStatus != RegistrationStatus.Open
        ) {
            return !!this.inviteEmail;
        }
        return false;
    }

    private loadTermsOfService(): void {
        this.loadingTermsOfService = true;
        this.userProfileService
            .getTermsOfService()
            .then((result) => {
                this.logger.debug(
                    `getTermsOfService result: ${JSON.stringify(result)}`
                );
                this.termsOfService = result.content;
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError(
                        "Loading Terms of service",
                        err
                    )
                );
            })
            .finally(() => {
                this.loadingTermsOfService = false;
            });
    }

    private isValid(param: Validation): boolean | undefined {
        return param.$dirty ? !param.$invalid : undefined;
    }

    private onSubmit(event: Event) {
        this.$v.$touch();
        if (this.$v.$invalid) {
            this.submitStatus = "ERROR";
            event.preventDefault();
            return;
        }

        this.submitStatus = "PENDING";
        if (this.smsNumber) {
            this.smsNumber = this.smsNumber.replace(/\D+/g, "");
        }
        this.loadingTermsOfService = true;
        this.userProfileService
            .createProfile({
                profile: {
                    hdid: this.oidcUser.hdid,
                    acceptedTermsOfService: this.accepted,
                    email: this.email || "",
                    isEmailVerified: false,
                    smsNumber: this.smsNumber || "",
                    isSMSNumberVerified: false,
                    preferences: {},
                },
                inviteCode: this.inviteKey || "",
            })
            .then((result) => {
                this.logger.debug(
                    `Create Profile result: ${JSON.stringify(result)}`
                );
                this.redirect();
            })
            .catch((err) => {
                this.addError(
                    ErrorTranslator.toBannerError("User profile creation", err)
                );
            })
            .finally(() => {
                this.loadingTermsOfService = false;
            });

        event.preventDefault();
    }

    private redirect(): void {
        this.checkRegistration().then((isRegistered: boolean) => {
            if (!isRegistered) {
                this.addError({
                    title: "User profile creation",
                    description: "Profile already created",
                    detail: "",
                    errorCode: "",
                });
                return;
            }
            this.$router.push({
                path:
                    this.smsNumber === "" && this.email === ""
                        ? "/timeline"
                        : "/profile",
                query: {
                    toVerifyPhone: this.smsNumber === "" ? "false" : "true",
                    toVerifyEmail: this.email === "" ? "false" : "true",
                },
            });
        });
    }
    private onEmailOptout(isChecked: boolean): void {
        if (!isChecked) {
            this.emailConfirmation = "";
            this.email = "";
        }
    }

    private onSMSOptout(isChecked: boolean): void {
        if (!isChecked) {
            this.smsNumber = "";
        }
    }
}
</script>

<template>
    <b-container class="pt-2">
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <div v-if="!isLoading && termsOfService !== ''">
            <b-row v-if="isRegistrationClosed">
                <b-col>
                    <div id="pageTitle">
                        <h1 id="Subject">Closed Registration</h1>
                        <div id="Description">
                            Thank you for your interest in the Health Gateway
                            service. At this time, the registration is closed.
                        </div>
                    </div>
                </b-col>
            </b-row>
            <div v-else>
                <b-form
                    v-if="isValidAge"
                    ref="registrationForm"
                    @submit.prevent="onSubmit"
                >
                    <b-row>
                        <b-col>
                            <div id="pageTitle">
                                <h2 id="Subject">Registration</h2>
                            </div>
                        </b-col>
                    </b-row>
                    <b-row class="mb-2">
                        <b-col>
                            <h4 class="subheading">
                                Communication Preferences (Optional)
                            </h4>
                        </b-col>
                    </b-row>
                    <b-row class="mb-3">
                        <b-col>
                            <b-row class="d-flex">
                                <b-col class="d-flex pr-0">
                                    <b-form-checkbox
                                        id="emailCheckbox"
                                        v-model="isEmailChecked"
                                        data-testid="emailCheckbox"
                                        @change="onEmailOptout($event)"
                                    >
                                        Email Notifications
                                    </b-form-checkbox>
                                </b-col>
                            </b-row>
                            <b-row class="d-flex">
                                <b-col class="d-flex pr-0">
                                    <i class="small">
                                        Receive application and health record
                                        updates
                                    </i>
                                </b-col>
                            </b-row>
                            <b-form-input
                                id="emailInput"
                                v-model="$v.email.$model"
                                data-testid="emailInput"
                                type="email"
                                placeholder="Your email address"
                                :disabled="isPredefinedEmail || !isEmailChecked"
                                :state="isValid($v.email)"
                            />
                            <b-form-invalid-feedback :state="isValid($v.email)">
                                Valid email is required
                            </b-form-invalid-feedback>
                        </b-col>
                    </b-row>
                    <b-row v-if="!isPredefinedEmail" class="mb-3">
                        <b-col>
                            <b-form-input
                                id="emailConfirmationInput"
                                v-model="$v.emailConfirmation.$model"
                                data-testid="emailConfirmationInput"
                                type="email"
                                placeholder="Confirm your email address"
                                :disabled="!isEmailChecked"
                                :state="isValid($v.emailConfirmation)"
                            />
                            <b-form-invalid-feedback
                                :state="$v.emailConfirmation.sameAsEmail"
                            >
                                Emails must match
                            </b-form-invalid-feedback>
                        </b-col>
                    </b-row>
                    <!-- SMS section -->
                    <b-row class="mb-3">
                        <b-col>
                            <b-row class="d-flex">
                                <b-col class="d-flex pr-0">
                                    <b-form-checkbox
                                        id="smsCheckbox"
                                        v-model="isSMSNumberChecked"
                                        @change="onSMSOptout($event)"
                                    >
                                        Text Notifications
                                    </b-form-checkbox>
                                </b-col>
                            </b-row>
                            <b-row class="d-flex">
                                <b-col class="d-flex pr-0">
                                    <i class="small">
                                        Receive health record updates only
                                    </i>
                                </b-col>
                            </b-row>
                            <b-form-input
                                id="smsNumberInput"
                                v-model="$v.smsNumber.$model"
                                v-mask="'(###) ###-####'"
                                type="tel"
                                data-testid="smsNumberInput"
                                class="d-flex"
                                placeholder="Your phone number"
                                :state="isValid($v.smsNumber)"
                                :disabled="!isSMSNumberChecked"
                            >
                            </b-form-input>
                            <b-form-invalid-feedback
                                :state="isValid($v.smsNumber)"
                            >
                                Valid sms number is required
                            </b-form-invalid-feedback>
                        </b-col>
                    </b-row>
                    <b-row v-if="!isEmailChecked && !isSMSNumberChecked">
                        <b-col class="font-weight-bold text-primary">
                            <hg-icon
                                icon="exclamation-triangle"
                                size="medium"
                                aria-hidden="true"
                                class="mr-2"
                            />
                            <span
                                >You won't receive notifications from the Health
                                Gateway. You can update this from your Profile
                                Page later.</span
                            >
                        </b-col>
                    </b-row>
                    <b-row class="mt-4">
                        <b-col>
                            <h4 class="subheading">Terms of Service</h4>
                        </b-col>
                    </b-row>
                    <b-row class="mb-3">
                        <b-col>
                            <HtmlTextAreaComponent
                                class="termsOfService"
                                :input="termsOfService"
                            />
                        </b-col>
                    </b-row>
                    <b-row class="mb-3">
                        <b-col>
                            <b-form-checkbox
                                id="accept"
                                v-model="accepted"
                                data-testid="acceptCheckbox"
                                class="accept"
                                :state="isValid($v.accepted)"
                            >
                                I agree to the terms of service above
                            </b-form-checkbox>
                            <b-form-invalid-feedback
                                :state="isValid($v.accepted)"
                            >
                                You must accept the terms of service.
                            </b-form-invalid-feedback>
                        </b-col>
                    </b-row>
                    <b-row class="mb-5">
                        <b-col class="justify-content-right">
                            <hg-button
                                class="px-5 float-right"
                                type="submit"
                                data-testid="registerButton"
                                variant="primary"
                                :disabled="!accepted"
                                >Register</hg-button
                            >
                        </b-col>
                    </b-row>
                </b-form>
                <div v-else-if="!clientRegistryError">
                    <h1>Minimum age required for registration</h1>
                    <p data-testid="minimumAgeErrorText">
                        You must be <strong>{{ minimumAge }}</strong> years of
                        age or older to use this application
                    </p>
                </div>
                <div v-else>
                    <h1>Error retrieving user information</h1>
                    <p data-testid="clientRegistryErrorText">
                        There may be an issue in our Client Registry. Please
                        contact <strong>HealthGateway@gov.bc.ca</strong>
                    </p>
                </div>
            </div>
        </div>
    </b-container>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

input {
    width: 320px !important;
    max-width: 320px !important;
}

.accept label {
    color: $primary;
}

.subheading {
    color: $soft_text;
}

.termsOfService {
    max-height: 330px;
    overflow-y: scroll;
    box-shadow: 0 0 2px #00000070;
    border: none;
}
</style>
