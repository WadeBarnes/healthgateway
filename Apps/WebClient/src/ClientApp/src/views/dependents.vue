<script lang="ts">
import { library } from "@fortawesome/fontawesome-svg-core";
import { faUserPlus } from "@fortawesome/free-solid-svg-icons";
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";

import DependentCardComponent from "@/components/dependentCard.vue";
import LoadingComponent from "@/components/loading.vue";
import NewDependentComponent from "@/components/modal/newDependent.vue";
import ResourceCentreComponent from "@/components/resourceCentre.vue";
import BannerError from "@/models/bannerError";
import type { WebClientConfiguration } from "@/models/configData";
import type { Dependent } from "@/models/dependent";
import User from "@/models/user";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.container";
import { IDependentService, ILogger } from "@/services/interfaces";
import ErrorTranslator from "@/utility/errorTranslator";

library.add(faUserPlus);

@Component({
    components: {
        LoadingComponent,
        DependentCardComponent,
        NewDependentComponent,
        "resource-centre": ResourceCentreComponent,
    },
})
export default class DependentsView extends Vue {
    @Ref("newDependentModal")
    readonly newDependentModal!: NewDependentComponent;

    @Getter("user", { namespace: "user" }) user!: User;

    @Getter("webClient", { namespace: "config" })
    webClientConfig!: WebClientConfiguration;

    @Action("addError", { namespace: "errorBanner" })
    addError!: (error: BannerError) => void;

    private logger!: ILogger;
    private dependentService!: IDependentService;

    private isLoading = true;
    private dependents: Dependent[] = [];

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        this.dependentService = container.get<IDependentService>(
            SERVICE_IDENTIFIER.DependentService
        );
        this.fetchDependents();
    }

    private fetchDependents() {
        this.isLoading = true;
        this.dependentService
            .getAll(this.user.hdid)
            .then((results) => {
                this.dependents = results;
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError("Fetch Dependents Error", err)
                );
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private showModal() {
        this.newDependentModal.showModal();
    }

    private hideModal() {
        this.newDependentModal.hideModal();
    }

    private needsUpdate() {
        this.fetchDependents();
    }
}
</script>
<template>
    <div class="m-3 flex-grow-1 d-flex flex-column">
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <b-row>
            <b-col class="col-12 col-lg-9 column-wrapper">
                <b-row>
                    <b-col>
                        <b-row id="pageTitle">
                            <b-col cols="7">
                                <h1 id="Subject" class="my-0">Dependents</h1>
                            </b-col>
                            <b-col cols="5" align-self="end">
                                <hg-button
                                    data-testid="addNewDependentBtn"
                                    class="float-right"
                                    variant="secondary"
                                    @click="showModal()"
                                >
                                    <hg-icon
                                        icon="user-plus"
                                        size="medium"
                                        class="mr-2"
                                    />
                                    <span>Add a new dependent</span>
                                </hg-button>
                            </b-col>
                        </b-row>
                        <hr />
                        <b-row id="pageTitle" class="mt-3">
                            <b-col>
                                <h5>
                                    You can add your dependents under the age of
                                    {{ webClientConfig.maxDependentAge }} to
                                    view their COVID-19 results. Please complete
                                    the form with your dependent's information,
                                    exactly as it appears on their BC Services
                                    Card.
                                </h5>
                            </b-col>
                        </b-row>
                    </b-col>
                </b-row>
                <b-row
                    v-for="dependent in dependents"
                    :key="dependent.hdid"
                    class="mt-2"
                >
                    <b-col>
                        <DependentCardComponent
                            :dependent="dependent"
                            @needs-update="needsUpdate"
                        />
                    </b-col>
                </b-row>
            </b-col>
        </b-row>
        <resource-centre />
        <NewDependentComponent
            ref="newDependentModal"
            @show="showModal"
            @handle-submit="fetchDependents"
        />
    </div>
</template>
<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

#pageTitle {
    color: $primary;
}

hr {
    border-top: 2px solid $primary;
}
</style>
