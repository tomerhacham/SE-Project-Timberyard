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
    GET_USERS_API,
    GET_USERS_ALIAS,
    ADD_USER_API,
    ADD_USER_ALIAS,
    ADD_SYSTEM_ADMIN_ALIAS,
    ADD_SYSTEM_ADMIN_API,
    REMOVE_USER_API,
    REMOVE_USER_ALIAS,
    CHANGE_PASSWORD_API,
    CHANGE_PASSWORD_ALIAS,
    SUCCESS_CODE,
} from '../constants/constants';

describe('SETTINGS TESTS', () => {
    beforeEach('Login to user admin', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/admin_login_response.json',
        }).as(LOGIN_ALIAS);

        // just to discard when visiting settings page
        cy.intercept('POST', GET_ALL_ALARMS_API, {}).as(GET_ALL_ALARMS_ALIAS);

        cy.intercept('GET', GET_USERS_API, {
            fixture: 'authentication/get_users_response.json',
        }).as(GET_USERS_ALIAS);

        cy.login(Cypress.env('adminEmail'), Cypress.env('adminPassword'));

        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));
            navigateToPage('settings');
        });
    });

    it('Check data fetched correctly', () => {
        cy.wait(`@${GET_USERS_ALIAS}`)
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

                assertCellValueInFirstRow('email', 'admin@timberyard.rbbn.com');
                assertCellValueInFirstRow('role', 'System Admin');
            });
    });

    it('USERS - Check fields', () => {
        cy.get('#users-settings-expand').click();
        cy.get('#users-settings-email-input').should('be.visible');
        cy.get('#users-settings-role-select')
            .should('be.visible')
            .and('contain', 'RegularUser');
        cy.get('#users-settings-add-button').should('be.disabled');
        cy.get('#users-settings-remove-button').should('be.disabled');
    });

    it('USERS - Check add Regular User', () => {
        cy.intercept('POST', ADD_USER_API, {
            body: {
                status: true,
                message: 'User added successfully',
                data: null,
            },
        }).as(ADD_USER_ALIAS);

        cy.get('#users-settings-expand').click();
        cy.get('#users-settings-email-input').type('newUser@ribbon.com');
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_USER_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'User added successfully');
        });
    });

    it('USERS - Check add System Admin', () => {
        cy.intercept('POST', ADD_SYSTEM_ADMIN_API, { body: true }).as(
            ADD_SYSTEM_ADMIN_ALIAS
        );

        cy.get('#users-settings-expand').click();
        cy.get('#users-settings-email-input').type('newAdmin@ribbon.com');
        cy.get('#users-settings-role-select').click();
        cy.get('#menu-item-role-Admin').click();
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_SYSTEM_ADMIN_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'Success');
        });
    });

    it('USERS - Check bad add Regular User request', () => {
        cy.intercept('POST', ADD_USER_API, {
            body: {
                status: false,
                message: 'User already exists',
                data: null,
            },
        }).as(ADD_USER_ALIAS);

        cy.get('#users-settings-expand').click();
        cy.get('#users-settings-email-input').type('newUser@ribbon.com');
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_USER_ALIAS}`).then(() => {
            checkMessage(
                'users-settings-message',
                'User already exists',
                'rgb(87, 41, 41)'
            );
        });
    });

    it('USERS - Check bad add System Admin request', () => {
        cy.intercept('POST', ADD_SYSTEM_ADMIN_API, { body: false }).as(
            ADD_SYSTEM_ADMIN_ALIAS
        );

        cy.get('#users-settings-expand').click();
        cy.get('#users-settings-email-input').type('newAdmin@ribbon.com');
        cy.get('#users-settings-role-select').click();
        cy.get('#menu-item-role-Admin').click();
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_SYSTEM_ADMIN_ALIAS}`).then(() => {
            checkMessage(
                'users-settings-message',
                'Error in adding admin user',
                'rgb(87, 41, 41)'
            );
        });
    });

    it('USERS - Check remove user', () => {
        cy.intercept('POST', REMOVE_USER_API, {
            body: {
                status: true,
                message: 'User removed successfully',
                data: null,
            },
        }).as(REMOVE_USER_ALIAS);

        cy.get('#users-settings-expand').click();
        cy.get('[class=ag-selection-checkbox]').first().click();
        cy.get('#users-settings-remove-button').click();

        cy.wait(`@${REMOVE_USER_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'User removed successfully');
        });
    });

    it('USERS - Check bad remove user request', () => {
        cy.intercept('POST', REMOVE_USER_API, {
            body: {
                status: false,
                message: 'No such user exists',
                data: null,
            },
        }).as(REMOVE_USER_ALIAS);

        cy.get('#users-settings-expand').click();
        cy.get('[class=ag-selection-checkbox]').first().click();
        cy.get('#users-settings-remove-button').click();

        cy.wait(`@${REMOVE_USER_ALIAS}`).then(() => {
            checkMessage(
                'users-settings-message',
                'No such user exists',
                'rgb(87, 41, 41)'
            );
        });
    });

    it('PASSWORD - Check password section fields', () => {
        cy.get('#password-settings-expand').click();
        cy.get('#old-password-input').should('be.visible');
        cy.get('#new-password-input').should('be.visible');
        cy.get('#confirm-password-input').should('be.visible');
        cy.get('#update-password-button').should('be.disabled');
    });

    it('PASSWORD - Check unmatching passwords message', () => {
        cy.get('#password-settings-expand').click();
        cy.get('#old-password-input').type('oldPassword');
        cy.get('#new-password-input').type('newPassword');
        cy.get('#confirm-password-input').type('unmatch123');
        cy.get('#update-password-button').should('be.enabled');
        cy.get('#update-password-button').click();
        checkMessage(
            'password-settings-message',
            'Passwords do not match',
            'rgb(102, 76, 30)'
        );
    });

    it('PASSWORD - Check successful password update', () => {
        cy.intercept('POST', CHANGE_PASSWORD_API, {
            body: {
                status: true,
                message: 'Password updated successfully',
                data: null,
            },
        }).as(CHANGE_PASSWORD_ALIAS);

        cy.get('#password-settings-expand').click();
        cy.get('#old-password-input').type(Cypress.env('adminPassword'));
        cy.get('#new-password-input').type('myNewPwd123');
        cy.get('#confirm-password-input').type('myNewPwd123');
        cy.get('#update-password-button').should('be.enabled');
        cy.get('#update-password-button').click();

        cy.wait(`@${CHANGE_PASSWORD_ALIAS}`).then(() => {
            checkMessage(
                'password-settings-message',
                'Password updated successfully'
            );
        });
    });

    it('Check unsuccessful password update', () => {
        cy.intercept('POST', CHANGE_PASSWORD_API, {
            body: {
                status: false,
                message: 'Old password is incorrect',
                data: null,
            },
        }).as(CHANGE_PASSWORD_ALIAS);

        cy.get('#password-settings-expand').click();
        cy.get('#old-password-input').type('faultyPwd');
        cy.get('#new-password-input').type('myNewPwd123');
        cy.get('#confirm-password-input').type('myNewPwd123');
        cy.get('#update-password-button').should('be.enabled');
        cy.get('#update-password-button').click();

        cy.wait(`@${CHANGE_PASSWORD_ALIAS}`).then(() => {
            checkMessage(
                'password-settings-message',
                'Old password is incorrect',
                'rgb(87, 41, 41)'
            );
        });
    });
});
