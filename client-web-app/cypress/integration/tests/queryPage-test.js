/// <reference types="cypress" />
import { validatePage } from '../commands/asserts';
import { NavigateToPage, assertCellValueInFirstRow } from '../commands/actions';
import {
    CARD_YIELD_POST,
    CARD_YIELD_API,
    STATION_YIELD_POST,
    STATION_YIELD_API,
    STATION_CARD_YIELD_POST,
    STATION_CARD_YIELD_API,
    NFF_POST,
    NFF_API,
    SUCCESS_CODE,
    TESTER_LOAD_API,
    TESTER_LOAD_POST,
    CARD_TEST_DURATION_API,
    CARD_TEST_DURATION_POST,
    BOUNDARIES_API,
    BOUNDARIES_POST,
} from '../constants/constants';

describe('QUERY PAGE TESTS', () => {
    beforeEach('Login to user admin', () => {
        cy.visit(Cypress.env('loginUrl'));
        cy.get('#login-email').type(Cypress.env('adminEmail'));
        cy.get('#login-password').type(Cypress.env('adminPassword'));
        cy.get('#login-signIn-button')
            .should('be.visible')
            .and('not.be.disabled');
        cy.get('#login-signIn-button').click();
        validatePage(Cypress.env('dashboardUrl'));
    });

    it('Check pages fields renders correctly', () => {
        NavigateToPage('cardYield');
        cy.get('#cardYield-catalog').should('be.visible');
        cy.get('#cardYield-startDate').should('be.visible');
        cy.get('#cardYield-endDate').should('be.visible');
        cy.get('#cardYield-submit-button').should('be.disabled');

        NavigateToPage('stationYield');
        cy.get('#stationYield-startDate').should('be.visible');
        cy.get('#stationYield-endDate').should('be.visible');
        cy.get('#stationYield-submit-button').should('be.disabled');

        NavigateToPage('stationCardYield');
        cy.get('#stationCardYield-station').should('be.visible');
        cy.get('#stationCardYield-catalog').should('be.visible');
        cy.get('#stationCardYield-startDate').should('be.visible');
        cy.get('#stationCardYield-endDate').should('be.visible');
        cy.get('#stationCardYield-submit-button').should('be.disabled');

        NavigateToPage('nff');
        cy.get('#nff-cardName').should('be.visible');
        cy.get('#nff-startDate').should('be.visible');
        cy.get('#nff-endDate').should('be.visible');
        cy.get('#nff-timeInterval').should('be.visible');
        cy.get('#nff-submit-button').should('be.disabled');

        NavigateToPage('boundaries');
        cy.get('#boundaries-catalog').should('be.visible');
        cy.get('#boundaries-startDate').should('be.visible');
        cy.get('#boundaries-endDate').should('be.visible');

        NavigateToPage('testerLoad');
        cy.get('#testerLoad-startDate').should('be.visible');
        cy.get('#testerLoad-endDate').should('be.visible');

        NavigateToPage('cardTestDuration');
        cy.get('#cardTestDuration-catalog').should('be.visible');
        cy.get('#cardTestDuration-startDate').should('be.visible');
        cy.get('#cardTestDuration-endDate').should('be.visible');
    });

    it('CARD YIELD - Check Loader appears correctly + empty response', () => {
        let sendResponse;
        const trigger = new Promise((resolve) => {
            sendResponse = resolve;
        });

        cy.intercept('POST', CARD_YIELD_API, (req) => {
            return trigger.then(() => {
                req.reply({ fixture: 'queries/empty_query_response.json' });
            });
        });

        NavigateToPage('cardYield');
        cy.get('#cardYield-catalog').type('randomInput');
        cy.get('#cardYield-startDate').type('2021-10-15');
        cy.get('#cardYield-endDate').type('2021-10-30');
        cy.get('#cardYield-submit-button').click();

        // Check loader
        cy.get('#loader')
            .should('be.visible')
            .then(() => {
                sendResponse(); // trigger reply
                cy.get('#loader').should('not.exist');
                cy.get('div[class=ag-overlay]')
                    .find('span')
                    .contains('No Rows To Show')
                    .should('exist');
            });
    });

    it('CARD YIELD - Check data renders as expected', () => {
        cy.intercept('POST', CARD_YIELD_API, {
            fixture: 'queries/card_yield_data.json',
        }).as(CARD_YIELD_POST);

        NavigateToPage('cardYield');

        cy.get('#cardYield-catalog').type('X56868');
        cy.get('#cardYield-startDate').type('2021-10-19');
        cy.get('#cardYield-endDate').type('2021-10-20');
        cy.get('#cardYield-submit-button').click();

        cy.wait(`@${CARD_YIELD_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);

                assertCellValueInFirstRow('Catalog', 'X56868');
                assertCellValueInFirstRow('CardName', 'OA_HF');
                assertCellValueInFirstRow('SuccessRatio', '100');
            });
    });

    it('STATION YIELD - Check Loader appears correctly + empty response', () => {
        let sendResponse;
        const trigger = new Promise((resolve) => {
            sendResponse = resolve;
        });

        cy.intercept('POST', STATION_YIELD_API, (req) => {
            return trigger.then(() => {
                req.reply({ fixture: 'queries/empty_query_response.json' });
            });
        });

        NavigateToPage('stationYield');
        cy.get('#stationYield-startDate').type('2021-10-15');
        cy.get('#stationYield-endDate').type('2021-10-30');
        cy.get('#stationYield-submit-button').click();

        // Check loader
        cy.get('#loader')
            .should('be.visible')
            .then(() => {
                sendResponse(); // trigger reply
                cy.get('#loader').should('not.exist');
                cy.get('div[class=ag-overlay]')
                    .find('span')
                    .contains('No Rows To Show')
                    .should('exist');
            });
    });

    it('STATION YIELD - Check query data renders as expected', () => {
        cy.intercept('POST', STATION_YIELD_API, {
            fixture: 'queries/station_yield_data.json',
        }).as(STATION_YIELD_POST);

        NavigateToPage('stationYield');

        cy.get('#stationYield-startDate').type('2021-10-19');
        cy.get('#stationYield-endDate').type('2021-10-20');
        cy.get('#stationYield-submit-button').click();

        cy.wait(`@${STATION_YIELD_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);
            });
    });

    it('STATION CARD YIELD - Check data renders as expected', () => {
        cy.intercept('POST', STATION_CARD_YIELD_API, {
            fixture: 'queries/station_card_yield_data.json',
        }).as(STATION_CARD_YIELD_POST);

        NavigateToPage('stationCardYield');

        cy.get('#stationCardYield-station').type('A4');
        cy.get('#stationCardYield-catalog').type('X56868');
        cy.get('#stationCardYield-startDate').type('2021-10-19');
        cy.get('#stationCardYield-endDate').type('2021-10-20');
        cy.get('#stationCardYield-submit-button').click();

        cy.wait(`@${STATION_CARD_YIELD_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);
            });
    });

    it('NFF - Check data renders as expected', () => {
        cy.intercept('POST', NFF_API, { fixture: 'queries/nff_data.json' }).as(
            NFF_POST
        );

        NavigateToPage('nff');

        cy.get('#nff-cardName').type('XMCP-B');
        cy.get('#nff-startDate').type('2020-11-29');
        cy.get('#nff-endDate').type('2020-11-30');
        cy.get('#nff-timeInterval').type(1);
        cy.get('#nff-submit-button').click();

        cy.wait(`@${NFF_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);
            });
    });

    it('TESTER LOAD - Check data renders as expected', () => {
        cy.intercept('POST', TESTER_LOAD_API, {
            fixture: 'queries/tester_load_data.json',
        }).as(TESTER_LOAD_POST);

        NavigateToPage('testerLoad');

        cy.get('#testerLoad-startDate').type('2021-01-01');
        cy.get('#testerLoad-endDate').type('2021-01-03');
        cy.get('#testerLoad-submit-button').click();

        cy.wait(`@${TESTER_LOAD_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);

                assertCellValueInFirstRow('Station', '1P');
                assertCellValueInFirstRow('NumberOfRuns', '2');
                assertCellValueInFirstRow('TotalRunTimeHours', '0.368333');
            });
    });

    it('CARD TEST DURATION - Check data renders as expected', () => {
        cy.intercept('POST', CARD_TEST_DURATION_API, {
            fixture: 'queries/card_test_duration_data.json',
        }).as(CARD_TEST_DURATION_POST);

        NavigateToPage('cardTestDuration');

        cy.get('#cardTestDuration-catalog').type('X93655');
        cy.get('#cardTestDuration-startDate').type('2021-11-11');
        cy.get('#cardTestDuration-endDate').type('2021-11-20');
        cy.get('#cardTestDuration-submit-button').click();

        cy.wait(`@${CARD_TEST_DURATION_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);

                assertCellValueInFirstRow('Operator', '73531');
                assertCellValueInFirstRow('NetTimeAvg', '686');
                assertCellValueInFirstRow('TotalTimeAvg', '1696');
            });
    });

    it('BOUNDARIES - Check data renders as expected', () => {
        cy.intercept('POST', BOUNDARIES_API, {
            fixture: 'queries/boundaries_data.json',
        }).as(BOUNDARIES_POST);

        NavigateToPage('boundaries');

        cy.get('#boundaries-catalog').type('X39337');
        cy.get('#boundaries-startDate').type('2021-12-22');
        cy.get('#boundaries-endDate').type('2022-02-22');
        cy.get('#boundaries-submit-button').click();

        cy.wait(`@${BOUNDARIES_POST}`)
            .its('response')
            .should('deep.include', {
                statusCode: SUCCESS_CODE,
            })
            .and('have.property', 'body')
            .then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('.ag-center-cols-container .ag-row')
                    .its('length')
                    .should('equal', body.records.length);

                cy.get('#show-graph-button').first().click();
                cy.get('#dialog-box').should('be.visible');
            });
    });
});
