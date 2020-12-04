const { AuthMethod } = require("../../support/constants")

describe('Reports', () => {
    before(() => {
        cy.readConfig().as("config").then(config => {
            cy.server();
            cy.route('GET', '/v1/api/configuration/', config);
            cy.login(Cypress.env('keycloak.username'), Cypress.env('keycloak.password'), AuthMethod.KeyCloak, "/reports");
        })
    })

    it('Validate Service Selection', () => {       
        cy.get('[data-testid=exportRecordBtn]')
            .should('not.be.enabled', 'be.visible')

        cy.get('[data-testid=infoText]')
            .should('have.text', ' Select a service above to create a report ')

        cy.get('[data-testid=infoImage]')
            .should('be.visible')

        cy.get('[data-testid=reportType]')
            .should('be.enabled', 'be.visible')
            .select("MED")        

        cy.get('[data-testid=exportRecordBtn]')
            .should('be.enabled', 'be.visible')
    })

    it('Validate Medication Report', () => {         
        cy.get('[data-testid=medicationReportSample]')
            .should('be.visible')
        
        cy.get('[data-testid=medicationReportSample] #subject')
            .should('have.text', 'Health Gateway Medication History Report');

        cy.get('[data-testid=exportRecordBtn]')
            .should('be.enabled', 'be.visible')
            .click();

        cy.get('[data-testid=genericMessageModal]')
            .should('be.visible');

        cy.get('[data-testid=genericMessageText]')
            .should('have.text', ' The file that you are downloading contains personal information. If you are on a public computer, please ensure that the file is deleted before you log off. ');

        cy.get('[data-testid=genericMessageSubmitBtn]')
            .click();
            
        cy.get('[data-testid=genericMessageModal]')
            .should('not.be.visible');
    })
}) 