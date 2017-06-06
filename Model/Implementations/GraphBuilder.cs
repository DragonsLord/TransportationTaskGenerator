using GraphX.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model.Implementations
{
    class GraphBuilder : IGraphBuilder
    {
        private int _countA;
        private int _countB;
        private int _totalRows;
        private int _totalColumn;

        public GraphBuilder(int a,int b,int totalRows,int totalColumn)
        {
            _countA = a;
            _countB = b;
            _totalRows = totalRows;
            _totalColumn = totalColumn;
        }

        public System.Drawing.Image Build(int[,] matrix)
        {
            GViewer viewer = new GViewer();
            Graph graph = new Graph("my");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (i < 1)
                    {
                        if (!CheckNodeColumn(matrix, j) && !CheckNodeRow(matrix, j))
                        {
                            graph.AddNode(GetLabelForNode(j));
                        }
                    }
                    if (matrix[i, j] != 0 && matrix[i, j] != 1000000 && i!=j)
                    {
                        Edge e = graph.AddEdge(GetLabelForNode(i), matrix[i, j].ToString(), GetLabelForNode(j));
                        e.Label.FontSize = 6;
                    }
                }
            }
            List<Node> sender = new List<Node>();
            for (int i = 0; i < _countA; i++)
            {
                sender.Add(graph.FindNode((Convert.ToChar(65).ToString() + (i + 1)).ToString()));
            }
            graph.LayerConstraints.PinNodesToSameLayer(sender.ToArray());
            sender = new List<Node>();
            for (int i = 0; i < _countB; i++)
            {
                sender.Add(graph.FindNode((Convert.ToChar(66).ToString() + (i + 1)).ToString()));
            }
            graph.LayerConstraints.PinNodesToSameLayer(sender.ToArray());

            graph.Attr.LayerDirection = LayerDirection.LR;


            int width = 1920;
            Bitmap bitmap = new Bitmap(width, 1080, PixelFormat.Format32bppPArgb);
            GraphRenderer renderer = new GraphRenderer(graph);
            renderer.CalculateLayout();
            renderer.Render(bitmap);

            return (System.Drawing.Image)bitmap;
        }

        private bool CheckNodeColumn(int[,] matrix,int k)
        {
            int count = 0;
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                if (k < matrix.GetLength(1) && ((matrix[j, k] != 0 || matrix[j, k] != 1000000)))
                {
                    count++;
                }
            }
            return matrix.GetLength(0)==count;
        }
        private bool CheckNodeRow(int[,] matrix, int k)
        {
            int count = 0;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (k < matrix.GetLength(0)  && ((matrix[k, j] != 0 || matrix[k, j] != 1000000)))
                {
                    count++;
                }
            }
            return matrix.GetLength(0) == count;
        }

        private string GetLabelForNode(int number)
        {
            int fict = _totalRows > _totalColumn ? _totalRows : _totalColumn;

            if (number == fict) return (Convert.ToChar(70).ToString()).ToString();
            else if (number<_countA) return (Convert.ToChar(65).ToString() + (number + 1)).ToString();

            else if (_totalRows - _countB <= number) return (Convert.ToChar(66).ToString() + (number + 1 - (_totalRows - _countB))).ToString();

            else return (Convert.ToChar(67).ToString() + (number + 1 -_countA)).ToString();
        }

    }
}