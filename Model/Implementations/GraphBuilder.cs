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
        static readonly BaseFont baseFont = BaseFont.CreateFont($"{ System.AppDomain.CurrentDomain.BaseDirectory}/OpenSans-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        static readonly iTextSharp.text.Font cellStyle = new iTextSharp.text.Font(baseFont, 16, 0, BaseColor.BLACK);
        private int _countA;
        private int _countB;
        private int _totalCount;

        public GraphBuilder(int a,int b,int total)
        {
            _countA = a;
            _countB = b;
            _totalCount = total;
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
            if (number<_countA) return (Convert.ToChar(65).ToString() + (number + 1)).ToString();

            else if (_totalCount - _countB <= number) return (Convert.ToChar(66).ToString() + (number + 1 - (_totalCount - _countB))).ToString();

            else return (Convert.ToChar(67).ToString() + (number + 1 -_countA)).ToString();
        }

        private void GeneratePdf(int[,] matrix,string filename)
        {
            string path = $"{System.AppDomain.CurrentDomain.BaseDirectory}/{filename}_1.pdf";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Document doc = new Document(PageSize.A5);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                //Order ID
                Paragraph p1 = new Paragraph();
                p1.Alignment = Element.ALIGN_CENTER;
                p1.Add(new Chunk(("Граф").ToUpper(), new iTextSharp.text.Font(baseFont, 20, 1, BaseColor.BLACK)));
                p1.SpacingAfter = 20;
                doc.Add(p1);

                //Table of dishes
                PdfPTable table = new PdfPTable(matrix.GetLength(1)+1);
                table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.DefaultCell.PaddingTop = 10;
                table.DefaultCell.PaddingBottom = 10;
                table.HorizontalAlignment = Element.ALIGN_CENTER;

                table.AddCell(GetCell(" "));
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    table.AddCell(GetCell(Convert.ToChar(65 + i + 1).ToString()));
                }
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        //add cell with node name
                        if (j<1) table.AddCell(GetCell(Convert.ToChar(65 + i + 1).ToString()));

                        //check weight of edge for ignore edge with M number 
                        if (matrix[i, j]== 1000000) table.AddCell(GetCell("M"));
                        else table.AddCell(GetCell(matrix[i, j].ToString()));
                    }
                }
                doc.Add(table);
                doc.Close();
                writer.Close();
            }
        }
        
        private PdfPCell GetCell(string text) => new PdfPCell(new Phrase(text, cellStyle)) { PaddingTop = 10, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER };
    }
}