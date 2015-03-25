using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterEditor
{
    public class StartMark : GMarkerGoogle
    {
        public StartMark(double Lat, double Long)
            :base(new PointLatLng(Lat, Long), GMarkerGoogleType.green_big_go)
        {
        }
        public int StationId { get; set; }
    }
}
