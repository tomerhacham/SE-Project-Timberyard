using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using TimberyardClient.Client;

namespace AcceptanceTests.Utils
{
    /// <summary>
    /// Proxy for the the timeryard client. The proxy will mock the REST api responses
    /// just for letting the acceptence tests to be written and compile wihout errors.
    /// 
    /// After the development will finish the 'Real' instance which make the REST api calls will be
    /// deligated and return the actual values
    /// </summary>
    public class TimberyardClientProxy : ITimberyardClient
    {
        //this is acctualy the Adapter class, but the proxy does not need to know that because all
        //of them implement the same interface
        public ITimberyardClient RealClient { get; set; }


        public async Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateCardYield(catalog, startDate, endDate) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateNoFailureFound(cardName, startDate, endDate, timeInterval) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateStationAndCardYield(station, catalog, startDate, endDate) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateStationsYield(startDate, endDate) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateTesterLoad(startDate, endDate) : defaultResponse;
            return response;
        }
        public async Task<IRestResponse> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateCardTestDuration(catalog, startDate, endDate) : defaultResponse;
            return response;
        }
    }
}
