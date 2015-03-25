using BusRouterConverter;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using GMap.NET.MapProviders;
using BusRouterEditor;

namespace BusFindingDemo
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay;
        private GMapOverlay routerOverlay;
        private GMapOverlay sortedPathOverlay;
        private GMarkerGoogle markerA = null;
        private GMarkerGoogle markerB = null;

        private int _currentRouter = 0;
        private int _startStation = 0;
        private int _endStation = 0;
        private Keys _currentKeyDown = 0;

        private List<int> _drawedStation = new List<int>();

        public Dictionary<int, Router> Routers { get { return BusManager.Inst().Routers; }}
        public Dictionary<int, Station> Stations { get { return BusManager.Inst().Stations; }}
        public Dictionary<string, StationPath> StationPaths { get { return BusManager.Inst().StationPaths; }}

        public Form1()
        {
            InitializeComponent();
            loadBusData();
            initUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.GoogleMap;
            gmap.Position = new PointLatLng(54.6961334816182, 25.2985095977783);
            gmap.MinZoom = 0;
            gmap.MaxZoom = 24;
            gmap.Zoom = 9;

            markersOverlay = new GMapOverlay("markers");
            routerOverlay = new GMapOverlay("routers");
            sortedPathOverlay = new GMapOverlay("routers");

            gmap.Overlays.Add(routerOverlay);
            gmap.Overlays.Add(markersOverlay);
            gmap.Overlays.Add(sortedPathOverlay);
            

            gmap.OnMarkerClick += gmap_OnMarkerClick;
            gmap.KeyDown += gmap_KeyDown;
        }

        private void gmap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _currentKeyDown = Keys.None;
            }

            _currentKeyDown = e.KeyCode;
        }

        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            StationMaker marker = item as StationMaker;
            if (_currentKeyDown == Keys.A)
            {
                if (markerA != null)
                    markersOverlay.Markers.Remove(markerA);
                _startStation = marker.StationId;
                StartMark mark = new StartMark(marker.Position.Lat, marker.Position.Lng);
                mark.StationId = marker.StationId;
                markersOverlay.Markers.Add(mark);
                markerA = mark;
            }
            else if (_currentKeyDown == Keys.S)
            {
                if (markerA != null)
                    markersOverlay.Markers.Remove(markerB);
                _endStation = marker.StationId;
                EndMarker mark = new EndMarker(marker.Position.Lat, marker.Position.Lng);
                mark.StationId = marker.StationId;
                markersOverlay.Markers.Add(mark);
                markerB = mark;
            }
            else if (_currentKeyDown == Keys.D)
            {
                if (marker == null)
                    return;
                _startStation = marker.StationId;
                StartMark mark = new StartMark(marker.Position.Lat, marker.Position.Lng);
                mark.StationId = marker.StationId;
                markersOverlay.Markers.Add(mark);
                markerA = mark;
                showTreePathAtStation(new List<int>(new int[]{ _startStation }), 1, new List<int>());
            }
            else
            {
                _startStation = 0;
                _endStation = 0;
            }
        }

        private void loadBusData()
        {
            BusManager.Inst().loadBusData();
        }

        private void initUI()
        {
            listRouter.Items.Clear();
            foreach (var value in Routers)
            {
                if (BUtils.isNotNull(value.Value.Stations1) && value.Value.Stations1.Count != 0)
                    listRouter.Items.Add(value.Value.RouterId + "_0");
                if (BUtils.isNotNull(value.Value.Stations2) && value.Value.Stations2.Count != 0)
                    listRouter.Items.Add(value.Value.RouterId + "_1");

            }

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
            Router router = Routers[routerId];
            if (router == null)
            {
                MessageBox.Show("Router not found!");
                return;
            }
            drawRouter(routerId, turn);
            gmap.Position = new PointLatLng(10.8012494, 106.6783981);
            gmap.Zoom = 13f;
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            if (listRouter.SelectedItem == null)
            {
                MessageBox.Show("Not selected router", "Error", MessageBoxButtons.OK);
                return;
            }
            int routerId = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[0]);
            int turn = Int32.Parse(listRouter.SelectedItem.ToString().Split('_')[1]);

            DijkstraFinding dijkstra = new DijkstraFinding(BusManager.Inst());
            Router router = null;
            if (!Routers.ContainsKey(routerId))
            {
                MessageBox.Show("Invalid router", "Error", MessageBoxButtons.OK);
                return;
            }
            router = Routers[routerId];
            List<int> stationIds = null;
            if (turn == 0)
                stationIds = router.Stations1;
            else if (turn == 1)
                stationIds = router.Stations2;
            if (stationIds == null)
            {
                MessageBox.Show("Invalid turn", "Error", MessageBoxButtons.OK);
                return;
            }

            for (int i = 0; i < stationIds.Count - 1; i++)
            {
                dijkstra.addPath(stationIds[i], stationIds[i + 1]);
            }

            Vertex dest = dijkstra.computePath(stationIds[0], stationIds[stationIds.Count - 20]);
            if (dest == null)
            {
                MessageBox.Show("Cannot find shortest path");
                return;
            }

            List<int> listStation = new List<int>();
            while (dest.PreviewVertex != null)
            {
                listStation.Add(dest.StationId);
                dest = dest.PreviewVertex;
            }
            listStation.Reverse();
            clearMap();
            drawStationPath(listStation, Color.Red, sortedPathOverlay);
        }

        private void btnCompute2_Click(object sender, EventArgs e)
        {
            try
            {
                DijkstraFinding dijkstra = new DijkstraFinding(BusManager.Inst());

                string[] listRouter = tbRouters.Text.Split(',');
                for (int i = 0; i < listRouter.Length; i++)
                {
                    string[] parts = listRouter[i].Split('_');
                    int routerId = Int32.Parse(parts[0]);
                    int turn = Int32.Parse(parts[1]);

                    dijkstra.addRouterAtStation(routerId, Routers[routerId].getStations(turn)[0], turn);        
                }
                Vertex dest = dijkstra.computePath(_startStation, _endStation);
                if (dest == null || dest.PreviewVertex == null)
                {
                    MessageBox.Show("Cannot find shortest path");
                    return;
                }
                List<int> pathStation = dijkstra.convertPathToStations(dest);
                //clearMap();
                sortedPathOverlay.Clear();
                drawStationPath(pathStation, Color.Red, sortedPathOverlay);
            } catch
            {
                MessageBox.Show("Invalid routers. Try again!");
            }
        }

        private void btnShowListRouter_Click(object sender, EventArgs e)
        {
            try
            {
                showListRouter(tbRouters.Text);
                gmap.Position = new PointLatLng(10.8012494, 106.6783981);
                gmap.Zoom = 13f;
            } catch {
                MessageBox.Show("Invalid routers. Try again!");
            }
        }

        private void btnShowStation_Click(object sender, EventArgs e)
        {
            showAllStation();
            gmap.Position = new PointLatLng(10.8012494, 106.6783981);
            gmap.Zoom = 13f;
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            showHideStation();
        }

        private void showAllStation()
        {
            List<int> stations = new List<int>();
            foreach (var value in Stations)
            {
                stations.Add(value.Value.StationId);
            }
            clearMap();
            drawStation(stations);
        }

        private void showListRouter(string strRouter)
        {
            clearMap();
            if (strRouter == String.Empty)
            {

                foreach (var value in Routers)
                    strRouter += value.Value.RouterId.ToString() + "_0," + value.Value.RouterId.ToString() + "_1,";
                    //strRouter += value.Value.RouterId.ToString() + "_0,";
                strRouter = strRouter.Substring(0, strRouter.Length - 1);
                tbRouters.Text = strRouter;
            }
            string[] listRouter = strRouter.Split(',');
            for (int i = 0; i < listRouter.Length; i++)
            {
                string[] parts = listRouter[i].Split('_');
                int routerId = Int32.Parse(parts[0]);
                int turn = Int32.Parse(parts[1]);
                drawRouter(routerId, turn);
            }
        }

        private void clearMap()
        {
            _drawedStation.Clear();
            markersOverlay.Clear();
            routerOverlay.Clear();
        }

        private void drawRouter(int routerId, int turn)
        {
            if (turn != 0 && turn != 1)
                return;
            Router router = Routers[routerId];
            if (router == null)
                return;
            List<int> stationIds = null;
            if (turn == 0)
                stationIds = router.Stations1;
            else if (turn == 1)
                stationIds = router.Stations2;

            drawStations(stationIds, routerId, turn);
        }

        private void drawRouterAtStation(int routerId, int stationId, Color color)
        {
            List<int> stations = new List<int>();
            bool check = false;
            Router router = Routers[routerId];
            List<int> turn = router.getTurn(stationId);

            foreach (var eachTurn in turn)
            {
                List<int> listStation = router.getStations(eachTurn);

                for (int i = 0; i < listStation.Count; i++)
                {
                    if (check || listStation[i] == stationId)
                    {
                        check = true;
                        stations.Add(listStation[i]);
                    }
                }
                drawStationPath(stations, color, routerOverlay);
            }
        }

        private void drawStations(List<int> listStation, int routerId = -1, int turn = 0)
        {

            // draw station
            drawStation(listStation, routerId, turn);
            // line
            drawStationPath(listStation, Color.Blue, routerOverlay);
        }

        private void drawStation(List<int> listStation, int routerId = -1, int turn = 0)
        {
            for (int i = 0; i < listStation.Count; i++)
            {
                Station currentStation = Stations[listStation[i]];
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

        private void drawStationPath(List<int> stationIds, Color color, GMapOverlay overlay)
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
                    string key = BUtils.generatePathId(preStation, station);
                    if (!StationPaths.ContainsKey(key))
                    {
                        preStation = station;
                        continue;
                    }
                    StationPath path = StationPaths[key];
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
            routers.Stroke = new Pen(color);
            routers.Stroke.Width = 5;
            overlay.Routes.Add(routers);
            
        }

        private void showTreePathAtStation(List<int> stationIds, int deep, List<int> visitedRouters)
        {
            if (deep <= 0)
                return;
            for (int j = 0; j < stationIds.Count; j++)
            {
                int stationId = stationIds[j];
                Station station = Stations[stationId];
                for (int i = 0; i < station.Routers.Count; i++)
                {
                    int routerId = station.Routers[i];
                    //if (visitedRouters.Contains(routerId))
                    //    continue;
                    visitedRouters.Add(routerId);
                    Color color = BUtils.randomColor();
                    drawRouterAtStation(routerId, stationId, color);
                }
                showTreePathAtStation(stationIds, --deep, visitedRouters);
            }
            
        }

        private void showHideStation()
        {
            if (gmap.Overlays.Contains(markersOverlay))
                gmap.Overlays.Remove(markersOverlay);
            else
                gmap.Overlays.Add(markersOverlay);
            gmap.Refresh();
        }

    }
}
