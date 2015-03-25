using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterEditor
{
    public class StationMaker : GMarkerGoogle
    {
        public StationMaker(double Lat, double Long)
            :base(new PointLatLng(Lat, Long), GMarkerGoogleType.blue_small)
        {
        }
        public int RouterId { get; set; }
        public int StationId { get; set; }
        public int Turn { get; set; }
    }
}
