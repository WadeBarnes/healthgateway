<script lang="ts">
// Load Bootstrap general plugins
import {
    AlertPlugin,
    ButtonPlugin,
    CardPlugin,
    FormCheckboxPlugin,
    FormDatepickerPlugin,
    FormGroupPlugin,
    FormInputPlugin,
    FormPlugin,
    FormRadioPlugin,
    FormRatingPlugin,
    FormSelectPlugin,
    FormTextareaPlugin,
    IconsPlugin,
    InputGroupPlugin,
    LayoutPlugin,
    LinkPlugin,
    ModalPlugin,
    NavbarPlugin,
    NavPlugin,
    PaginationNavPlugin,
    SpinnerPlugin,
    TablePlugin,
    TooltipPlugin,
} from "bootstrap-vue";

Vue.use(LayoutPlugin);
Vue.use(NavPlugin);
Vue.use(NavbarPlugin);
Vue.use(ModalPlugin);
Vue.use(ButtonPlugin);
Vue.use(CardPlugin);
Vue.use(LinkPlugin);
Vue.use(FormPlugin);
Vue.use(FormInputPlugin);
Vue.use(FormCheckboxPlugin);
Vue.use(FormTextareaPlugin);
Vue.use(AlertPlugin);
Vue.use(SpinnerPlugin);
Vue.use(InputGroupPlugin);
Vue.use(PaginationNavPlugin);
Vue.use(TablePlugin);
Vue.use(TooltipPlugin);
Vue.use(FormGroupPlugin);
Vue.use(FormRadioPlugin);
Vue.use(FormRatingPlugin);
Vue.use(FormSelectPlugin);
Vue.use(FormDatepickerPlugin);
Vue.use(IconsPlugin);
Vue.use(VueTheMask);

// Load general icons
import { config } from "@fortawesome/fontawesome-svg-core";
// Prevent auto adding CSS to the header since that breaks Content security policies.
config.autoAddCss = false;
// Add font awesome styles manually
import "@fortawesome/fontawesome-svg-core/styles.css";

import Vue from "vue";
import { Component, Ref, Watch } from "vue-property-decorator";
import VueTheMask from "vue-the-mask";
import { Action, Getter } from "vuex-class";

import CommunicationComponent from "@/components/communication.vue";
import ErrorCard from "@/components/errorCard.vue";
import IdleComponent from "@/components/modal/idle.vue";
import FooterComponent from "@/components/navmenu/navFooter.vue";
import HeaderComponent from "@/components/navmenu/navHeader.vue";
import SidebarComponent from "@/components/navmenu/sidebar.vue";
import Process, { EnvironmentType } from "@/constants/process";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import { ILogger } from "@/services/interfaces";

import ImmunizationCardComponent from "./components/modal/immunizationCard.vue";
import ScreenWidth from "./constants/screenWidth";
import container from "./plugins/inversify.container";

const logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);

@Component({
    components: {
        NavHeader: HeaderComponent,
        NavFooter: FooterComponent,
        NavSidebar: SidebarComponent,
        ErrorCard: ErrorCard,
        IdleComponent,
        CommunicationComponent,
        "immunization-modal-card": ImmunizationCardComponent,
    },
})
export default class App extends Vue {
    @Ref("idleModal") readonly idleModal!: IdleComponent;
    @Action("setIsMobile") setIsMobile!: (isMobile: boolean) => void;
    @Getter("isMobile") isMobile!: boolean;
    @Getter("oidcIsAuthenticated", { namespace: "auth" })
    oidcIsAuthenticated?: boolean;

    private readonly host: string =
        window.location.hostname.toLocaleUpperCase();
    private readonly isProduction: boolean =
        Process.NODE_ENV == EnvironmentType.production &&
        (this.host.startsWith("HEALTHGATEWAY") ||
            this.host.startsWith("WWW.HEALTHGATEWAY"));

    private windowWidth = 0;
    private vaccinationStatusPath = "/vaccination-status";

    constructor() {
        super();
        logger.debug(
            `Node ENV: ${JSON.stringify(
                Process.NODE_ENV
            )}; host: ${JSON.stringify(this.host)}`
        );
    }

    @Watch("isAppIdle")
    private onIsAppIdleChanged(idle: boolean) {
        if (idle && this.oidcIsAuthenticated) {
            this.idleModal.show();
        }
    }

    private created() {
        this.windowWidth = window.innerWidth;
        this.$nextTick(() => {
            window.addEventListener("resize", this.onResize);
            this.onResize();
        });
    }

    private beforeDestroy() {
        window.removeEventListener("resize", this.onResize);
    }

    private onResize() {
        this.windowWidth = window.innerWidth;

        if (this.windowWidth < ScreenWidth.Mobile) {
            if (!this.isMobile) {
                this.setIsMobile(true);
            }
        } else {
            if (this.isMobile) {
                this.setIsMobile(false);
            }
        }
    }

    private get isHeaderVisible(): boolean {
        return this.$route.path !== this.vaccinationStatusPath;
    }

    private get isFooterVisible(): boolean {
        return this.$route.path !== this.vaccinationStatusPath;
    }

    private get isCommunicationVisible(): boolean {
        return this.$route.path !== this.vaccinationStatusPath;
    }
}
</script>

<template>
    <div id="app-root" class="container-fluid-fill d-flex h-100 flex-column">
        <div v-if="!isProduction" class="devBanner">
            <div class="text-center bg-warning small">
                Non-production environment:
                <strong>{{ host }}</strong>
            </div>
        </div>

        <NavHeader v-show="isHeaderVisible" />
        <b-row>
            <NavSidebar class="no-print sticky-top vh-100" />
            <main class="col fill-height d-flex flex-column">
                <CommunicationComponent v-show="isCommunicationVisible" />
                <ErrorCard
                    title="Whoops!"
                    description="An error occurred."
                    show="true"
                />
                <router-view></router-view>
                <IdleComponent ref="idleModal" />
            </main>
        </b-row>

        <footer v-show="isFooterVisible" class="footer no-print">
            <NavFooter />
        </footer>

        <immunization-modal-card />
    </div>
</template>

<style lang="scss" scoped>
.row {
    margin: 0px;
    padding: 0px;
}

.col {
    margin: 0px;
    padding: 0px;
}
</style>
<style lang="scss">
@import "@/assets/scss/_variables.scss";
@media print {
    .navbar {
        display: flex !important;
    }

    .no-print,
    .no-print * {
        display: none !important;
    }
}

html {
    height: 100vh;
}

body {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
}

main {
    padding-bottom: 0px;
    padding-top: 0px;
}

#app-root {
    min-height: 100vh;
}

.fill-height {
    margin: 0px;
    min-height: 100vh;
}

.devBanner {
    z-index: $z_header;
}

label.hg-label {
    font-weight: bold;
}

.is-invalid .hg-label {
    color: $danger;
}
</style>
