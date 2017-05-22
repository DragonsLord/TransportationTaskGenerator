using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TransportTasksGenerator.Model.Implementations
{
    internal static class Visualizer
    {
        public static void Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, string dir)
            where TEdge : IEdge<TVertex>
        {
            Visualize(graph, fileName, NoOpEdgeFormatter, dir);
        }

        public static void Visualize<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph,
            string fileName, FormatEdgeAction<TVertex, TEdge> edgeFormatter, string dir)
            where TEdge : IEdge<TVertex>
        {
            var fullFileName = Path.Combine(dir, fileName);
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            //viz.GraphFormat.PageDirection = GraphvizPageDirection.LT;
            // viz.CommonEdgeFormat.Weight = 4;
            //viz.GraphFormat.ClusterRank = GraphvizClusterMode.Local;
            viz.GraphFormat.IsLandscape = true;
            //viz.GraphFormat.IsCentered = true;

            viz.FormatVertex += VizFormatVertex;

            viz.FormatEdge += edgeFormatter;

            viz.Generate(new FileDotEngine(), fullFileName);
        }

        static void NoOpEdgeFormatter<TVertex, TEdge>(object sender, FormatEdgeEventArgs<TVertex, TEdge> e)
            where TEdge : IEdge<TVertex>
        {
            e.EdgeFormatter.Label = new GraphvizEdgeLabel() { Distance = 0, Value= "53463"};
        }

        public static string ToDotNotation<TVertex, TEdge>(this IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var viz = new GraphvizAlgorithm<TVertex, TEdge>(graph);
            viz.FormatVertex += VizFormatVertex;
            return viz.Generate(new DotPrinter(), "");
        }

        static void VizFormatVertex<TVertex>(object sender, FormatVertexEventArgs<TVertex> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
            e.VertexFormatter.FillColor = new GraphvizColor(1, 255, 255, 0);
            e.VertexFormatter.Orientation = 2.0;
        }
    }

    public sealed class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            string output = outputFileName;
            File.WriteAllText(output, dot);

            // assumes dot.exe is on the path:
            var args = string.Format(@"{0} -Tpng -O", output);

            System.Diagnostics.Process.Start(@"E:\Graphviz2.38\bin\dot.exe", args);
            return output;
        }
    }

    public sealed class DotPrinter : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            return dot;
        }
    }
}