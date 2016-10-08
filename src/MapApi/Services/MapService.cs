using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MapApi
{
    public class MapService
    {
        private readonly MessageHub _hub;
        private readonly IDocumentStore _store;

        public MapService(MessageHub hub, IDocumentStore store)
        {
            _hub = hub;
            _store = store;
        }

        public IEnumerable<MapPoint> GetMapPoints(string mapId)
        {
            using (var session = _store.QuerySession())
            {
                return session
                    .Query<MapPoint>()
                    .Where(m => m.MapId == mapId);
            }
        }

        public async Task<Map> GetMap(string id)
        {
            using (var session = _store.QuerySession())
            {
                return await session
                    .Query<Map>()
                    .SingleOrDefaultAsync(m => m.Id == id);
            }
        }

        public async Task<bool> AddMapPoint(MapPoint point)
        {
            using (var session = _store.LightweightSession())
            {
                session.Store(point);
                await session.SaveChangesAsync();
            }

            _hub.DataUpdated(point.MapId, Utils.Serialize(GetMapPoints(point.MapId)));
            return true;
        }

        public async Task<bool> RemoveMapPoint(MapPoint point)
        {
            using (var session = _store.LightweightSession())
            {
                session.DeleteWhere<MapPoint>(x => x.MapId == point.MapId && x.Lat == point.Lat && x.Lng == point.Lng);
                await session.SaveChangesAsync();
            }

            _hub.DataUpdated(point.MapId, Utils.Serialize(GetMapPoints(point.MapId)));
            return true;
        }

        public async Task<bool> ReplacePoints(string mapId, IEnumerable<MapPoint> points)
        {
            using (var session = _store.LightweightSession())
            {
                session.DeleteWhere<MapPoint>(x => x.MapId == mapId);
                await session.SaveChangesAsync();
                
                session.Store(points.ToArray());
                await session.SaveChangesAsync();
            }

            _hub.DataUpdated(mapId, JsonConvert.SerializeObject(points,
                               new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            return true;
        }
    }
}