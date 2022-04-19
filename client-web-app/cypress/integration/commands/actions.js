import { validatePage } from "./asserts";

export const NavigateToPage = (page) => {
    cy.get('#navbar-button').click();
    cy.get(`#sidebar-${page}`).click();
    validatePage(Cypress.env(`${page}Url`));
}