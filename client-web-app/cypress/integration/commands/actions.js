import { validatePage } from './asserts';

export const navigateToPage = (page) => {
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

// Success = rgb(26, 79, 73)
// Error = rgb(87, 41, 41)
// Warning = rgb(102, 76, 30)
// Info = rgb(40, 72, 98)
export const checkMessage = (id, text, color = 'rgb(26, 79, 73)') => {
    cy.get(`#${id}`)
        .should('be.visible')
        .and('contain.text', text)
        .and('have.css', 'color', color);
};
