import { validatePage } from './asserts';

export const NavigateToPage = (page) => {
    cy.get('#navbar-button').click();
    cy.get(`#sidebar-${page}`).click();
    validatePage(Cypress.env(`${page}Url`));
};

export const assertCellValueInFirstRow = (colId, value) => {
    cy.get('.ag-center-cols-container .ag-row')
        .first()
        .find(`[col-id="${colId}"]`)
        .then((cell) => {
            expect(cell).to.have.text(value);
        });
};
