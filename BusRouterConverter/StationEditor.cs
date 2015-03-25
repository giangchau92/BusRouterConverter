using GMap.NET;
using GMap.NET.MapProviders;
using Newtonsoft.Json.Linq;
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

namespace BusRouterConverter
{
    public partial class StationEditor : Form
    {
        List<Station> listStation = new List<Station>();
        List<Router> listRouter = new List<Router>();
        List<StationPath> listPath = new List<StationPath>();

        public StationEditor()
        {
            InitializeComponent();
        }

        private void StationEditor_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMapProviders.GoogleMap;
            gmap.Position = new PointLatLng(54.6961334816182, 25.2985095977783);
            gmap.MinZoom = 0;
            gmap.MaxZoom = 24;
            gmap.Zoom = 9;
        }
    }
}
