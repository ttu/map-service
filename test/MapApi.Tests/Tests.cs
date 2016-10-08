using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Xunit;

namespace MapApi.Tests
{
    public class MapServiceTests
    {
        // Marten doesn't have in-memory database, so have to use own docker container for testing

        private readonly string _conStr = "Server=127.0.0.1;Port=5433;Database=map-db-ut;User Id=postgres;Password=pwd;";
        private readonly IDocumentStore _store;

        public MapServiceTests()
        {
            _store = MartenHelper.GetStore(_conStr);
            MartenHelper.Clean(_store);
        }

        [Fact]
        public async Task HubUpdated_Message()
        {
            var are = new AutoResetEvent(false);

            bool result = false;

            var hub = new MessageHub();

            hub.Updated += (e, d) =>
            {
                result = true;
                are.Set();
            };

            var service = new MapService(hub, _store);

            await service.AddMapPoint(new MapPoint());
            are.WaitOne();

            Assert.True(result);
        }

        [Fact]
        public async Task Get_Remove_MapPoints()
        {
            var mapId = "ABBA";

            var hub = NSubstitute.Substitute.For<MessageHub>();
            var service = new MapService(hub, _store);

            var point = new MapPoint { MapId = mapId, Lat = 2, Lng = 4 };
            await service.AddMapPoint(point);

            var result = service.GetMapPoints(mapId);
            Assert.Equal(1, result.Count());

            await service.RemoveMapPoint(point);
            Assert.Equal(0, result.Count());       
        }

        [Fact]
        public async Task Replace_MapPoints()
        {
            var mapId = "ABBA";

            var hub = NSubstitute.Substitute.For<MessageHub>();
            var service = new MapService(hub, _store);

            var point = new MapPoint { MapId = mapId, Lat = 2, Lng = 4 };
            await service.AddMapPoint(point);

            var result = service.GetMapPoints(mapId);
            Assert.Equal(1, result.Count());

            var points = new List<MapPoint>{
                new MapPoint { MapId = mapId, Lat = 2, Lng = 4 },
                new MapPoint { MapId = mapId, Lat = 2, Lng = 4 }
            };

            await service.ReplacePoints(mapId, points);

            result = service.GetMapPoints(mapId);
            Assert.Equal(2, result.Count());       
        }
    }
}
