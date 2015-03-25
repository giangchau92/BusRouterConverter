using BusRouterConverter;
using BusRouterEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusFindingDemo
{
    public class DijkstraFinding
    {
        private BusManager _busData = null;
        private Dictionary<int, Vertex> _listVertex = new Dictionary<int, Vertex>();
        private Dictionary<string, Edge> _listEdge = new Dictionary<string, Edge>();

        public DijkstraFinding(BusManager busMgr)
        {
            if (busMgr == null)
                throw new ArgumentNullException("DijkstraFinding:DijkstraFinding busMgr null");
            _busData = busMgr;
        }

        public bool addRouterAtStation(int routerId, int stationId, int turn)
        {
            if (!_busData.Routers.ContainsKey(routerId) ||
                !_busData.Stations.ContainsKey(stationId) ||
                !BUtils.isValidTurn(turn))
                return false;

            List<int> stations = _busData.Routers[routerId].getStations(turn);
            bool check = false;
            for (int i = 0; i < stations.Count - 1; i++)
            {
                if (check || stations[i] == stationId)
                {
                    addPath(stations[i], stations[i + 1]);
                    check = true;
                }
            }
            return true;
        }

        public bool addPath(int stationA, int stationB)
        {
            if (!_busData.Stations.ContainsKey(stationA) || !_busData.Stations.ContainsKey(stationB))
                return false;
            Vertex vertA = checkAndAddVertex(stationA);
            Vertex vertB = checkAndAddVertex(stationB);

            string pathId = BUtils.generatePathId(stationA, stationB);
            Edge edge = checkAndAddEdge(pathId);
            StationPath path = _busData.getStationPath(pathId);
            edge.Target = vertB;
            edge.Weight = path.Distance;
            addEdgeToVertex(edge, vertA);

            return true;
        }

        #region NOT USE

        private Vertex addVertex(Vertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException("DijkstraFinding:addVertex vertex null");
            if (_listVertex.ContainsKey(vertex.StationId))
                _listVertex.Remove(vertex.StationId);

            _listVertex.Add(vertex.StationId, vertex);
            return vertex;
        }

        private Vertex removeVertex(int id)
        {
            Vertex result = null;
            if (_listVertex.ContainsKey(id))
            {
                result = _listVertex[id];
                _listVertex.Remove(id);
            }
            return result;
        }

        private Vertex getVertex(int id)
        {
            Vertex result = null;
            if (_listVertex.ContainsKey(id))
                result = _listVertex[id];
            return result;
        }

        private Edge addEdge(Vertex vertex, Edge edge)
        {
            if (vertex == null || edge == null)
                throw new ArgumentNullException("DijkstraFinding:addEgg vertex, egg null");
            if (!_listVertex.ContainsKey(vertex.StationId))
                return null;
            vertex.Adjacencies.Add(edge);
            if (!_listEdge.ContainsKey(edge.PathId))
                _listEdge.Add(edge.PathId, edge);
            return edge;
        }

        private Edge addEdge(int vertexId, Edge edge)
        {
            if (!_listVertex.ContainsKey(vertexId))
                return null;
            return addEdge(_listVertex[vertexId], edge);
        }

        #endregion

        public Vertex computePath(int fromId, int toId)
        {
            if (!_listVertex.ContainsKey(fromId) || !_listVertex.ContainsKey(toId))
                return null;

            Vertex source = _listVertex[fromId];
            Vertex destination = _listVertex[toId];

            source.MinDistance = 0;
            SortedVertexList listQueue = new SortedVertexList();
            List<Vertex> visitedVert = new List<Vertex>();
            
            listQueue.Add(source);
            while (listQueue.Count != 0)
            {
                Vertex u = listQueue.poll();
                visitedVert.Add(u);
                // Visit each edge exiting u
                foreach (Edge e in u.Adjacencies)
                {
                    Vertex v = e.Target;
                    double weight = e.Weight;
                    double distanceThroughU = u.MinDistance + weight;
                    if (distanceThroughU < v.MinDistance) {
                        //listQueue.Remove(v);
                        v.MinDistance = distanceThroughU ;
                        v.PreviewVertex = u;
                        if (v == destination)
                            return destination;
                        if (!listQueue.Contains(v))
                            listQueue.Add(v);
                    }
                }

                listQueue.Sort(new SortedVertexList.VertexComparer());
            }
            return destination;
        }

        public List<int> convertPathToStations(Vertex vertex)
        {
            List<int> result = new List<int>();
            while (vertex != null)
            {
                result.Add(vertex.StationId);
                vertex = vertex.PreviewVertex;
            }
            result.Reverse();
            return result;
        }
        
        /**
         * private function
         */

        private Edge addEdgeToVertex(Edge edge, Vertex vertex)
        {
            if (edge == null || vertex == null)
                return null;
            vertex.addAdjacencies(edge);
            if (_listEdge.ContainsKey(edge.PathId))
                _listEdge.Remove(edge.PathId);
            _listEdge.Add(edge.PathId, edge);
            return edge;
        }
        private Vertex checkAndAddVertex(int stationId)
        {
            Vertex vertA = null;
            if (!_listVertex.ContainsKey(stationId))
            {
                vertA = new Vertex(stationId);
                _listVertex.Add(stationId, vertA);
            }
            else
                vertA = _listVertex[stationId];
            return vertA;
        }
        private Edge checkAndAddEdge(string pathId)
        {
            Edge edge = null;
            if (!_listEdge.ContainsKey(pathId))
            {
                edge = new Edge(pathId);
                _listEdge.Add(pathId, edge);
            }
            else
                edge = _listEdge[pathId];
            return edge;
        }

        //
        public Dictionary<int, Router> Routers { get { return BusManager.Inst().Routers; } }
        public Dictionary<int, Station> Stations { get { return BusManager.Inst().Stations; } }
        public Dictionary<string, StationPath> StationPaths { get { return BusManager.Inst().StationPaths; } }
    }

    public class Vertex
    {
        public int StationId { get; set; }
        public List<Edge> Adjacencies { get; private set; }
        public double MinDistance { get; set; }
        public Vertex PreviewVertex { get; set; }
        public Vertex(int id)
        {
            StationId = id;
            Adjacencies = new List<Edge>();
            MinDistance = Double.MaxValue;
            PreviewVertex = null;
        }

        public Edge addAdjacencies(Edge newEdge)
        {
            Edge edge = Adjacencies.Find(x => x.PathId == newEdge.PathId);
            if (edge != null)
                Adjacencies.Remove(edge);
            Adjacencies.Add(newEdge);

            return newEdge;
        }
    }

    public class Edge
    {
        public string PathId { get; set; }
        public Vertex Target { get; set; }
        public double Weight { get; set; }
        public Edge(string id, Vertex target = null, double weight = 0)
        {
            PathId = id;
            Target = target;
            Weight = weight;
        }
    }

    public class SortedVertexList : List<Vertex>
    {
        public SortedVertexList()
            :base()
        {

        }

        public Vertex poll()
        {
            Vertex result = null;
            if (this.Count != 0)
            {
                result = this.ElementAt(0);
                this.Remove(result);
            }
            return result;
        }
        

        public class VertexComparer : Comparer<Vertex>
        {
            public override int Compare(Vertex x, Vertex y)
            {
                return Comparer<double>.Default.Compare(x.MinDistance, y.MinDistance);
            }
        }
    }
}
