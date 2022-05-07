using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimberyardClient.Client;

namespace TimberyardClient.Client
{
    public enum Field
    {
        Catalog,
        Station
    }

    public interface ITimberyardClient
    {
        #region Queries Scenarios

        public Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateStationsYield(DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval);
        public Task<IRestResponse> CalculateTesterLoad(DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate);
        public Task<IRestResponse> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate);

        #endregion

        #region Alarms Scenarios

        public Task<IRestResponse> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers);
        public Task<IRestResponse> EditAlarm(int id, string name, Field field, string objective, int threshold, bool active, List<string> receivers);
        public Task<IRestResponse> RemoveAlarm(int id);
        public Task<IRestResponse> CheckAlarmsCondition();

        #endregion

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
        private readonly string CARD_TEST_DURATION_ENDPOINT = "/api/Queries/CardTestDuration";
        private readonly string BOUNDARIES_DURATION_ENDPOINT = "/api/Queries/Boundaries";

        //Alarms
        private readonly string ADD_NEW_ALARM_ENDPOINT = "/api/Alarms/AddNewAlarm";
        private readonly string EDIT_ALARM_ENDPOINT = "/api/Alarms/EditAlarm";
        private readonly string REMOVE_ALARM_ENDPOINT = "/api/Alarms/RemoveAlarm";
        private readonly string CHECK_ALARM_CONDITION_ENDPOINT = "/api/Alarms/CheckAlarmsCondition";
        #endregion

        RestClient RestClient { get; }
        public UserCredentials UserCredentials { get; set; }

        public TimberyardClient(IOptions<UserCredentials> userCredentials, IOptions<ServiceSettings> serviceSettings)
        {
            UserCredentials = userCredentials.Value;
            RestClient = new RestClient(serviceSettings.Value.Url);
            RestClient.UseSerializer(() => new JsonNetSerializer());
            RestClient.AddDefaultHeaders(new Dictionary<string, string>()
            {
                { "Content-Type","application/json" },
                { "accept","application/json" }
            });
        }

        #region Queries Scenarios

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
            var request = new RestRequest(NFF_ENDPOINT, Method.POST);
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

        public async Task<IRestResponse> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(CARD_TEST_DURATION_ENDPOINT, Method.POST);
            var body = new { Catalog = catalog, StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        public async Task<IRestResponse> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            var request = new RestRequest(BOUNDARIES_DURATION_ENDPOINT, Method.POST);
            var body = new { Catalog = catalog, StartDate = startDate, EndDate = endDate };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        #endregion


        #region Alarms Scenarios

        public async Task<IRestResponse> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var request = new RestRequest(ADD_NEW_ALARM_ENDPOINT, Method.POST);
            var body = new { Name = name, Field = field, Objective = objective, Threshold = threshold, Receivers = receivers };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        public async Task<IRestResponse> EditAlarm(int id, string name, Field field, string objective, int threshold, bool active, List<string> receivers)
        {
            var request = new RestRequest(EDIT_ALARM_ENDPOINT, Method.POST);
            var body = new { Id = id, Name = name, Field = field, Objective = objective, Threshold = threshold, Active = active, Receivers = receivers };
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        public async Task<IRestResponse> RemoveAlarm(int id)
        {
            var request = new RestRequest(REMOVE_ALARM_ENDPOINT, Method.POST);
            var body = id;
            request.AddJsonBody(body);
            return await ExecuteWrapperAsync(request);
        }

        public async Task<IRestResponse> CheckAlarmsCondition()
        {
            var request = new RestRequest(CHECK_ALARM_CONDITION_ENDPOINT, Method.POST);
            return await ExecuteWrapperAsync(request);
        }

        #endregion

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
