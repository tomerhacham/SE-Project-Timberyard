using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Queries;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;

namespace WebService.Domain.Interface
{
    public class SystemFacade
    {
        QueriesController QueriesController { get; }
        AlarmsController AlarmsController { get; }
        public SystemFacade(QueriesController queriesController, AlarmsController alarmsController)
        {
            QueriesController = queriesController;
            AlarmsController = alarmsController;
        }

        #region Queries
        public Task<Result<QueryResult>> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateBoundaries(catalog, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateCardYield(catalog, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateStationsYield(startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateStationAndCardYield(station, catalog, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateNFF(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            return QueriesController.CalculateNFF(cardName, startDate, endDate, timeInterval);
        }
        public Task<Result<QueryResult>> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateTesterLoad(startDate, endDate);
        } 
        #endregion
    
        public Task<Result<AlarmDTO>> AddNewAlarm(string name, Field field, int threshold, List<string> receivers)
        {
            return AlarmsController.AddNewAlarm(name, field, threshold, receivers);
        }

    }
}
