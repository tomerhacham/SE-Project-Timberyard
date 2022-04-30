using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimberyardClient.Client;

namespace AcceptanceTests.Utils
{
    /// <summary>
    /// Adapter class for future use in case responses will change.
    /// Currently the class simply deligate the request to the RealClient
    /// </summary>
    public class TimberyardClientRealAdapter : InitializeSystem, ITimberyardClient
    {
        public ITimberyardClient RealClient { get; set; }

        public TimberyardClientRealAdapter()
        {
            RealClient = GetConfiguratedClient();
        }

        #region Queries Scenarios

        public Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateCardYield(catalog, startDate, endDate);
        }

        public Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            return RealClient.CalculateNoFailureFound(cardName, startDate, endDate, timeInterval);
        }

        public Task<IRestResponse> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateStationAndCardYield(station, catalog, startDate, endDate);
        }

        public Task<IRestResponse> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateStationsYield(startDate, endDate);
        }

        public Task<IRestResponse> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateTesterLoad(startDate, endDate);
        }

        public Task<IRestResponse> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateCardTestDuration(catalog, startDate, endDate);
        }

        public Task<IRestResponse> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateBoundaries(catalog, startDate, endDate);
        }

        #endregion

        #region Alarms Scenarios

        public Task<IRestResponse> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            return RealClient.AddNewAlarm(name, field, objective, threshold, receivers);
        }
        public Task<IRestResponse> EditAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            return RealClient.EditAlarm(name, field, objective, threshold, receivers);
        }
        public Task<IRestResponse> RemoveAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            return RealClient.RemoveAlarm(name, field, objective, threshold, receivers);
        }

        public Task<IRestResponse> CheckAlarmsCondition()
        {
            return RealClient.CheckAlarmsCondition();
        }

        #endregion
    }
}
