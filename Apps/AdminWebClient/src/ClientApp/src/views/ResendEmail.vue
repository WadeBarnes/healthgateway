<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

import BannerFeedbackComponent from "@/components/core/BannerFeedback.vue";
import LoadingComponent from "@/components/core/Loading.vue";
import { ResultType } from "@/constants/resulttype";
import BannerFeedback from "@/models/bannerFeedback";
import { DateWrapper, StringISODateTime } from "@/models/dateWrapper";
import Email from "@/models/email";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { IEmailAdminService } from "@/services/interfaces";

@Component({
    components: {
        LoadingComponent,
        BannerFeedbackComponent,
    },
})
export default class ResendEmailView extends Vue {
    private filterText = "";
    private isLoading = true;
    private showFeedback = false;
    private bannerFeedback: BannerFeedback = {
        type: ResultType.NONE,
        title: "",
        message: "",
    };

    private selectedEmails: Email[] = [];

    private tableHeaders = [
        {
            text: "Subject",
            value: "subject",
        },
        {
            text: "Status",
            value: "emailStatusCode",
        },
        {
            text: "Date",
            value: "sentDateTime",
        },
        { text: "Email", value: "to" },
        { text: "Is Invited?", value: "userInviteStatus" },
    ];

    private emailList: Email[] = [];

    private emailService!: IEmailAdminService;

    private mounted() {
        this.emailService = container.get(SERVICE_IDENTIFIER.EmailAdminService);
        this.loadEmails();
    }

    private loadEmails() {
        this.isLoading = true;
        this.emailList = [];
        this.selectedEmails = [];
        this.emailService
            .getEmails()
            .then((emails) => {
                this.emailList.push(...emails);
            })
            .catch((err) => {
                console.log(err);
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Failed to load emails",
                };
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private formatDateTime(date: StringISODateTime): string {
        if (!date) {
            return "";
        }
        return new DateWrapper(date, { isUtc: true }).format(
            DateWrapper.defaultDateTimeFormat
        );
    }

    private resendEmails(): void {
        this.isLoading = true;
        let selectedIds = this.selectedEmails.map((s) => s.id);
        this.emailService
            .resendEmails(selectedIds)
            .then(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Success,
                    title: "Success.",
                    message: "Emails queued to be sent",
                };
            })
            .catch((err) => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Sending emails failed, please try again",
                };
                console.log(err);
            })
            .finally(() => {
                this.isLoading = false;
                this.loadEmails();
            });
    }
}
</script>

<template>
    <v-container>
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <BannerFeedbackComponent
            :show-feedback.sync="showFeedback"
            :feedback="bannerFeedback"
            class="mt-5"
        ></BannerFeedbackComponent>
        <v-row justify="center">
            <v-col md="9">
                <v-text-field
                    v-model="filterText"
                    label="Filter"
                    hide-details="auto"
                >
                    <v-icon slot="append">fas fa-search</v-icon>
                </v-text-field>
            </v-col>
        </v-row>
        <v-row justify="center">
            <v-col md="9">
                <v-row>
                    <v-col no-gutters>
                        <v-data-table
                            v-model="selectedEmails"
                            :headers="tableHeaders"
                            :items="emailList"
                            :items-per-page="5"
                            show-select
                            :search="filterText"
                        >
                            <template #item.sentDateTime="{ item }">
                                <span>{{
                                    formatDateTime(item.sentDateTime)
                                }}</span>
                            </template>
                        </v-data-table>
                    </v-col>
                </v-row>
                <v-row justify="end" no-gutters>
                    <v-btn
                        :disabled="selectedEmails.length === 0"
                        @click="resendEmails()"
                        >Resend Emails</v-btn
                    >
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>
