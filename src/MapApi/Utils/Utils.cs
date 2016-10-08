using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapApi
{
    public static class Utils
    {
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value,
                                    new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
        }
    }
}