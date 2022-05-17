using RestSharp;
using System;
using System.Collections.Generic;
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

        #region Queries Scenarios

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
        public async Task<IRestResponse> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CalculateBoundaries(catalog, startDate, endDate) : defaultResponse;
            return response;
        }

        #endregion

        #region Alarms Scenarios

        public async Task<IRestResponse> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.AddNewAlarm(name, field, objective, threshold, receivers) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> EditAlarm(int Id, string name, Field field, string objective, int threshold, bool active, List<string> receivers)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.EditAlarm(Id, name, field, objective, threshold, active, receivers) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> RemoveAlarm(int Id)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.RemoveAlarm(Id) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> CheckAlarmsCondition()
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.CheckAlarmsCondition() : defaultResponse;
            return response;
        }
        #endregion

        #region Authentication Scenarios
        public async Task<IRestResponse> RequestVerificationCode(string email)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.RequestVerificationCode(email) : defaultResponse;
            return response;

        }

        public async Task<IRestResponse> Login(string email, string password)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.Login(email, password) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> AddUser(string email)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.AddUser(email) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> RemoveUser(string email)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.RemoveUser(email) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> ChangeSystemAdminPassword(string email, string newPassword, string oldPassword)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.ChangeSystemAdminPassword(email, newPassword, oldPassword) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> AddSystemAdmin(string email)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.AddSystemAdmin(email) : defaultResponse;
            return response;
        }

        public async Task<IRestResponse> ForgetPassword(string email)
        {
            var defaultResponse = new RestResponse() { StatusCode = HttpStatusCode.OK };
            var response = RealClient != null ? await RealClient.ForgetPassword(email) : defaultResponse;
            return response;
        }

        public async Task Authenticate()
        {
            if (RealClient != null)
            {
                await RealClient.Authenticate();
            }
        }

        #endregion
    }
}
