using ETL.DataObjects;
using ETL.Utils;
using ETL.Utils.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
        public Result<Log> Deserialize(string data)
        {
            try 
            {
                
                var log = JsonConvert.DeserializeObject<Log>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = JsonConverters.ToArray()
                }) ;
                return new Result<Log>(true,log);
            }
            catch (Exception e) 
            {
                return new Result<Log>(false, null, e.Message);
            }

        }
    }
}
