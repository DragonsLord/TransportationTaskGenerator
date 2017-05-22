using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model.Implementations
{
    class GraphBuilder : IGraphBuilder
    {
        private BidirectionalGraph<string, Edge<string>> _graph = new BidirectionalGraph<string, Edge<string>>(true);
        private Dictionary<Edge<string>, double> _cost = new Dictionary<Edge<string>, double>();

        public void Build(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (j < 1)
                    {
                        _graph.AddVertex(Convert.ToChar(65 + j).ToString());
                    }
                    if (matrix[i, j] != 0)
                    {
                        _graph.AddEdge(new Edge<string>(Convert.ToChar(65 + i).ToString(), Convert.ToChar(65 + j).ToString()));
                        _cost.Add(new Edge<string>(Convert.ToChar(65 + i).ToString(), Convert.ToChar(65 + j).ToString()), matrix[i, j]);
                    }
                }
            }

            _graph.Visualize("graph", dir: Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }
    }
}