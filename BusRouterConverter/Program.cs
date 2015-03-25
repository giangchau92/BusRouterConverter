using GMap.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusRouterConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] filepaths = null;
            try
            {
                filepaths = Directory.GetFiles(@"input\");

            }
            catch (IOException ex)
            {
                System.Console.WriteLine("Exceiption: " + ex.Message);
                return;
            }

            List<Station> listStation = new List<Station>();
            List<Router> listRouter = new List<Router>();
            List<StationPath> listPath = new List<StationPath>();

            for (int i = 0; i < filepaths.Length; i++)
            {
                string path = filepaths[i];
                string json = File.ReadAllText(path);

                //
                char[] keys = { '\\', '-', '.' };
                int routerid = Int32.Parse(path.Split(keys)[1]);
                int turn = Int32.Parse(path.Split(keys)[2]);

                Router router = listRouter.Find(x => x.RouterId == routerid);

                if (router == null)
                {
                    router = new Router();
                    router.RouterId = routerid;
                    listRouter.Add(router);
                }

                Dictionary<string, object> obj = deserializeObject(json);
                JArray array = (JArray)obj["TABLE"];
                JToken rows = (JToken)array[0];
                JArray array2 = (JArray)rows["ROW"];
                Station preStation = null;
                foreach (JToken stationdata in array2)
                {
                    JArray arraydata = (JArray)stationdata["COL"];
                    string address = parseJsonData(arraydata[12]).ToString();
                    double Lat = Double.Parse(parseJsonData(arraydata[9]).ToString());
                    double Lng = Double.Parse(parseJsonData(arraydata[8]).ToString());
                    int stationId = Int32.Parse(parseJsonData(arraydata[1]).ToString());
                    string stationName = parseJsonData(arraydata[7]).ToString();
                    string stationCode = parseJsonData(arraydata[13]).ToString();
                    int routerId = Int32.Parse(parseJsonData(arraydata[0]).ToString());
                    string routerName = parseJsonData(arraydata[10]).ToString();
                    string routerDes = parseJsonData(arraydata[11]).ToString();

                    Station station = null;

                    if (listStation.Contains(station))
                    {
                        station = listStation[stationId];
                        station.Routers.Add(routerid);
                    }
                    else
                    {
                        station = new Station();
                        station.Address = address;
                        station.LatLong = new LatLong(Lat, Lng);
                        station.StationId = stationId;
                        station.StationName = stationName;
                        station.StationCode = stationCode;

                        if (!station.Routers.Contains(routerid))
                            station.Routers.Add(routerid);

                        listStation.Add(station);
                        router.Description = routerDes;
                    }

                    if (turn == 1)
                    {
                        if (!router.Stations1.Contains(stationId))
                            router.Stations1.Add(stationId);
                    }
                    else
                    {
                        if (!router.Stations2.Contains(stationId))
                            router.Stations2.Add(stationId);
                    }

                    if (preStation == null)
                        preStation = station;
                    else
                    {
                        StationPath stationPath = new StationPath();
                        stationPath.PathId = preStation.StationId + "_" + station.StationId;
                        //
                        PointLatLng start = new PointLatLng(preStation.LatLong.Latitude, preStation.LatLong.Longitude);
                        PointLatLng end = new PointLatLng(station.LatLong.Latitude, station.LatLong.Longitude);
                        MapRoute mapRouter = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRouteBetweenPoints(start, end, true, false, 50);
                        long distance = (long)(mapRouter.Distance * 1000); // as meter
                        for (int j = 0; j < mapRouter.Points.Count; j++)
                        {
                            PointLatLng point = mapRouter.Points[j];
                            LatLong latlong = new LatLong();
                            latlong.Latitude = point.Lat;
                            latlong.Longitude = point.Lng;
                            stationPath.Path.Add(latlong);

                        }
                        stationPath.StartStation = preStation.StationId;
                        stationPath.EndStation = station.StationId;
                        stationPath.Distance = distance;
                        listPath.Add(stationPath);
                        preStation = station;
                    }

                }
            }

            string outurl = @"E:\Projects\BusMap\BusRouterConverter\output\";
            string output = JsonConvert.SerializeObject(listStation);
            File.WriteAllText(outurl + "stations.json", output);
            

            output = JsonConvert.SerializeObject(listRouter);
            File.WriteAllText(outurl + "routers.json", output);

            output = JsonConvert.SerializeObject(listPath);
            File.WriteAllText(outurl + "stationPath.json", output);
            
        }


        private static Dictionary<string, object> deserializeObject(object json)
        {
            Dictionary<string, object> result = null;
            if (json is string)
            {
                result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json as string);
            }
            return result;
        }

        private static JValue parseJsonData(JToken json)
        {
            return (JValue)json["DATA"];
        }
        
    }
}
