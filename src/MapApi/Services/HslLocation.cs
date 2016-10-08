using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapApi
{
    public class HslLocation
    {
        private MapService _service;

        public HslLocation(MapService service)
        {
            _service = service;
        }

        public string MapId => "4fGe9";

        public string LastData { get; private set; }

        public async Task Update()
        {
            LastData = await GetLocations();
            var points = JsonConvert.DeserializeObject<IEnumerable<MapPoint>>(LastData);
            await _service.ReplacePoints(this.MapId, points);
        }

        private async Task<string> GetLocations()
        {
            using (var client = new HttpClient())
            {
                var jsonData = await client.GetStringAsync("http://dev.hsl.fi/siriaccess/vm/json?operatorRef=HSL");

                var locations = JObject.Parse(jsonData)["Siri"]["ServiceDelivery"]["VehicleMonitoringDelivery"]
                        .SelectMany(s => s["VehicleActivity"])
                        .Select(s => s["MonitoredVehicleJourney"])
                        .Where(s => s["VehicleLocation"]["Latitude"] != null)
                        .Select(s => new
                        {
                            lng = s["VehicleLocation"]["Longitude"],
                            lat = s["VehicleLocation"]["Latitude"],
                            description = s["LineRef"]["value"]
                        });

                return JsonConvert.SerializeObject(locations);
            }
        }
    }
}