using ETL.DataObjects;
using ETL.Utils;
using ETL.Utils.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ETL
{

    public class Deserializer
    {
        List<JsonConverter> JsonConverters { get; }
        private readonly ILogger _logger;
        public Deserializer(ILogger logger)
        {
            JsonConverters = new List<JsonConverter>() { new DatetimeJsonConverter() };
            _logger = logger;
        }
        /// <summary>
        /// Deserializing string from json format to populate an Log instance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Result<Log> Deserialize(string data)
        {
            try
            {

                var log = JsonConvert.DeserializeObject<Log>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = JsonConverters.ToArray()
                });
                if (!ValidateLog(log))
                {
                    return new Result<Log>(false, null, "Error in log validation");

                }
                return new Result<Log>(true, log);
            }
            catch (Exception e)
            {
                _logger.Warning($"Error raise in deserializetion", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Log>(false, null, e.Message);
            }

        }

        private bool ValidateLog(Log log)
        {
            //Verify Nettime is in bounds
            if (log.NetTime.TotalDays >= 1)
            {
                return false;
            }
            return true;
        }
    }
}
