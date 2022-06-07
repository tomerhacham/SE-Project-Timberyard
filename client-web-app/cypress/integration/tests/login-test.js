/// <reference types="cypress" />
import { checkMessage, navigateToPage } from '../commands/actions';
import { validatePage } from '../commands/asserts';
import {
    LOGIN_API,
    LOGIN_ALIAS,
    UNSUCCESSFUL_LOGIN_MESSAGE,
} from '../constants/constants';

describe('LOGIN TESTS', () => {
    it('Check login page fields/buttons', () => {
        cy.visit(Cypress.env('loginUrl'));
        cy.get('#login-email').should('be.visible');
        cy.get('#login-password').should('be.visible');
        cy.get('#login-signIn-button').should('be.disabled');
        cy.get('#login-signIn-button').should('contain.text', 'Sign In');

        // Check invalid email label
        cy.get('#login-email').type('user@');
        cy.get('#login-email-label').should(
            'have.css',
            'color',
            'rgb(209, 67, 67)'
        );
        cy.get('#login-email').type('gmail.com');
        cy.get('#login-email-label').should(
            'have.css',
            'color',
            'rgb(80, 72, 229)'
        );

        // Don't have password
        cy.get('#password-toggle-button').click();
        cy.get('#login-email').should('be.visible');
        cy.get('#login-password').should('not.exist');
        cy.get('#login-signIn-button').should('contain.text', 'Send Code');
        cy.get('#forgot-password-button').should('not.exist');

        // Forgot password
        cy.get('#password-toggle-button').click();
        cy.get('#forgot-password-button').should('exist');
        cy.get('#forgot-password-button').click();
        cy.get('#login-page-message')
            .should('be.visible')
            .and(
                'contain.text',
                'Please enter your email address to reset your password'
            );
    });

    it('Login as admin, validate redirect after success', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/admin_login_response.json',
        }).as(LOGIN_ALIAS);

        cy.login(Cypress.env('adminEmail'), Cypress.env('adminPassword'));
        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));
        });
    });

    it('Login as admin, validate settings page visible', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/admin_login_response.json',
        }).as(LOGIN_ALIAS);

        cy.login(Cypress.env('adminEmail'), Cypress.env('adminPassword'));
        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));
            navigateToPage('settings');
        });
    });

    it('Login as regular user, validate redirect after success', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/user_login_response.json',
        }).as(LOGIN_ALIAS);

        cy.login(Cypress.env('userEmail'), Cypress.env('userPassword'));
        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));
        });
    });

    it('Login as regular user, validate settings page not visible', () => {
        cy.intercept('POST', LOGIN_API, {
            fixture: 'authentication/user_login_response.json',
        }).as(LOGIN_ALIAS);

        cy.login(Cypress.env('userEmail'), Cypress.env('userPassword'));
        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('dashboardUrl'));
            cy.get(`#sidebar-settings`).should('not.exist');
        });
    });

    it('Unsuccessful attempt to login, validate page/message', () => {
        cy.fixture('authentication/admin_login_response.json').then(
            (response) => {
                response.status = false;
                response.message = UNSUCCESSFUL_LOGIN_MESSAGE;

                cy.intercept('POST', LOGIN_API, response).as(LOGIN_ALIAS);
            }
        );

        cy.login('fail@gmail.com', 'IncorrectPwd1');
        cy.wait(`@${LOGIN_ALIAS}`).then(() => {
            validatePage(Cypress.env('loginUrl'));
            checkMessage(
                'login-page-message',
                UNSUCCESSFUL_LOGIN_MESSAGE,
                'rgb(87, 41, 41)'
            );
        });
    });
});
