<script lang="ts">
import { load } from "recaptcha-v3";
import Vue from "vue";
import { Component, Watch } from "vue-property-decorator";
import { required } from "vuelidate/lib/validators";
import { Validation } from "vuelidate/vuelidate";
import { Action, Getter } from "vuex-class";

import DatePickerComponent from "@/components/datePicker.vue";
import ErrorCardComponent from "@/components/errorCard.vue";
import LoadingComponent from "@/components/loading.vue";
import VaccinationStatusResultComponent from "@/components/vaccinationStatusResult.vue";
import BannerError from "@/models/bannerError";
import type { WebClientConfiguration } from "@/models/configData";
import { DateWrapper, StringISODate } from "@/models/dateWrapper";
import VaccinationStatus from "@/models/vaccinationStatus";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.container";
import { ILogger } from "@/services/interfaces";
import PHNValidator from "@/utility/phnValidator";

const validPersonalHealthNumber = (value: string): boolean => {
    var phn = value.replace(/\D/g, "");
    return PHNValidator.IsValid(phn);
};

@Component({
    components: {
        "vaccination-status-result": VaccinationStatusResultComponent,
        "date-picker": DatePickerComponent,
        "error-card": ErrorCardComponent,
        LoadingComponent,
    },
})
export default class VaccinationStatusView extends Vue {
    @Action("retrieve", { namespace: "vaccinationStatus" })
    retrieveVaccinationStatus!: (params: {
        phn: string;
        dateOfBirth: StringISODate;
        token: string;
    }) => Promise<void>;

    @Getter("webClient", { namespace: "config" })
    webClientConfig!: WebClientConfiguration;

    @Getter("vaccinationStatus", { namespace: "vaccinationStatus" })
    status!: VaccinationStatus | undefined;

    @Getter("isLoading", { namespace: "vaccinationStatus" })
    isLoading!: boolean;

    @Getter("error", { namespace: "vaccinationStatus" })
    error!: BannerError | undefined;

    @Getter("statusMessage", { namespace: "vaccinationStatus" })
    statusMessage!: string;

    private logger!: ILogger;
    private displayResult = false;
    private errorDisplaySeconds = 5;

    private phn = "";
    private dateOfBirth = "";

    private validations() {
        return {
            phn: {
                required: required,
                formatted: validPersonalHealthNumber,
            },
            dateOfBirth: {
                required: required,
                maxValue: (value: string) =>
                    new DateWrapper(value).isBefore(new DateWrapper()),
            },
        };
    }

    @Watch("status")
    private onStatusChange() {
        if (this.status?.loaded) {
            this.displayResult = true;
        }
    }

    private isValid(param: Validation): boolean | undefined {
        return param.$dirty ? !param.$invalid : undefined;
    }

    private handleSubmit() {
        this.$v.$touch();
        if (!this.$v.$invalid) {
            load(this.webClientConfig.captchaSiteKey)
                .then((recaptcha) => {
                    recaptcha.showBadge();
                    recaptcha
                        .execute("retrieveVaccinationStatus")
                        .then((token) => {
                            this.retrieveVaccinationStatus({
                                phn: this.phn,
                                dateOfBirth: this.dateOfBirth,
                                token,
                            })
                                .then(() => {
                                    this.logger.debug(
                                        "Vaccination status retrieved"
                                    );
                                })
                                .catch((err) => {
                                    this.logger.error(
                                        `Error retrieving vaccination status: ${err}`
                                    );
                                });
                        })
                        .catch((err) => {
                            this.logger.error(
                                `Error executing captcha action: ${err}`
                            );
                        });
                })
                .catch((err) => {
                    this.logger.error(`Error loading captcha: ${err}`);
                });
        }
    }

    private created() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
    }
}
</script>

<template>
    <div class="fill-height d-flex flex-column">
        <LoadingComponent :is-loading="isLoading" :text="statusMessage" />
        <div class="header">
            <img
                class="img-fluid m-3"
                src="@/assets/images/gov/bcid-logo-rev-en.svg"
                width="181"
                alt="BC Mark"
            />
        </div>
        <div v-if="error !== undefined" class="container">
            <b-alert
                variant="danger"
                class="no-print my-3"
                :show="error !== undefined"
                dismissible
            >
                <h4>{{ error.title }}</h4>
                <h6>{{ error.errorCode }}</h6>
                <div class="pl-4">
                    <p data-testid="errorTextDescription">
                        {{ error.description }}
                    </p>
                    <p data-testid="errorTextDetails">
                        {{ error.detail }}
                    </p>
                    <p v-if="error.traceId" data-testid="errorSupportDetails">
                        If this issue persists, contact HealthGateway@gov.bc.ca
                        and provide
                        <span class="trace-id">{{ error.traceId }}</span>
                    </p>
                </div>
            </b-alert>
        </div>
        <vaccination-status-result v-if="displayResult" />
        <div v-else>
            <div class="p-3 bg-success text-white" no-gutters>
                <h3 class="text-center m-0">COVID‑19 Vaccination Check</h3>
            </div>
            <form class="container my-3" @submit.prevent="handleSubmit">
                <p>Please provide the following.</p>
                <b-row>
                    <b-col cols="12" sm="auto">
                        <b-form-group
                            label="Personal Health Number"
                            label-for="phn"
                        >
                            <b-form-input
                                id="phn"
                                v-model="phn"
                                data-testid="phnInput"
                                :state="isValid($v.phn)"
                                @blur="$v.phn.$touch()"
                            />
                            <b-form-invalid-feedback v-if="!$v.phn.required">
                                Personal Health Number is required.
                            </b-form-invalid-feedback>
                            <b-form-invalid-feedback
                                v-else-if="!$v.phn.formatted"
                            >
                                Personal Health Number must be valid.
                            </b-form-invalid-feedback>
                        </b-form-group>
                    </b-col>
                </b-row>
                <b-row>
                    <b-col cols="12" sm="auto">
                        <b-form-group
                            label="Date of Birth"
                            label-for="dateOfBirth"
                            :state="isValid($v.dateOfBirth)"
                        >
                            <date-picker
                                id="dateOfBirth"
                                v-model="dateOfBirth"
                                data-testid="dateOfBirthInput"
                                :state="isValid($v.dateOfBirth)"
                                @blur="$v.dateOfBirth.$touch()"
                            />
                            <b-form-invalid-feedback
                                v-if="
                                    $v.dateOfBirth.$dirty &&
                                    !$v.dateOfBirth.required
                                "
                                force-show
                            >
                                A valid date of birth is required.
                            </b-form-invalid-feedback>
                            <b-form-invalid-feedback
                                v-else-if="
                                    $v.dateOfBirth.$dirty &&
                                    !$v.dateOfBirth.maxValue
                                "
                                force-show
                            >
                                Date of birth must be before today.
                            </b-form-invalid-feedback>
                        </b-form-group>
                    </b-col>
                </b-row>
                <hr />
                <div class="text-center my-3">
                    <hg-button variant="secondary" class="mr-2" to="/">
                        Cancel
                    </hg-button>
                    <hg-button
                        variant="primary"
                        type="submit"
                        :disabled="isLoading"
                    >
                        Check
                    </hg-button>
                </div>
                <p>
                    Your information is being collected to provide you with your
                    COVID-19 vaccination status under s. 26(c) of the
                    <em>Freedom of Information and Protection of Privacy Act</em
                    >. Contact the Ministry Privacy Officer at
                    <a href="mailto:MOH.Privacy.Officer@gov.bc.ca"
                        >MOH.Privacy.Officer@gov.bc.ca</a
                    >
                    or 778-698-5849 if you have any questions about this
                    collection.
                </p>
            </form>
        </div>
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

.header {
    background-color: $hg-brand-primary;
}

.trace-id {
    overflow-wrap: anywhere;
}
</style>

<style lang="scss">
@import "@/assets/scss/_variables.scss";

.vld-overlay {
    .vld-background {
        opacity: 0.75;
    }

    .vld-icon {
        text-align: center;
    }
}
</style>
