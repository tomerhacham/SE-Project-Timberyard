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
        public Deserializer()
        {
            JsonConverters = new List<JsonConverter>() { new DatetimeJsonConverter() };
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
