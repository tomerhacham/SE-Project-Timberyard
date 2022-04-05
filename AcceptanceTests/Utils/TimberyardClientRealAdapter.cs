using AcceptanceTests.Client;
using RestSharp;
using System;
using System.Threading.Tasks;

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

        public Task<IRestResponse> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateCardYield(catalog, startDate, endDate);
        }

        public Task<IRestResponse> CalculateNoFailureFound(string cardName, DateTime startDate, DateTime endDate)
        {
            return RealClient.CalculateNoFailureFound(cardName, startDate, endDate);
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
    }
}
