using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterConverter
{
    public class UTM
    {
        public double UTMEasting { get; set; }
        public double UTMNorthing { get; set; }
        public string UTMZone { get; set; }

        public UTM(double east, double north, string zone = "48N")
        {
            UTMEasting = east;
            UTMNorthing = north;
            UTMZone = zone;
        }
    }

    public class LatLong : ICloneable
    {
        public LatLong()
        {

        }
        public LatLong(double lat, double log)
        {
            Latitude = lat;
            Longitude = log;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public object Clone()
        {
            return new LatLong(Latitude, Longitude);
        }
    }

    public class Station
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string Address { get; set; }
        public LatLong LatLong { get; set; }
        public List<int> Routers { get; set; }

        public Station()
        {
            Routers = new List<int>();
        }
    }

    public class Router : ICloneable
    {
        public int RouterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Stations1 { get; set; }
        public List<int> Stations2 { get; set; }

        public Router()
        {
            Stations1 = new List<int>();
            Stations2 = new List<int>();
        }

        public List<int> getStations(int turn)
        {
            if (turn == 0)
                return Stations1;
            else if (turn == 1)
                return Stations2;
            else
                return null;
        }

        public List<int> getTurn(int stationId)
        {
            List<int> listTurn = new List<int>();

            List<int> stationIds = Stations1;
            for (int i = 0; i < stationIds.Count; i++)
                if (stationIds[i] == stationId)
                    listTurn.Add(0);
            stationIds = Stations2;
            for (int i = 0; i < stationIds.Count; i++)
                if (stationIds[i] == stationId)
                    listTurn.Add(1);
            return listTurn;
        }

        public object Clone()
        {
            Router result = new Router();
            result.RouterId = RouterId;
            result.Description = Description;
            result.Stations1 = new List<int>(Stations1);
            result.Stations2 = new List<int>(Stations2);
            return result;
        }
    }

    public class StationPath
    {
        public string PathId { get; set; }
        public long Distance { get; set; }
        public int StartStation { get; set; }
        public int EndStation { get; set; }
        public List<LatLong> Path { get; set; }

        public StationPath()
        {
            Path = new List<LatLong>();
        }
    }
}
