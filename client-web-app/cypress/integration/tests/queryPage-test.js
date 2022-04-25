/// <reference types="cypress" />
import { validatePage } from '../commands/asserts';
import { NavigateToPage } from '../commands/actions';
import { 
    CARD_YIELD_POST, CARD_YIELD_API, STATION_YIELD_POST, 
    STATION_YIELD_API, STATION_CARD_YIELD_POST, 
    STATION_CARD_YIELD_API, NFF_POST, NFF_API, SUCCESS_CODE
} from '../constants/constants';

describe("QUERY PAGE TESTS", () => {
    beforeEach('Login to user admin', () => {
        cy.visit(Cypress.env('loginUrl'));
        cy.get("#login-user-email").type(Cypress.env('adminEmail'));
        cy.get("#login-admin-password").type(Cypress.env('adminPassword'));
        cy.get("#login-signIn-button").should('be.visible').and('not.be.disabled');
        cy.get("#login-signIn-button").click();
        validatePage(Cypress.env('dashboardUrl'));
    });

    it("Check pages fields renders correctly", () => {
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
        
    });

    it("CARD YIELD - Check Loader appears correctly + empty response", () => {
        let sendResponse;
        const trigger = new Promise((resolve) => {
            sendResponse = resolve;
        });

        cy.intercept('POST', CARD_YIELD_API, (req) => {
            return trigger.then(() => {
                req.reply({fixture: 'empty_query_response.json'});
            });
        });

        NavigateToPage('cardYield');
        cy.get('#cardYield-catalog').type('randomInput');
        cy.get('#cardYield-startDate').type('2021-10-15');
        cy.get('#cardYield-endDate').type('2021-10-30');
        cy.get('#cardYield-submit-button').click();

        // Check loader
        cy.get('#loader').should('be.visible')
            .then(() => {
                sendResponse(); // trigger reply
                cy.get('#loader').should('not.exist');
                cy.get('#query-table').find('div').contains('No rows').should('exist');
            })

    });

    it("CARD YIELD - Check data renders as expected", () => {
        cy.intercept('POST', CARD_YIELD_API, {fixture: 'card_yield_data.json'})
            .as(CARD_YIELD_POST);
        
        NavigateToPage('cardYield');
        
        cy.get('#cardYield-catalog').type('X56868');
        cy.get('#cardYield-startDate').type('2021-10-19');
        cy.get('#cardYield-endDate').type('2021-10-20');
        cy.get('#cardYield-submit-button').click();

        cy.wait(`@${CARD_YIELD_POST}`)
            .its('response').should('deep.include', {
                statusCode: SUCCESS_CODE
            })
            .and('have.property', 'body').then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                cy.get('#query-table').should('be.visible');
                cy.get('canvas[role="img"]').should('be.visible');
                // Check that number of rows in table equals response length
                cy.get('div[class*=virtualScrollerRenderZone]')
                    .children('[role=row]').its('length')
                    .should('equal', body.records.length);
            });
    });

    it("STATION YIELD - Check Loader appears correctly + empty response", () => {
        let sendResponse;
        const trigger = new Promise((resolve) => {
            sendResponse = resolve;
        });

        cy.intercept('POST', STATION_YIELD_API, (req) => {
            return trigger.then(() => {
                req.reply({fixture: 'empty_query_response.json'});
            });
        });

        NavigateToPage('stationYield');
        cy.get('#stationYield-startDate').type('2021-10-15');
        cy.get('#stationYield-endDate').type('2021-10-30');
        cy.get('#stationYield-submit-button').click();

        // Check loader
        cy.get('#loader').should('be.visible')
            .then(() => {
                sendResponse(); // trigger reply
                cy.get('#loader').should('not.exist');
                cy.get('#query-table').find('div').contains('No rows').should('exist');
            })
    });

    it("STATION YIELD - Check query data renders as expected", () => {
        cy.intercept('POST', STATION_YIELD_API, {fixture: 'station_yield_data.json'})
            .as(STATION_YIELD_POST);

        NavigateToPage('stationYield');
        
        cy.get('#stationYield-startDate').type('2021-10-19');
        cy.get('#stationYield-endDate').type('2021-10-20');
        cy.get('#stationYield-submit-button').click();

        cy.wait(`@${STATION_YIELD_POST}`)
            .its('response').should('deep.include', {
                statusCode: SUCCESS_CODE
            })
            .and('have.property', 'body').then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                // Check that number of rows in table equals response length
                cy.get('#query-table').should('be.visible');
                cy.get('div[class*=virtualScrollerRenderZone]')
                    .children('[role=row]').its('length')
                    .should('equal', body.records.length);
            });
    });

    it("STATION CARD YIELD - Check data renders as expected", () => {
        cy.intercept('POST', STATION_CARD_YIELD_API, {fixture: 'station_card_yield_data.json'})
            .as(STATION_CARD_YIELD_POST);
        
        NavigateToPage('stationCardYield');
        
        cy.get('#stationCardYield-station').type('A4');
        cy.get('#stationCardYield-catalog').type('X56868');
        cy.get('#stationCardYield-startDate').type('2021-10-19');
        cy.get('#stationCardYield-endDate').type('2021-10-20');
        cy.get('#stationCardYield-submit-button').click();

        cy.wait(`@${STATION_CARD_YIELD_POST}`)
            .its('response').should('deep.include', {
                statusCode: SUCCESS_CODE
            })
            .and('have.property', 'body').then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                cy.get('#query-table').should('be.visible');
                // Check that number of rows in table equals response length
                cy.get('div[class*=virtualScrollerRenderZone]')
                    .children('[role=row]').its('length')
                    .should('equal', body.records.length);
            });
    });

    it("NFF - Check data renders as expected", () => {
        cy.intercept('POST', NFF_API, {fixture: 'nff_data.json'})
            .as(NFF_POST);
        
        NavigateToPage('nff');
        
        cy.get('#nff-cardName').type('XMCP-B');
        cy.get('#nff-startDate').type('2020-11-29');
        cy.get('#nff-endDate').type('2020-11-30');
        cy.get('#nff-timeInterval').type(1);
        cy.get('#nff-submit-button').click();

        cy.wait(`@${NFF_POST}`)
            .its('response').should('deep.include', {
                statusCode: SUCCESS_CODE
            })
            .and('have.property', 'body').then((body) => {
                expect(body).to.have.keys('columnNames', 'records');
                expect(body.columnNames).to.be.an('array');
                expect(body.records).to.be.an('array');
                cy.get('#query-table').should('be.visible');
                // Check that number of rows in table equals response length
                cy.get('div[class*=virtualScrollerRenderZone]')
                    .children('[role=row]').its('length')
                    .should('equal', body.records.length);
            });
    });
})