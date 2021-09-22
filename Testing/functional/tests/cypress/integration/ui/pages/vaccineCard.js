const { AuthMethod, localDevUri } = require("../../../support/constants");

const monthNames = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
];

const dobYearSelector =
    "[data-testid=dateOfBirthInput] [data-testid=formSelectYear]";
const dobMonthSelector =
    "[data-testid=dateOfBirthInput] [data-testid=formSelectMonth]";
const dobDaySelector =
    "[data-testid=dateOfBirthInput] [data-testid=formSelectDay]";
const dovYearSelector =
    "[data-testid=dateOfVaccineInput] [data-testid=formSelectYear]";
const dovMonthSelector =
    "[data-testid=dateOfVaccineInput] [data-testid=formSelectMonth]";
const dovDaySelector =
    "[data-testid=dateOfVaccineInput] [data-testid=formSelectDay]";

const feedbackPhnMustBeValidSelector = "[data-testid=feedbackPhnMustBeValid]";
const feedbackPhnIsRequiredSelector = "[data-testid=feedbackPhnIsRequired]";
const feedbackDobIsRequiredSelector = "[data-testid=feedbackDobIsRequired]";
const feedbackDovIsRequiredSelector = "[data-testid=feedbackDovIsRequired]";

const homeUrl = "/";
const vaccineCardUrl = "/vaccinecard";

const vaccinationStatusModule = "VaccinationStatus";

function enterVaccineCardPHN(phn) {
    cy.get("[data-testid=phnInput]")
        .should("be.visible", "be.enabled")
        .type(phn);
}

function select(selector, value) {
    cy.get(selector).should("be.visible", "be.enabled").select(value);
}

function selectExist(selector, value) {
    cy.get(selector)
        .children("[value=" + value + "]")
        .should("exist");
}

function selectNotExist(selector, value) {
    cy.get(selector)
        .children("[value=" + value + "]")
        .should("not.exist");
}

function clickVaccineCardEnterButton() {
    cy.get("[data-testid=btnEnter]").should("be.enabled", "be.visible").click();
}

describe("Vaccine Card Page", () => {
    it("Landing Page - Vaccine Card Button does not exist - Vaccine Status module disabled", () => {
        cy.enableModules({});
        cy.visit(homeUrl);
        cy.get("[data-testid=btnVaccineCard]").should("not.exist");
    });

    it("Landing Page - Log In - Vaccine Status module disabled and vaccine card URL entered directly", () => {
        cy.enableModules({});
        cy.visit(vaccineCardUrl);

        cy.get("[data-testid=BCSCBtn]").should("be.visible", "be.enabled");
        cy.get("[data-testid=IDIRBtn]").should("be.visible", "be.enabled");
        cy.get("[data-testid=KeyCloakBtn]").should("be.visible", "be.enabled");
    });

    it("Landing Page - Vaccine Card Button Exists - Vaccine Status module enabled", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(homeUrl);
        cy.get("[data-testid=btnVaccineCard]").should("exist");
    });

    it("Landing Page - Vaccination Card - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        cy.get("[data-testid=vaccineCardFormTitle]").should("be.visible");
        cy.get("[data-testid=btnCancel]").should("be.visible");
        cy.get("[data-testid=btnEnter]").should("be.enabled", "be.visible");
        cy.get("[data-testid=btnPrivacyStatement]").should("be.visible");
        cy.get("[data-testid=btnLogin]").should("be.enabled", "be.visible");
    });

    it("Vaccination Card - Cancel - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        cy.get("[data-testid=btnCancel]").should("be.visible").click();

        cy.get("[data-testid=btnVaccineCard]").should("exist");
    });

    it("Vaccination Card - Login BC Services Card App - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        cy.get("[data-testid=btnLogin]")
            .should("be.visible", "be.enabled")
            .click();

        cy.get("[data-testid=BCSCBtn]").should("be.visible", "be.enabled");
        cy.get("[data-testid=IDIRBtn]").should("be.visible", "be.enabled");
        cy.get("[data-testid=KeyCloakBtn]").should("be.visible", "be.enabled");
    });

    it("Vaccination Card - Valid PHN via Select Year - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dobYearSelector, "Year");

        cy.get("[data-testid=phnInput]")
            .should("be.visible", "be.enabled")
            .and("have.class", "form-control is-valid");
    });

    it("Vaccination Card - Invalid PHN via Select Year - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn").replace(/.$/, ""));

        select(dobYearSelector, "Year");

        cy.get(feedbackPhnMustBeValidSelector).should("be.visible");
    });

    it("Vaccination Card - PHN, DOB and DOV not entered via Click Enter - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        clickVaccineCardEnterButton();

        cy.get(feedbackPhnIsRequiredSelector).should("be.visible");
        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOB and DOV not entered with Invalid PHN via Click Enter - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn").replace(/.$/, ""));

        clickVaccineCardEnterButton();

        cy.get(feedbackPhnMustBeValidSelector).should("be.visible");
        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOB and DOV not entered via Click Enter - unauthenticated user", () => {
        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOB and DOV cannot be future dated - unauthenticated user", () => {
        const d = new Date();
        const year = d.getFullYear();
        const monthNumber = d.getMonth(); //Maps to monthNames constant array. Index starts at 0
        const month = d.getMonth() + 1; //0 is January but UI index starts at 1 for January
        const nextMonth = d.getMonth() === 11 ? 1 : month + 1; //When current month is December, next month will be January
        const day = d.getDate();

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        // Future Date
        d.setDate(d.getDate() + 1);
        const nextYear = d.getFullYear() === year ? year + 1 : d.getFullYear();
        const nextDay = d.getDate();

        // Test Future Year does not exist
        selectNotExist(dobYearSelector, nextYear.toString());
        selectNotExist(dovYearSelector, nextYear.toString());

        // Test Current Year
        select(dobYearSelector, year.toString());
        select(dovYearSelector, year.toString());

        if (nextMonth > 1) {
            // Current year has been set in dropdown, so if next month is 1 - January, it means
            // current month is December.
            // Test Future Month does not exist. Month can only be current or past month for current year.
            selectNotExist(dobMonthSelector, nextMonth);
            selectNotExist(dovMonthSelector, nextMonth);
        }

        // Test and set Current Month
        select(dobMonthSelector, monthNames[monthNumber]);
        select(dovMonthSelector, monthNames[monthNumber]);

        if (nextDay > 1) {
            // Current Year and Month have been set. If next day is 1, it means previous.
            // day was last day of current month. Next Day is associated with the current month.
            // Test Future Day in current month does not exist.
            selectNotExist(dobDaySelector, nextDay);
            selectNotExist(dovDaySelector, nextDay);
        }
        //Test Current Day exists
        selectExist(dobDaySelector, day);
        selectExist(dovDaySelector, day);
    });

    it("Vaccination Card - DOB Year and DOV not entered via Click Enter - unauthenticated user", () => {
        const dobMonth = "June";
        const dobDay = "15";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dobMonthSelector, dobMonth);

        select(dobDaySelector, dobDay);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOB Month and DOV not entered via Click Enter - unauthenticated user", () => {
        const dobYear = "2021";
        const dobDay = "15";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dobYearSelector, dobYear);

        select(dobDaySelector, dobDay);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOB Day and DOV not entered via Click Enter - unauthenticated user", () => {
        const dobYear = "2021";
        const dobMonth = "June";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dobYearSelector, dobYear);

        select(dobMonthSelector, dobMonth);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOV Year and DOB not entered via Click Enter - unauthenticated user", () => {
        const dovMonth = "June";
        const dovDay = "15";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dovMonthSelector, dovMonth);

        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOV Month and DOB not entered via Click Enter - unauthenticated user", () => {
        const dovYear = "2021";
        const dovDay = "15";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dovYearSelector, dovYear);

        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - DOV Day and DOB not entered via Click Enter - unauthenticated user", () => {
        const dovYear = "2021";
        const dovMonth = "June";

        cy.enableModules(vaccinationStatusModule);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(Cypress.env("phn"));

        select(dovYearSelector, dovYear);

        select(dovMonthSelector, dovMonth);

        clickVaccineCardEnterButton();

        cy.get(feedbackDobIsRequiredSelector).should("be.visible");
        cy.get(feedbackDovIsRequiredSelector).should("be.visible");
    });

    it("Vaccination Card - Partially Vaccinated 1 Dose - unauthenticated user", () => {
        const phn = "9735353315";
        const dobYear = "1967";
        const dobMonth = "June";
        const dobDay = "2";
        const dovYear = "2021";
        const dovMonth = "July";
        const dovDay = "4";

        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(phn);

        select(dobYearSelector, dobYear);
        select(dobMonthSelector, dobMonth);
        select(dobDaySelector, dobDay);
        select(dovYearSelector, dovYear);
        select(dovMonthSelector, dovMonth);
        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusPartiallyVaccinated]").should("be.visible");
        //cy.get("[data-testid=dose-1]").should("be.visible");
    });

    it("Vaccination Card - Fully Vaccinated 2 Doses - unauthenticated user", () => {
        const phn = "9735361219 ";
        const dobYear = "1994";
        const dobMonth = "June";
        const dobDay = "9";
        const dovYear = "2021";
        const dovMonth = "January";
        const dovDay = "20";

        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(phn);

        select(dobYearSelector, dobYear);
        select(dobMonthSelector, dobMonth);
        select(dobDaySelector, dobDay);
        select(dovYearSelector, dovYear);
        select(dovMonthSelector, dovMonth);
        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusVaccinated]").should("be.visible");
        //cy.get("[data-testid=dose-1]").should("be.visible");
        //cy.get("[data-testid=dose-2]").scrollIntoView().should("be.visible");
    });

    it("Vaccination Card - Fully Vaccinated 1 Dose - unauthenticated user", () => {
        const phn = "9000691107";
        const dobYear = "1987";
        const dobMonth = "March";
        const dobDay = "23";
        const dovYear = "2021";
        const dovMonth = "May";
        const dovDay = "15";

        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(phn);

        select(dobYearSelector, dobYear);
        select(dobMonthSelector, dobMonth);
        select(dobDaySelector, dobDay);
        select(dovYearSelector, dovYear);
        select(dovMonthSelector, dovMonth);
        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusVaccinated]").should("be.visible");
        //cy.get("[data-testid=dose-1]").should("be.visible");
    });

    it("Vaccination Card - Fully Vaccinated 3 Doses - unauthenticated user", () => {
        const phn = "9000691185";
        const dobYear = "1990";
        const dobMonth = "June";
        const dobDay = "21";
        const dovYear = "2021";
        const dovMonth = "March";
        const dovDay = "1";

        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(phn);

        select(dobYearSelector, dobYear);
        select(dobMonthSelector, dobMonth);
        select(dobDaySelector, dobDay);
        select(dovYearSelector, dovYear);
        select(dovMonthSelector, dovMonth);
        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusVaccinated]").should("be.visible");
        //cy.get("[data-testid=dose-1]").should("be.visible");
        //cy.get("[data-testid=dose-2]").scrollIntoView().should("be.visible");
        //cy.get("[data-testid=dose-3]").scrollIntoView().should("be.visible");
    });

    it("Vaccination Card - Fully Vaccinated 5 Doses - unauthenticated user", () => {
        const phn = "9876809694";
        const dobYear = "1964";
        const dobMonth = "June";
        const dobDay = "9";
        const dovYear = "2021";
        const dovMonth = "February";
        const dovDay = "1";

        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.visit(vaccineCardUrl);

        enterVaccineCardPHN(phn);

        select(dobYearSelector, dobYear);
        select(dobMonthSelector, dobMonth);
        select(dobDaySelector, dobDay);
        select(dovYearSelector, dovYear);
        select(dovMonthSelector, dovMonth);
        select(dovDaySelector, dovDay);

        clickVaccineCardEnterButton();

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusVaccinated]").should("be.visible");
        //cy.get("[data-testid=dose-1]").should("be.visible");
        //cy.get("[data-testid=dose-2]").scrollIntoView().should("be.visible");
        //cy.get("[data-testid=dose-3]").scrollIntoView().should("be.visible");
        //cy.get("[data-testid=dose-4]").scrollIntoView().should("be.visible");
        //cy.get("[data-testid=dose-5]").scrollIntoView().should("be.visible");
    });

    it("Landing Page - Vaccination Card - Registered Keycloak authenticated user", () => {
        cy.enableModules([
            "Immunization",
            vaccinationStatusModule,
            "VaccinationStatusPdf",
        ]);
        cy.login(
            Cypress.env("keycloak.username"),
            Cypress.env("keycloak.password"),
            AuthMethod.KeyCloak,
            "/covid19"
        );

        cy.get("[data-testid=formTitleVaccineCard]").should("be.visible");
        cy.get("[data-testid=statusPartiallyVaccinated]").should("be.visible");
        cy.get("[data-testid=dose-1]").should("be.visible");
        cy.get("[data-testid=dose-2]").scrollIntoView().should("be.visible");
    });

    it("Landing Page - Vaccination Card - Unregistered Keycloak authenticated user", () => {
        cy.enableModules(["Immunization", vaccinationStatusModule]);
        cy.login(
            Cypress.env("keycloak.unregistered.username"),
            Cypress.env("keycloak.password"),
            AuthMethod.KeyCloak,
            "/covid19"
        );

        cy.get("[data-testid=noCovidImmunizationsText]")
            .should("be.visible")
            .contains("No records found");
    });
});
