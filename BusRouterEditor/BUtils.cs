using BusRouterConverter;
using GMap.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusRouterEditor
{
    public class BUtils
    {
        public static bool isNotNull(object o)
        {
            return o != null;
        }

        public static bool isValidTurn(int turn)
        {
            return turn == 1 || turn == 0;
        }

        public static string generatePathId(int pre, int sta)
        {
            return String.Format("{0}_{1}", pre, sta);
        }

        public static void loadFormFiles(String[] filenames, Dictionary<int, Router> listRouter, Dictionary<int, Station> listStation,
            Dictionary<string, StationPath> listPath)
        {
            for (int i = 0; i < filenames.Length; i++)
            {
                string path = filenames[i];
                string json = File.ReadAllText(path);

                //
                char[] keys = { '\\', '-', '.' };
                string[] parts = path.Split(keys);
                int routerid = Int32.Parse(parts[parts.Length - 3]);
                int turn = Int32.Parse(parts[parts.Length - 2]);

                Router router = null;

                if (!listRouter.ContainsKey(routerid))
                {
                    router = new Router();
                    router.RouterId = routerid;
                    listRouter.Add(routerid, router);
                }
                else
                {
                    router = listRouter[routerid];
                    List<int> list = null;
                    if (turn == 1)
                        list = router.Stations1;
                    else
                        list = router.Stations2;
                    if (list.Count != 0)
                    {
                        DialogResult result = MessageBox.Show("Router: " + routerid + "_" + turn + " is exist. Please delete it then try again", "Warning", MessageBoxButtons.OK);
                        continue;
                    }
                    
                }

                Dictionary<string, object> obj = deserializeObject(json);
                JArray array = (JArray)obj["TABLE"];
                JToken rows = (JToken)array[0];
                JArray array2 = (JArray)rows["ROW"];
                Station preStation  = null;
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

                    router.Description = routerDes;
                    router.Name = routerName;
                    Station station = null;

                    if (listStation.ContainsKey(stationId))
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

                        listStation.Add(stationId, station);
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
                        MapRoute mapRouter = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRoute(start, end, true, false, 50);
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
                        if (!listPath.ContainsKey(stationPath.PathId))
                            listPath.Add(stationPath.PathId, stationPath);
                        preStation = station;
                    }

                }
            }
        }

        static List<KnownColor> listColor = new List<KnownColor>();

        public static Color randomColor()
        {
            if (listColor.Count == 0)
            {
                
                KnownColor[] names = new KnownColor[] { KnownColor.Aqua, KnownColor.BlueViolet, KnownColor.Chartreuse,
                    KnownColor.Coral, KnownColor.CornflowerBlue, KnownColor.DarkGreen, KnownColor.Firebrick,
                    KnownColor.Gold, KnownColor.Indigo, KnownColor.HotPink, KnownColor.LawnGreen, KnownColor.LightSalmon,
                    KnownColor.LightSeaGreen, KnownColor.Maroon};
                listColor = new List<KnownColor>(names);
            }
            Random randomGen = new Random(new System.DateTime().Millisecond);
            //KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));


            KnownColor randomColorName = listColor[randomGen.Next(listColor.Count)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            listColor.Remove(randomColorName);
            return randomColor;
        }

        private static JValue parseJsonData(JToken json)
        {
            return (JValue)json["DATA"];
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
    }
}
