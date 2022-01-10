/// <reference types="cypress" />

describe("CARD YIELD TESTS", () => {
    beforeEach('Login to user admin', () => {
        cy.visit("/");
        cy.get("#login-user-email").type(Cypress.env('adminEmail'));
        cy.get("#login-admin-password").type(Cypress.env('adminPassword'));
        cy.get("#login-signIn-button").should('be.visible').and('not.be.disabled');
        cy.get("#login-signIn-button").click();
    });

    it("Make Card Yield query", () => {
        //Open sidebar, get to page
        cy.get("#navbar-button").click();
        cy.get("#sidebar-cardyield").click();

        //Enter query fields
        cy.get("#cardyield-catalog-num").type('X56868');
        cy.get("#cardyield-start-date").type('2021-10-19');
        cy.get("#cardyield-end-date").type('2021-10-20');

        
        // Submit
        cy.get("#cardyield-submit-button").should('be.visible').and('not.be.disabled');
        cy.get("#cardyield-submit-button").click();

        cy.intercept('POST', '**/Queries/CardYield', {fixture: 'cardyield_data.json'}).as('result');
        cy.wait('@result');

        cy.get("#query-table").should('be.visible');
    });

    // it("Make Card Yield empty query", () => {
    //     //Open sidebar, get to page
    //     cy.get("#navbar-button").click();
    //     cy.get("#sidebar-cardyield").click();

    //     //Enter query fields
    //     cy.get("#cardyield-catalog-num").type('wrong');
    //     cy.get("#cardyield-start-date").type('2021-10-19');
    //     cy.get("#cardyield-end-date").type('2021-10-20');

    //     // Submit
    //     cy.get("#cardyield-submit-button").should('be.visible').and('not.be.disabled');
    //     cy.get("#cardyield-submit-button").click();

    //     cy.intercept('POST', '**/Queries/CardYield', {});
    // });
})