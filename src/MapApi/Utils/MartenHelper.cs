using System.Collections.Generic;
using Marten;

namespace MapApi
{
    public static class MartenHelper
    {
        public static DocumentStore GetStore(string connectionString)
        {
            return DocumentStore.For(connectionString);
        }

        public static void Init(IDocumentStore store)
        {
            using (var session = store.LightweightSession())
            {
                var mapOpenStreetMap = new Map
                {
                    Id = "e5c4d",
                    Lat = 60.169,
                    Lng = 24.939,
                    Elevation = 15,
                    MaxZoom = 18,
                    Url = "http://{s}.tile.osm.org/{z}/{x}/{y}.png",
                    Attribution = "&copy; <a href='http://osm.org/copyright'>OpenStreetMap</a> contributors"
                };

                var points = new List<MapPoint> {
                    new MapPoint { MapId = mapOpenStreetMap.Id, Lat = 60.169, Lng = 24.939, Description = "Hello" },
                    new MapPoint { MapId = mapOpenStreetMap.Id, Lat = 60.171, Lng = 24.942, Description = "Hello 2" },
                    new MapPoint { MapId = mapOpenStreetMap.Id, Lat = 60.162, Lng = 24.942, Description = "Hello 3" }
                };

                session.Store(mapOpenStreetMap);
                session.Store(points.ToArray());

                var mapBoxV10 = new Map
                {
                    Id = "4fGe9",
                    Lat = 60.169,
                    Lng = 24.939,
                    Elevation = 15,
                    MaxZoom = 18,
                    Url = "https://api.mapbox.com/styles/v1/mapbox/streets-v10/tiles/256/{z}/{x}/{y}?access_token={access_token}",
                    AccessToken = "TOKEN HERE"
                };

                session.Store(mapBoxV10);
                session.SaveChanges();
            }
        }

        public static void Clean(IDocumentStore store)
        {
            // http://jasperfx.github.io/marten/documentation/documents/advanced/cleaning/           
            // Tear down and remove all Marten related database schema objects
            store.Advanced.Clean.CompletelyRemoveAll();
        }
    }
}
