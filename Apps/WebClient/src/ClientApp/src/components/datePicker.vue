<script lang="ts">
import { library } from "@fortawesome/fontawesome-svg-core";
import { faCalendar } from "@fortawesome/free-solid-svg-icons";
import { BFormDatepicker } from "bootstrap-vue";
import Vue from "vue";
import {
    Component,
    Emit,
    Model,
    Prop,
    Ref,
    Watch,
} from "vue-property-decorator";
import { Validation } from "vuelidate/vuelidate";

import { DateWrapper } from "@/models/dateWrapper";

library.add(faCalendar);

@Component
export default class DatePickerComponent extends Vue {
    @Model("change", { type: String }) public model!: string;
    @Prop() state?: boolean;
    @Ref("datePicker") datePicker!: BFormDatepicker;

    private value = "";
    private inputValue = "";

    private mounted() {
        this.value = this.model;
    }

    private onFocus() {
        // This makes the calendar popup when the input receives focus
        // Currently disabled since it seems glitchy
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        // (this.datePicker.$refs.control as any).show();
    }

    @Watch("value")
    private onValueChanged() {
        this.inputValue = this.value
            ? new DateWrapper(this.value).format().toUpperCase()
            : "";
        this.updateModel();
    }

    @Watch("model")
    private onModelChanged() {
        this.value = this.model;
    }

    private onInputChanged() {
        this.$v.inputValue.$touch();
        if (this.isValid(this.$v.inputValue)) {
            this.value = this.inputValue
                ? DateWrapper.fromStringFormat(this.inputValue).toISODate()
                : "";
        }
    }

    @Emit("change")
    private updateModel() {
        return this.value;
    }

    @Emit("blur")
    private onBlur() {
        return;
    }

    private get getState() {
        let isValid = this.isValid(this.$v.inputValue);
        if (isValid || isValid == undefined) {
            return this.state;
        }
        return isValid;
    }

    private validations() {
        return {
            inputValue: {
                minValue: (value: string) =>
                    !value ||
                    DateWrapper.fromStringFormat(value).isAfter(
                        new DateWrapper("1900-01-01")
                    ),
                maxValue: (value: string) =>
                    !value ||
                    DateWrapper.fromStringFormat(value).isBefore(
                        new DateWrapper("2100-01-01")
                    ),
            },
        };
    }

    private isValid(param: Validation): boolean | undefined {
        return param.$dirty ? !param.$invalid : undefined;
    }
}
</script>

<template>
    <b-input-group>
        <b-form-input
            v-model="inputValue"
            v-mask="'####-AAA-##'"
            type="text"
            placeholder="YYYY-MMM-DD"
            autocomplete="off"
            :state="getState"
            @focus.native="onFocus"
            @blur.native="onBlur"
            @click.native.capture.stop
            @change="onInputChanged"
        ></b-form-input>
        <b-input-group-append>
            <b-form-datepicker
                ref="datePicker"
                v-model="value"
                menu-class="datepicker-style"
                button-only
                right
                locale="en-CA"
            >
                <template #button-content>
                    <hg-icon icon="calendar" size="small" />
                </template>
            </b-form-datepicker>
        </b-input-group-append>
    </b-input-group>
</template>

<style lang="scss">
@import "@/assets/scss/_variables.scss";
// Fixes datepicker displaing under site header on mobile
.datepicker-style {
    z-index: $z_datepicker;
}
</style>
