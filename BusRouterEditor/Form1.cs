using BusRouterConverter;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusRouterEditor
{
    public partial class Form1 : Form
    {
        private Dictionary<int, Station> _listStation = new Dictionary<int, Station>();
        private Dictionary<int, Router> _listRouter = new Dictionary<int, Router>();
        private Dictionary<string, StationPath> _listPath = new Dictionary<string, StationPath>();

        private GMapOverlay markersOverlay;
        private GMapOverlay routerOverlay;

        private int _currentRouter = 0;
        private Keys _currentKeyDown = Keys.None;
        private StationMaker currentMarker = null;

        private List<int> _drawedStation = new List<int>();

        public Form1()
        {
            InitializeComponent();
        }

        #region EVENT_ZONE
        private void Form1_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.GoogleMap;
            gmap.Position = new PointLatLng(54.6961334816182, 25.2985095977783);
            gmap.MinZoom = 0;
            gmap.MaxZoom = 24;
            gmap.Zoom = 9;

            markersOverlay = new GMapOverlay("markers");
            routerOverlay = new GMapOverlay("routers");

            gmap.Overlays.Add(routerOverlay);
            gmap.Overlays.Add(markersOverlay);
            gmap.OnMarkerClick += gmap_OnMarkerClick;
            gmap.MouseMove += gmap_MouseMove;
            gmap.MouseDown += gmap_MouseDown;
            gmap.MouseUp += gmap_MouseUp;
            gmap.KeyDown += Form1_KeyDown;
            gmap.KeyUp += gmap_KeyUp;

            // init data
            loadBusData();
            initUI();
        }

        void gmap_KeyUp(object sender, KeyEventArgs e)
        {
            //_currentKeyDown = Keys.None;
        }

        void gmap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right || currentMarker == null)
                return;
            if (_currentKeyDown == Keys.S)
            {
                updateWhenChangeStation(currentMarker.StationId, currentMarker.Position, currentMarker.Turn);
            }
            
        }

        void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                return;
        }

        void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                return;
            if (currentMarker != null && _currentKeyDown == Keys.S)
            {
                PointLatLng p = gmap.FromLocalToLatLng(e.X, e.Y);
                currentMarker.Position = p;
            }
        }

        void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
           if (_currentKeyDown == Keys.A)
                currentMarker = item as StationMaker;
        }


        private void listRouter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear
            clearMap();

            if (listRouter.SelectedIndex == -1)
                return;
            int routerId = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[0]);
            int turn = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[1]);
            _currentRouter = routerId;
            Router router = _listRouter[routerId];
            if (router == null)
            {
                MessageBox.Show("Router not found!");
                return;
            }
            tbRouterId.Text = router.Name;
            drawRouter(routerId, turn);
            gmap.Position = new PointLatLng(10.8012494, 106.6783981);
            gmap.Zoom = 13f;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           _currentKeyDown = e.KeyCode;
           if (e.KeyCode == Keys.Escape)
               _currentKeyDown = Keys.None;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    standeData();
                    string outurl = @"E:\Projects\BusMap\BusRouterConverter\output\";
                    string output = JsonConvert.SerializeObject(convertDicToList(_listStation));
                    File.WriteAllText(outurl + "stations.json", output);


                    output = JsonConvert.SerializeObject(convertDicToList(_listRouter));
                    File.WriteAllText(outurl + "routers.json", output);

                    output = JsonConvert.SerializeObject(convertDicToList(_listPath));
                    File.WriteAllText(outurl + "stationPath.json", output);

                    MessageBox.Show("Successful");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /**
         * Show all router
         */
        private void btnShowAll_Click(object sender, EventArgs e)
        {
            listRouter.SelectedIndex = -1;
            clearMap();
            foreach (KeyValuePair<int, Router> item in _listRouter)
            {
                Router router = item.Value;
                drawRouter(router.RouterId, 0);//
            }
            gmap.Position = new PointLatLng(10.8012494, 106.6783981);
            gmap.Zoom = 13f;
        }

        /**
         * Import bus router file
         */
        private void btnImportClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Json file (.json)|*.json|All file (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String[] filenames = dialog.FileNames;
                BUtils.loadFormFiles(filenames, _listRouter, _listStation, _listPath);
                MessageBox.Show("Import successfull", "Info", MessageBoxButtons.OK);

                initUI();
            }
        }

        private void btnDeleteRouter_Click(object sender, EventArgs e)
        {
            // clear
            clearMap();

            if (listRouter.SelectedIndex == -1)
                return;
            int routerId = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[0]);
            int turn = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[1]);
            _currentRouter = routerId;
            Router router = _listRouter[routerId];
            if (router == null)
            {
                MessageBox.Show("Router not found!");
                return;
            }
            deleteRouter(routerId);
            initUI();
            MessageBox.Show("Delete router successfully");
        }

        #endregion

        private void initUI()
        {
            listRouter.Items.Clear();
            foreach (var value in _listRouter)
            {
                if (BUtils.isNotNull(value.Value.Stations1) && value.Value.Stations1.Count != 0)
                    listRouter.Items.Add(value.Value.RouterId + "_0");
                if (BUtils.isNotNull(value.Value.Stations2) && value.Value.Stations2.Count != 0)
                    listRouter.Items.Add(value.Value.RouterId + "_1");

            }

        }

        private void loadBusData()
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

        private void updateWhenChangeStation(int stationId, PointLatLng newPos, int turn)
        {
            // Update station location
            Station station = _listStation[stationId];
            if (station == null)
                return;
            station.LatLong = new LatLong(newPos.Lat, newPos.Lng);

            List<int> routerIds = station.Routers;
            for (int i = 0; i < routerIds.Count; i++)
            {
                Router router = _listRouter[routerIds[i]];
                List<int> stationIds = null;
                if (turn == 0)
                    stationIds = router.Stations1;
                else if (turn == 1)
                    stationIds = router.Stations2;
                else
                    stationIds = null;


                if (stationIds == null || stationIds.Count <= 1)
                    return;
                int preStationId = -1, nextStationId = -1;
                for (int j = 0; j < stationIds.Count; j++)
                {
                    if (stationIds[j] == stationId)
                    {
                        if (j == 0)
                            nextStationId = stationIds[j + 1];
                        else if (j == stationIds.Count - 1)
                            preStationId = stationIds[j - 1];
                        else
                        {
                            preStationId = stationIds[j - 1];
                            nextStationId = stationIds[j + 1];
                        }
                        break;
                    }
                }
                List<string> listPaths = new List<string>();
                if (preStationId != -1)
                    listPaths.Add(preStationId + "_" + stationId);
                if (nextStationId != -1)
                    listPaths.Add(stationId + "_" + nextStationId);

                for (int j = 0; j < listPaths.Count; j++)
                    updateStationPath(listPaths[j]);
            }

            clearMap();
            //for (int i = 0; i < routerIds.Count; i++)
            //    drawRouter(routerIds[i], turn);
            drawRouter(_currentRouter, turn);
        }

        /**
         * turn = 0 or turn 1
         */
        private void drawRouter(int routerId, int turn)
        {
            if (turn != 0 && turn != 1)
                return;
            Router router = _listRouter[routerId];
            if (router == null)
                return;
            List<int> stationIds = null;
            if (turn == 0)
                stationIds = router.Stations1;
            else if (turn == 1)
                stationIds = router.Stations2;

            drawStations(stationIds, routerId, turn);

        }

        private void drawStations(List<int> listStation, int routerId = -1, int turn = 0)
        {

            // draw station
            drawStation(listStation, routerId, turn);
            // line
            drawStationPath(listStation);
        }

        private void drawStation(List<int> listStation, int routerId = -1, int turn = 0)
        {
            for (int i = 0; i < listStation.Count; i++)
            {
                Station currentStation = _listStation[listStation[i]];
                // avoid duplicate StationMaker
                if (_drawedStation.Contains(currentStation.StationId))
                    continue;
                StationMaker maker = new StationMaker(currentStation.LatLong.Latitude, currentStation.LatLong.Longitude);
                maker.StationId = currentStation.StationId;
                maker.RouterId = routerId;
                maker.Turn = turn;
                markersOverlay.Markers.Add(maker);
                _drawedStation.Add(currentStation.StationId);
            }
        }

        private void drawStationPath(List<int> stationIds)
        {

            List<PointLatLng> list = new List<PointLatLng>();
            int preStation = 0;
            for (int i = 0; i < stationIds.Count; i++)
            {
                int station = stationIds[i];
                if (preStation == 0)
                    preStation = station;
                else
                {
                    string key = preStation + "_" + station;
                    if (!_listPath.ContainsKey(key))
                    {
                        preStation = station;
                        continue;
                    }
                    StationPath path = _listPath[key];
                    List<LatLong> points = path.Path;
                    for (int j = 0; j < points.Count; j++)
                    {
                        LatLong point = points[j];
                        PointLatLng pont1 = new PointLatLng(point.Latitude, point.Longitude);
                        list.Add(pont1);
                    }
                    preStation = station;
                }
            }

            GMapRoute routers = new GMapRoute(list, "polygon");
            routerOverlay.Routes.Add(routers);
        }

        private void updateStationPath(string stationPathId)
        {
            try
            {
                if (!_listPath.ContainsKey(stationPathId))
                    return;
                StationPath path = _listPath[stationPathId];
                Station startStation, endStation;
                if (!_listStation.ContainsKey(path.StartStation))
                    throw new Exception("Invalid station: " + path.StartStation);
                startStation = _listStation[path.StartStation];
                if (!_listStation.ContainsKey(path.EndStation))
                    throw new Exception("Invalid station: " + path.EndStation);
                endStation = _listStation[path.EndStation];

                PointLatLng start = new PointLatLng(startStation.LatLong.Latitude, startStation.LatLong.Longitude);
                PointLatLng end = new PointLatLng(endStation.LatLong.Latitude, endStation.LatLong.Longitude);
                MapRoute mapRouter = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRoute(start, end, false, false, 50);
                path.Path.Clear();
                long distance = (long)(mapRouter.Distance * 1000); // as meter
                for (int j = 0; j < mapRouter.Points.Count; j++)
                {
                    PointLatLng point = mapRouter.Points[j];
                    LatLong latlong = new LatLong();
                    latlong.Latitude = point.Lat;
                    latlong.Longitude = point.Lng;
                    path.Path.Add(latlong);

                }
                path.Distance = distance;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        private void clearMap()
        {
            _drawedStation.Clear();
            markersOverlay.Clear();
            routerOverlay.Clear();
            currentMarker = null;
        }

        private bool deleteRouter(int routerId)
        {
            if (!_listRouter.ContainsKey(routerId))
                return false;
            Router router = _listRouter[routerId];
            _listRouter.Remove(routerId);
            removeStations(router.Stations1, routerId);
            removeStations(router.Stations2, routerId);
            
            return true;
        }

        private void removeStations(List<int> stations, int routerId)
        {
            for (int i = 0; i < stations.Count; i++)
            {
                Station station = _listStation[stations[i]];
                station.Routers.Remove(routerId);
                if (station.Routers.Count == 0)
                    _listStation.Remove(station.StationId);
            }
        }

        private void standeData()
        {
            foreach (var item in _listStation)
            {
                item.Value.Routers = item.Value.Routers.Distinct().ToList();
            }
        }

        public List<T> convertDicToList<K, T>(Dictionary<K, T> dic)
        {
            List<T> result = new List<T>();

            foreach (KeyValuePair<K, T> entry in dic)
            {
                result.Add(entry.Value);
            }
            return result;
        }
        
    }
}
