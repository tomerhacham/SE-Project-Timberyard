/// <reference types="cypress" />
import {
    checkMessage,
    navigateToPage,
    assertCellValueInFirstRow,
} from '../commands/actions';
import { validatePage } from '../commands/asserts';
import {
    LOGIN_API,
    LOGIN_ALIAS,
    GET_ALL_ALARMS_API,
    GET_ALL_ALARMS_ALIAS,
    SUCCESS_CODE,
    ADD_ALARM_API,
    ADD_ALARM_ALIAS,
    EDIT_ALARM_API,
    EDIT_ALARM_ALIAS,
    REMOVE_ALARM_API,
    REMOVE_ALARM_ALIAS,
    GET_USERS_API,
    GET_USERS_ALIAS,
} from '../constants/constants';

describe('ALARMS TESTS', () => {
    beforeEach('Login to user admin', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/admin_login_response.json',
        }).as(LOGIN_ALIAS);

        // just to discard when visiting settings page
        cy.intercept('GET', GET_USERS_API, {}).as(GET_USERS_ALIAS);

        cy.intercept('POST', GET_ALL_ALARMS_API, {
            fixture: 'alarms/get_alarms_response.json',
        }).as(GET_ALL_ALARMS_ALIAS);

        cy.login(Cypress.env('adminEmail'), Cypress.env('adminPassword'));

        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));

            navigateToPage('settings');
            cy.get('#alarms-settings-expand').click();
        });
    });

    it('Check data fetched correctly', () => {
        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.length);

                assertCellValueInFirstRow('active', 'true');
                assertCellValueInFirstRow('name', 'Alarm 1');
                assertCellValueInFirstRow('field', 'Catalog');
                assertCellValueInFirstRow('objective', 'X568678');
                assertCellValueInFirstRow('threshold', '3');
                assertCellValueInFirstRow(
                    'receivers',
                    'raz@ribbon.com, tomer@ribbon.com'
                );
            });
    });

    it('Check buttons states', () => {
        cy.get('#add-alarm-button').should('be.enabled');
        cy.get('#edit-alarm-button').should('be.disabled');
        cy.get('#remove-alarm-button').should('be.disabled');

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            // Row selected
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#add-alarm-button').should('be.enabled');
            cy.get('#edit-alarm-button').should('be.enabled');
            cy.get('#remove-alarm-button').should('be.enabled');

            // Unselect
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#add-alarm-button').should('be.enabled');
            cy.get('#edit-alarm-button').should('be.disabled');
            cy.get('#remove-alarm-button').should('be.disabled');
        });
    });

    it('Check Add Alarm Dialog + Email add/delete', () => {
        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('#add-alarm-button').click();
            // Check dialog title, fields
            cy.get('h2').contains('Add New Alarm').should('exist');
            cy.get('#name-textfield-dialog').should('be.visible');
            cy.get('#field-select-dialog').should('be.visible');
            cy.get('#objective-textfield-dialog').should('be.visible');
            cy.get('#threshold-textfield-dialog').should('be.visible');
            cy.get('#email-chip-textfield').should('be.visible');

            // Check email add/delete
            cy.get('#email-chip-textfield').type('user@ribbon.com');
            cy.get('#chip-add-button').click();
            cy.get('#chip-item').should('contain', 'user@ribbon.com');
            cy.get('svg[data-testid="CancelIcon"').click();
            cy.get('#chip-item').should('not.exist');
        });
    });

    it('Check Edit Alarm Dialog', () => {
        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#edit-alarm-button').click();
            cy.get('h2').contains('Edit Alarm Details').should('exist');
            cy.get('#name-textfield-dialog').should('be.visible');
            cy.get('#field-select-dialog').should('be.visible');
            cy.get('#objective-textfield-dialog').should('be.visible');
            cy.get('#threshold-textfield-dialog').should('be.visible');
            cy.get('#active-textfield-dialog').should('be.visible');
            cy.get('#email-chip-textfield').should('be.visible');
        });
    });

    it('Check Remove Alarm Confirmation Window', () => {
        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#remove-alarm-button').click();

            cy.on('window:confirm', (text) => {
                expect(text).to.contains(
                    'Are you sure you want to remove this alarm?'
                );
            });
        });
    });

    it('Check Adding Alarm Response', () => {
        cy.intercept('POST', ADD_ALARM_API, {
            body: {
                status: true,
                message: 'Alarm created successfully',
                data: null,
            },
        }).as(ADD_ALARM_ALIAS);

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('#add-alarm-button').click();
            cy.get('#name-textfield-dialog').type('Test Alarm');
            cy.get('#field-select-dialog').click();
            cy.get('#menu-item-field-Station').click();
            cy.get('#objective-textfield-dialog').type('ABC1234');
            cy.get('#threshold-textfield-dialog').clear().type('4');
            cy.get('#email-chip-textfield').type('user@ribbon.com');
            cy.get('#chip-add-button').click();

            cy.get('#submit-dialog-button').click();
            cy.wait(`@${ADD_ALARM_ALIAS}`).then(() => {
                checkMessage(
                    'alarms-settings-message',
                    'Alarm created successfully'
                );
            });
        });
    });

    it('Check Adding Alarm Bad Response', () => {
        cy.intercept('POST', ADD_ALARM_API, {
            body: {
                status: false,
                message: 'Error in adding alarm',
                data: null,
            },
        }).as(ADD_ALARM_ALIAS);

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('#add-alarm-button').click();
            cy.get('#name-textfield-dialog').type('Test Alarm');
            cy.get('#field-select-dialog').click();
            cy.get('#menu-item-field-Station').click();
            cy.get('#objective-textfield-dialog').type('ABC1234');
            cy.get('#threshold-textfield-dialog').clear().type('4');
            cy.get('#email-chip-textfield').type('user@ribbon.com');
            cy.get('#chip-add-button').click();

            cy.get('#submit-dialog-button').click();
            cy.wait(`@${ADD_ALARM_ALIAS}`).then(() => {
                checkMessage(
                    'alarms-settings-message',
                    'Error in adding alarm',
                    'rgb(87, 41, 41)'
                );
            });
        });
    });

    it('Check Editing Alarm Response, Data Updates', () => {
        cy.intercept('POST', EDIT_ALARM_API, {
            body: {
                status: true,
                message: 'Alarm edited successfully',
                data: null,
            },
        }).as(EDIT_ALARM_ALIAS);

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#edit-alarm-button').click();
            // Edit name, threshold and delete first receiver
            cy.get('#name-textfield-dialog').clear().type('New Name');
            cy.get('#threshold-textfield-dialog').clear().type('20');
            cy.get('svg[data-testid="CancelIcon"').first().click();
            cy.get('#submit-dialog-button').click();

            cy.wait(`@${EDIT_ALARM_ALIAS}`).then(() => {
                checkMessage(
                    'alarms-settings-message',
                    'Alarm edited successfully'
                );
                assertCellValueInFirstRow('name', 'New Name');
                assertCellValueInFirstRow('threshold', '20');
                assertCellValueInFirstRow('receivers', 'tomer@ribbon.com');
            });
        });
    });

    it('Check Editing Alarm Bad Response, Data Did not Update', () => {
        cy.intercept('POST', EDIT_ALARM_API, {
            body: {
                status: false,
                message: 'Error in editing alarm',
                data: null,
            },
        }).as(EDIT_ALARM_ALIAS);

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#edit-alarm-button').click();
            // Edit name, threshold and delete first receiver
            cy.get('#name-textfield-dialog').clear().type('New Name');
            cy.get('#threshold-textfield-dialog').clear().type('20');
            cy.get('svg[data-testid="CancelIcon"').first().click();
            cy.get('#submit-dialog-button').click();

            cy.wait(`@${EDIT_ALARM_ALIAS}`).then(() => {
                checkMessage(
                    'alarms-settings-message',
                    'Error in editing alarm',
                    'rgb(87, 41, 41)'
                );
                // Assert values did not change
                assertCellValueInFirstRow('name', 'Alarm 1');
                assertCellValueInFirstRow('threshold', '3');
                assertCellValueInFirstRow(
                    'receivers',
                    'raz@ribbon.com, tomer@ribbon.com'
                );
            });
        });
    });

    it('Check Removing Alarm Response', () => {
        cy.intercept('POST', REMOVE_ALARM_API, {
            body: {
                status: true,
                message: 'Alarm deleted successfully',
                data: null,
            },
        }).as(REMOVE_ALARM_ALIAS);

        cy.wait(`@${GET_ALL_ALARMS_ALIAS}`).then(() => {
            cy.get('[class=ag-selection-checkbox]').first().click();
            cy.get('#remove-alarm-button').click();

            cy.wait(`@${REMOVE_ALARM_ALIAS}`).then(() => {
                checkMessage(
                    'alarms-settings-message',
                    'Alarm deleted successfully'
                );
            });
        });
    });
});
