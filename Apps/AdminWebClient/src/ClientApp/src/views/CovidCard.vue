<script lang="ts">
import { saveAs } from "file-saver";
import { Component, Vue, Watch } from "vue-property-decorator";

import BannerFeedbackComponent from "@/components/core/BannerFeedback.vue";
import LoadingComponent from "@/components/core/Loading.vue";
import { Countries, InternationalDestinations } from "@/constants/countries";
import { Provinces } from "@/constants/provinces";
import { ResultType } from "@/constants/resulttype";
import { SnackbarPosition } from "@/constants/snackbarPosition";
import { States } from "@/constants/states";
import Address from "@/models/address";
import BannerFeedback from "@/models/bannerFeedback";
import CovidCardPatientResult from "@/models/covidCardPatientResult";
import { DateWrapper, StringISODate } from "@/models/dateWrapper";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { ICovidSupportService } from "@/services/interfaces";
import { Mask, phnMask, postalCodeMask, zipCodeMask } from "@/utility/masks";
import PHNValidator from "@/utility/phnValidator";

interface ImmunizationRow {
    date: string;
    product: string;
    lotNumber: string;
    clinic: string;
}

const emptyAddress: Address = {
    streetLines: [],
    city: "",
    state: "",
    postalCode: "",
    country: "",
};

@Component({
    components: {
        LoadingComponent,
        BannerFeedbackComponent,
    },
})
export default class CovidCardView extends Vue {
    private isEditMode = false;
    private isLoading = false;
    private showFeedback = false;

    private phn = "";
    private address: Address = { ...emptyAddress };
    private immunizations: ImmunizationRow[] = [];
    private searchResult: CovidCardPatientResult | null = null;
    private covidSupportService!: ICovidSupportService;
    private selectedDestination = "";

    private bannerFeedback: BannerFeedback = {
        type: ResultType.NONE,
        title: "",
        message: "",
    };

    private tableHeaders = [
        {
            text: "Date",
            value: "date",
            width: "10em",
        },
        {
            text: "Product",
            value: "product",
        },
        {
            text: "Lot Number",
            value: "lotNumber",
        },
        {
            text: "Clinic",
            value: "clinic",
        },
    ];

    private get internationalDestinations() {
        // sort destinations alphabetically except place Canada and US at the top
        const destinations = Object.keys(InternationalDestinations)
            .filter(
                (destination) =>
                    destination !== Countries.CA[0] &&
                    destination !== Countries.US[0]
            )
            .sort();
        destinations.unshift(Countries.CA[0], Countries.US[0]);

        return destinations.map((destination) => ({
            text: destination,
            value: destination,
        }));
    }

    @Watch("selectedDestination")
    private onSelectedDestinationChanged() {
        const countryCode = InternationalDestinations[this.selectedDestination];
        const countryName = Countries[countryCode][0] ?? "";
        this.address.country = this.isCanadaSelected
            ? ""
            : countryName.toLocaleUpperCase();
    }

    private get patientName() {
        return `${this.searchResult?.patient?.firstname} ${this.searchResult?.patient?.lastname}`;
    }

    private get provinceStateList() {
        if (this.isCanadaSelected) {
            return Object.keys(Provinces).map((provinceCode) => {
                return {
                    text: Provinces[provinceCode],
                    value: provinceCode,
                };
            });
        } else if (this.isUnitedStatesSelected) {
            return Object.keys(States).map((stateCode) => {
                return {
                    text: States[stateCode],
                    value: stateCode,
                };
            });
        } else {
            return [];
        }
    }

    private get isCanadaSelected() {
        return this.selectedDestination === Countries.CA[0];
    }

    private get isUnitedStatesSelected() {
        return this.selectedDestination === Countries.US[0];
    }

    private get snackbarPosition(): string {
        return SnackbarPosition.Bottom;
    }

    private get streetLines(): string {
        return this.address.streetLines.join("\n");
    }

    private set streetLines(streetLines: string) {
        this.address.streetLines = streetLines.split("\n");
    }

    private get birthDate(): string {
        if (this.searchResult === null) {
            return "";
        }
        return this.formatDate(this.searchResult.patient.birthdate);
    }

    private get phnMask(): Mask {
        return phnMask;
    }

    private get postalCodeMask(): Mask | undefined {
        if (this.isCanadaSelected) {
            return postalCodeMask;
        } else if (this.isUnitedStatesSelected) {
            return zipCodeMask;
        }
        return undefined;
    }

    private mounted() {
        this.covidSupportService = container.get(
            SERVICE_IDENTIFIER.CovidSupportService
        );
    }

    private clear(emptySearchField = true) {
        this.searchResult = null;
        this.isEditMode = false;
        this.immunizations = [];
        this.address = { ...emptyAddress };

        if (emptySearchField) {
            this.phn = "";
        }
    }

    private handleSearch() {
        this.clear(false);

        const phnDigits = this.phn.replace(/[^0-9]/g, "");
        if (!PHNValidator.IsValid(phnDigits)) {
            this.showFeedback = true;
            this.bannerFeedback = {
                type: ResultType.Error,
                title: "Validation error",
                message: "Invalid PHN",
            };
            return;
        }

        this.isLoading = true;

        this.covidSupportService
            .getPatient(phnDigits)
            .then((result) => {
                this.phn = "";
                this.searchResult = result;
                this.setAddress(
                    this.searchResult?.patient?.postalAddress,
                    this.searchResult?.patient?.physicalAddress
                );
                this.immunizations =
                    this.searchResult.immunizations?.map((immz) => {
                        return {
                            date: immz.dateOfImmunization,
                            clinic: immz.providerOrClinic,
                            product:
                                immz.immunization.immunizationAgents[0]
                                    .productName,
                            lotNumber:
                                immz.immunization.immunizationAgents[0]
                                    .lotNumber,
                        };
                    }) ?? [];
            })
            .catch(() => {
                this.searchResult = null;
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Search Error",
                    message: "Unknown error searching patient data",
                };
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private setAddress(
        defaultAddress: Address | null,
        backupAddress: Address | null
    ) {
        if (defaultAddress) {
            this.address = { ...defaultAddress };
        } else if (backupAddress) {
            this.address = { ...backupAddress };
        } else {
            this.address = { ...emptyAddress };
        }

        // convert country code to country name
        this.selectedDestination = Countries[this.address.country]
            ? Countries[this.address.country][0]
            : "";

        // select Canada if the address has a province but no country
        if (this.address.country === "" && Provinces[this.address.state]) {
            this.selectedDestination = Countries.CA[0];
        }
    }

    private handleSubmit() {
        this.isLoading = true;
        this.covidSupportService
            .mailDocument({
                personalHealthNumber:
                    this.searchResult?.patient.personalhealthnumber ?? "",
                mailAddress: this.address,
            })
            .then((mailResult) => {
                if (mailResult) {
                    this.searchResult = null;
                    this.showFeedback = true;
                    this.bannerFeedback = {
                        type: ResultType.Success,
                        title: "Success",
                        message:
                            "COVID-19 Immunization Card mailed successfully.",
                    };
                } else {
                    this.showFeedback = true;
                    this.bannerFeedback = {
                        type: ResultType.Error,
                        title: "Error",
                        message:
                            "Something went wrong when mailing the card, please try again later.",
                    };
                }
            })
            .catch(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Mail Error",
                    message: "Unknown error mailing report",
                };
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private handlePrint() {
        this.isLoading = true;
        this.covidSupportService
            .retrieveDocument(
                this.searchResult?.patient.personalhealthnumber ?? ""
            )
            .then((documentResult) => {
                if (documentResult) {
                    const downloadLink = `data:application/pdf;base64,${documentResult.data}`;
                    fetch(downloadLink).then((res) => {
                        res.blob().then((blob) => {
                            saveAs(blob, documentResult.fileName);
                        });
                    });
                }
            })
            .catch(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Download Error",
                    message: "Unknown error downloading report",
                };
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private onEditModeChange() {
        if (!this.isEditMode) {
            this.setAddress(
                this.searchResult?.patient?.postalAddress ?? null,
                this.searchResult?.patient?.physicalAddress ?? null
            );
        }
    }

    private formatDate(date: StringISODate): string {
        if (!date) {
            return "";
        }
        return new DateWrapper(date).format(DateWrapper.defaultFormat);
    }
}
</script>

<template>
    <v-container>
        <LoadingComponent :is-loading="isLoading" />
        <BannerFeedbackComponent
            :show-feedback.sync="showFeedback"
            :feedback="bannerFeedback"
            :position="snackbarPosition"
        />
        <v-row no-gutters>
            <v-col cols="12" sm="12" md="10" offset-md="1">
                <form @submit.prevent="handleSearch()">
                    <v-row align="center" dense>
                        <v-col>
                            <v-text-field
                                v-model="phn"
                                v-mask="phnMask"
                                label="PHN"
                            />
                        </v-col>
                        <v-col cols="auto">
                            <v-btn type="submit" class="mt-2 primary">
                                <span>Search</span>
                                <v-icon class="ml-2" size="sm">
                                    fas fa-search
                                </v-icon>
                            </v-btn>
                        </v-col>
                        <v-col cols="auto">
                            <v-btn
                                type="reset"
                                class="mt-2 secondary"
                                @click="clear"
                            >
                                <span>Clear</span>
                                <v-icon class="ml-2" size="sm">
                                    mdi-backspace
                                </v-icon>
                            </v-btn>
                        </v-col>
                    </v-row>
                </form>
                <v-row
                    v-if="searchResult != null && searchResult.patient == null"
                    align="center"
                >
                    <v-col>
                        <h2>No records found.</h2>
                    </v-col>
                </v-row>
                <v-form
                    v-if="searchResult != null && searchResult.patient != null"
                    autocomplete="false"
                    @submit.prevent="handleSubmit()"
                >
                    <v-row>
                        <v-col>
                            <h2>Patient Information</h2>
                        </v-col>
                    </v-row>
                    <v-row align="center" dense>
                        <v-col>
                            <v-text-field
                                v-model="patientName"
                                disabled
                                label="Name"
                            />
                        </v-col>
                        <v-col>
                            <v-text-field
                                v-model="birthDate"
                                disabled
                                label="Birthdate"
                            />
                        </v-col>
                        <v-col>
                            <v-text-field
                                v-model="
                                    searchResult.patient.personalhealthnumber
                                "
                                disabled
                                label="PHN"
                            />
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <h2>COVID-19 Immunizations</h2>
                        </v-col>
                    </v-row>
                    <v-row dense>
                        <v-col no-gutters>
                            <v-data-table
                                :headers="tableHeaders"
                                :items="immunizations"
                                :items-per-page="5"
                                :hide-default-footer="true"
                            >
                                <template #[`item.date`]="{ item }">
                                    <span>{{ formatDate(item.date) }}</span>
                                </template>
                            </v-data-table>
                        </v-col>
                    </v-row>
                    <v-row justify="end">
                        <v-col class="text-right">
                            <v-btn
                                type="button"
                                class="primary"
                                :disabled="immunizations.length === 0"
                                @click="handlePrint()"
                            >
                                <span>Print</span>
                                <v-icon class="ml-2" size="sm"
                                    >fas fa-print</v-icon
                                >
                            </v-btn>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <h2>Mailing Address</h2>
                        </v-col>
                    </v-row>
                    <v-row align="center" dense>
                        <v-col>
                            <v-textarea
                                v-model="streetLines"
                                label="Address"
                                :disabled="!isEditMode"
                                auto-grow
                                rows="1"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                    </v-row>
                    <v-row align="center" dense>
                        <v-col cols sm="6" md="4">
                            <v-text-field
                                v-model="address.city"
                                label="City"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                        <v-col
                            v-if="provinceStateList.length > 0"
                            cols
                            sm="6"
                            md="4"
                        >
                            <v-select
                                v-model="address.state"
                                :items="provinceStateList"
                                label="Province/State"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                        <v-col v-else cols sm="6" md="4">
                            <v-text-field
                                v-model="address.state"
                                label="Province/State"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                        <v-col cols md="4">
                            <v-text-field
                                v-if="postalCodeMask !== undefined"
                                v-model="address.postalCode"
                                v-mask="postalCodeMask"
                                label="Postal Code"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                            <v-text-field
                                v-else
                                v-model="address.postalCode"
                                label="Postal Code"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                        <v-col cols md="8" xl="4">
                            <v-select
                                v-model="selectedDestination"
                                :items="internationalDestinations"
                                label="Country"
                                :disabled="!isEditMode"
                                autocomplete="chrome-off"
                            />
                        </v-col>
                    </v-row>
                    <v-row align="center" dense>
                        <v-col cols="auto">
                            <v-checkbox
                                v-model="isEditMode"
                                label="Mail to a different address"
                                @change="onEditModeChange"
                            />
                        </v-col>
                        <v-col class="text-right">
                            <v-btn
                                type="submit"
                                class="mx-2 primary"
                                :disabled="immunizations.length === 0"
                            >
                                <span>Mail</span>
                                <v-icon class="ml-2" size="sm"
                                    >fas fa-paper-plane</v-icon
                                >
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-form>
            </v-col>
        </v-row>
    </v-container>
</template>
