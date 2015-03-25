using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterEditor
{
    public class EndMarker : GMarkerGoogle
    {
        public EndMarker(double Lat, double Long)
            :base(new PointLatLng(Lat, Long), GMarkerGoogleType.pink)
        {
        }
        public int StationId { get; set; }
    }
}
