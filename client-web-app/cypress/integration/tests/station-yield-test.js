/// <reference types="cypress" />
import { validatePage } from '../commands/asserts';
import { STATION_YIELD_POST, STATION_YIELD_API } from '../constants/constants';

describe("STATION YIELD TESTS", () => {
    beforeEach('Login to user admin', () => {
        cy.visit(Cypress.env('loginUrl'));
        cy.get("#login-user-email").type(Cypress.env('adminEmail'));
        cy.get("#login-admin-password").type(Cypress.env('adminPassword'));
        cy.get("#login-signIn-button").should('be.visible').and('not.be.disabled');
        cy.get("#login-signIn-button").click();
        validatePage(Cypress.env('dashboardUrl'));

        // Open sidebar, get to page
        cy.get("#navbar-button").click();
        cy.get("#sidebar-stationyield").click();
        validatePage(Cypress.env('stationYieldUrl'));
    });

    it("Check Loader appears correctly + empty response", () => {
        let sendResponse;
        const trigger = new Promise((resolve) => {
            sendResponse = resolve;
        });

        cy.intercept('POST', STATION_YIELD_API, (req) => {
            return trigger.then(() => {
                req.reply({fixture: 'empty_query_response.json'});
            });
        });

        //Enter query fields
        cy.get('#stationYield-startDate').type('2021-10-15');
        cy.get('#stationYield-endDate').type('2021-10-30');

        // Submit
        cy.get('#stationYield-submit-button').click();

        // Check for loader
        cy.get('#loader').should('be.visible')
            .then(() => {
                sendResponse(); // trigger reply
                cy.get('#loader').should('not.exist');
                cy.get('#query-table').find('div').contains('No rows').should('exist');
            })
    });

    it("Check query data renders as expected", () => {
        cy.intercept('POST', STATION_YIELD_API, {fixture: 'station_yield_data.json'})
            .as(STATION_YIELD_POST);
        
        //Enter query fields
        cy.get('#stationYield-startDate').type('2021-10-19');
        cy.get('#stationYield-endDate').type('2021-10-20');

        // Submit
        cy.get('#stationYield-submit-button').click();

        cy.wait(`@${STATION_YIELD_POST}`);
        cy.get('#query-table').should('be.visible');
        // cy.get('canvas[role="img"]').should('be.visible');
    });
})