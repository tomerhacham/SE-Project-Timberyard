export const validatePage = (page) => {
    cy.url().should('eq', Cypress.config().baseUrl + '/' + page);
}