using BusRouterConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusFindingDemo
{
    public class BusManager
    {
        private Dictionary<int, Station> _listStation = new Dictionary<int, Station>();
        private Dictionary<int, Router> _listRouter = new Dictionary<int, Router>();
        private Dictionary<string, StationPath> _listPath = new Dictionary<string, StationPath>();

        private static BusManager _instance = new BusManager();

        public static BusManager Inst()
        {
            return _instance;
        }

        public Dictionary<int, Router> Routers { get { return _listRouter; }}
        public Dictionary<int, Station> Stations { get { return _listStation; }}

        public Dictionary<string, StationPath> StationPaths
        {
            get
            {
                return _listPath;
            }
        }

        public StationPath getStationPath(string pathId)
        {
            if (!_listPath.ContainsKey(pathId))
                return null;
            return _listPath[pathId];
        }

        public void loadBusData()
        {
            string path = @"E:\Projects\BusMap\BusRouterConverter\output\";

            string routerString = File.ReadAllText(path + "routers.json");
            string stationString = File.ReadAllText(path + "stations.json");
            string stationPathString = File.ReadAllText(path + "stationPath.json");

            List<Router> list1 = JsonConvert.DeserializeObject<List<Router>>(routerString);
            List<Station> list2 = JsonConvert.DeserializeObject<List<Station>>(stationString);
            List<StationPath> list3 = JsonConvert.DeserializeObject<List<StationPath>>(stationPathString);

            for (int i = 0; i < list1.Count; i++)
            {
                Router router = list1[i];
                _listRouter.Add(router.RouterId, router);
            }
            for (int i = 0; i < list2.Count; i++)
            {
                Station station = list2[i];
                _listStation.Add(station.StationId, station);
            }
            for (int i = 0; i < list3.Count; i++)
            {
                StationPath stationPath = list3[i];
                if (!_listPath.ContainsKey(stationPath.PathId))
                    _listPath.Add(stationPath.PathId, stationPath);
            }
        }

    }
}
