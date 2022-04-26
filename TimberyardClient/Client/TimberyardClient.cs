using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimberyardClient.Client
{
    public interface ITimberyardClient
    {
        public Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateStationsYield(DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval);
        public Task<IRestResponse> CalculateTesterLoad(DateTime startDate, DateTime endDate);
    }
    public class TimberyardClient : ITimberyardClient
    {
        #region End-Points
        //Queries
        private readonly string CARD_YIELD_ENDPOINT = "/api/Queries/CardYield";
        private readonly string STATION_YIELD_ENDPOINT = "/api/Queries/StationsYield";
        private readonly string STATION_AND_CARD_YIELD_ENDPOINT = "/api/Queries/StationAndCardYield";
        private readonly string NFF_ENDPOINT = "/api/Queries/NFF";
        private readonly string TESTER_LOAD_ENDPOINT = "/api/Queries/TesterLoad";
        #endregion

        RestClient RestClient { get; }
        public UserCredentials UserCredentials { get; set; }

        public TimberyardClient(IOptions<UserCredentials> userCredentials, IOptions<ServiceSettings> serviceSettings)
        {
            UserCredentials = userCredentials.Value;
            RestClient = new RestClient(serviceSettings.Value.Url);
            RestClient.AddDefaultHeaders(new Dictionary<string, string>()
            {
                { "Content-Type","application/json" },
                { "accept","application/json" }
            });
        }

        public async Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(CARD_YIELD_ENDPOINT, Method.POST);
            var body = new { Catalog = catalog, StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }
        public async Task<IRestResponse> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(STATION_YIELD_ENDPOINT, Method.POST);
            var body = new { StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }
        public async Task<IRestResponse> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(STATION_AND_CARD_YIELD_ENDPOINT, Method.POST);
            var body = new { Station = station, Catalog = catalog, StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }
        public async Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            var request = new RestRequest(STATION_AND_CARD_YIELD_ENDPOINT, Method.POST);
            var body = new { CardName = cardName, StartDate = startDate, EndDate = endDate, TimeInterval = timeInterval };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }
        public async Task<IRestResponse> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(TESTER_LOAD_ENDPOINT, Method.POST);
            var body = new { StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        /// <summary>
        /// Utility function to wrap the request sending process and add the JWT data
        /// </summary>
        /// <param name="request">The RestRequest object</param>
        /// <returns></returns>
        private async Task<IRestResponse> ExecuteWrapperAsync(IRestRequest request)
        {
            //TODO: add JWT data whem implemented
            //E.g request.AddOrUpdateHeader("Authorization", $"Bearer {JWTToken}");

            var response = await RestClient.ExecuteAsync(request);
            return response;

        }

    }
}
