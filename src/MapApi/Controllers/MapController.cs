using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MapApi.Controllers
{
    [Route("api/[controller]")]
    public class MapController : Controller
    {
        private MapService _service;

        public MapController(MapService service)
        {
            _service = service;
        }

        [HttpGet("{id}/markers")]
        public IEnumerable<MapPoint> GetMarkers(string id)
        {
            return _service.GetMapPoints(id);
        }

        [HttpGet("{id}")]
        public async Task<Map> GetMap(string id)
        {
            return await _service.GetMap(id);
        }

        [HttpPost("markers")]
        public async Task<bool> UpdateMarker([FromBody]MapPoint point)
        {
            return await _service.AddMapPoint(point);
        }

        [HttpDelete("markers")]
        public async Task<bool> RemoveMarker([FromBody]MapPoint point)
        {
            return await _service.RemoveMapPoint(point);
        }
    }
}
