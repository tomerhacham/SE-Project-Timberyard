/// <reference types="cypress" />
import { checkMessage, NavigateToPage } from '../commands/actions';
import { validatePage } from '../commands/asserts';
import {
    LOGIN_API,
    LOGIN_ALIAS,
    GET_ALL_ALARMS_API,
    GET_ALL_ALARMS_POST,
    ADD_USER_API,
    ADD_USER_ALIAS,
    ADD_SYSTEM_ADMIN_ALIAS,
    ADD_SYSTEM_ADMIN_API,
    UNSUCCESSFUL_ADD_USER_MESSAGE,
    REMOVE_USER_API,
    REMOVE_USER_ALIAS,
    UNSUCCESSFUL_REMOVE_USER_MESSAGE,
} from '../constants/constants';

describe('LOGIN TESTS', () => {
    beforeEach('Login to user admin', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/admin_login_response.json',
        }).as(LOGIN_ALIAS);

        // just to discard when visiting settings page
        cy.intercept('POST', GET_ALL_ALARMS_API, {}).as(GET_ALL_ALARMS_POST);

        cy.login(Cypress.env('adminEmail'), Cypress.env('adminPassword'));

        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));

            NavigateToPage('settings');
            cy.get('#users-settings-expand').click();
            cy.get('#users-settings-add-button').should('be.disabled');
            cy.get('#users-settings-remove-button').should('be.disabled');
        });
    });

    it('Check add Regular User', () => {
        cy.intercept('POST', ADD_USER_API, {
            fixture: 'authentication/manage_user_response.json',
        }).as(ADD_USER_ALIAS);

        cy.get('#users-settings-email-input').type('newUser@ribbon.com');
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_USER_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'User added successfully');
        });
    });

    it('Check add System Admin', () => {
        cy.intercept('POST', ADD_SYSTEM_ADMIN_API, { body: true }).as(
            ADD_SYSTEM_ADMIN_ALIAS
        );

        cy.get('#users-settings-email-input').type('newAdmin@ribbon.com');
        cy.get('#users-settings-role-select').click();
        cy.get('#menu-item-role-Admin').click();
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_SYSTEM_ADMIN_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'Success');
        });
    });

    it('Check bad add Regular User request', () => {
        cy.fixture('authentication/manage_user_response.json').then(
            (response) => {
                response.status = false;
                response.message = UNSUCCESSFUL_ADD_USER_MESSAGE;

                cy.intercept('POST', ADD_USER_API, response).as(ADD_USER_ALIAS);
            }
        );

        cy.get('#users-settings-email-input').type('newUser@ribbon.com');
        cy.get('#users-settings-add-button').click();

        cy.wait(`@${ADD_USER_ALIAS}`).then(() => {
            checkMessage(
                'users-settings-message',
                UNSUCCESSFUL_ADD_USER_MESSAGE,
                'rgb(87, 41, 41)'
            );
        });
    });

    it('Check bad add System Admin request', () => {
        cy.intercept('POST', ADD_SYSTEM_ADMIN_API, { body: false }).as(
            ADD_SYSTEM_ADMIN_ALIAS
        );

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

    it('Check remove user', () => {
        cy.fixture('authentication/manage_user_response.json').then(
            (response) => {
                response.message = 'User removed successfully';

                cy.intercept('POST', REMOVE_USER_API, response).as(
                    REMOVE_USER_ALIAS
                );
            }
        );

        cy.get('#users-settings-email-input').type('oldUser@ribbon.com');
        cy.get('#users-settings-remove-button').click();

        cy.wait(`@${REMOVE_USER_ALIAS}`).then(() => {
            checkMessage('users-settings-message', 'User removed successfully');
        });
    });

    it('Check bad remove user request', () => {
        cy.fixture('authentication/manage_user_response.json').then(
            (response) => {
                response.status = false;
                response.message = UNSUCCESSFUL_REMOVE_USER_MESSAGE;

                cy.intercept('POST', REMOVE_USER_API, response).as(
                    REMOVE_USER_ALIAS
                );
            }
        );

        cy.get('#users-settings-email-input').type('oldUser@ribbon.com');
        cy.get('#users-settings-remove-button').click();

        cy.wait(`@${REMOVE_USER_ALIAS}`).then(() => {
            checkMessage(
                'users-settings-message',
                UNSUCCESSFUL_REMOVE_USER_MESSAGE,
                'rgb(87, 41, 41)'
            );
        });
    });
});
