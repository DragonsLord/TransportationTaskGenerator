using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model.Implementations
{
    class PdfSaver : ISaver
    {
        static readonly BaseFont baseFont = BaseFont.CreateFont($"{ System.AppDomain.CurrentDomain.BaseDirectory}OpenSans-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        static readonly iTextSharp.text.Font cellStyle = new iTextSharp.text.Font(baseFont, 16, 0, BaseColor.BLACK);
        private IEnumerable<SolvedTask> answers;
        public void Save(IEnumerable<SolvedTask> answers,int clearA,int clearB)
        {
            this.answers = answers;
            SaveTask();
            SaveAnswer(clearA, clearB);


        }
        public void SaveTask()
        {
            string path = $"{System.AppDomain.CurrentDomain.BaseDirectory}/task.pdf";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                Paragraph p1 = new Paragraph();
                p1.Alignment = Element.ALIGN_LEFT;
                p1.Add(new Chunk(("Завдання").ToUpper(), new iTextSharp.text.Font(baseFont, 22, 1, BaseColor.BLACK)));
                p1.SpacingAfter = 20;
                doc.Add(p1);
                int variantNumber = 1;
                GraphBuilder builder;
                foreach (var single in answers)
                {
                    builder = new GraphBuilder(single.Task.Senders.Length, single.Task.Recievers.Length, single.Task.Restrictions.GetLength(1));
                    p1 = new Paragraph();
                    p1.Alignment = Element.ALIGN_LEFT;
                    p1.Add(new Chunk(("Варіант #" + variantNumber).ToUpper(), new iTextSharp.text.Font(baseFont, 20, 1, BaseColor.BLACK)));
                    p1.SpacingAfter = 20;
                    doc.Add(p1);

                    doc.Add(new Paragraph(new Chunk(("Вихідні пункти"), new iTextSharp.text.Font(baseFont, 18, 1, BaseColor.BLACK))) { SpacingAfter = 20 });
                    PdfPTable table = new PdfPTable(2);
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    for (int i = 0; i < single.Task.Senders.Length; i++)
                    {
                        table.AddCell(GetCell((Convert.ToChar(65).ToString() + i).ToString()));
                        table.AddCell(GetCell(single.Task.Senders[i].ToString()));
                    }
                    doc.Add(table);

                    doc.Add(new Paragraph(new Chunk(("Кінцеві пункти"), new iTextSharp.text.Font(baseFont, 18, 1, BaseColor.BLACK))) { SpacingAfter = 20 });
                    table = new PdfPTable(2);
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    for (int i = 0; i < single.Task.Recievers.Length; i++)
                    {
                        table.AddCell(GetCell((Convert.ToChar(66).ToString() + i).ToString()));
                        table.AddCell(GetCell(single.Task.Recievers[i].ToString()));
                    }
                    doc.Add(table);
                    Image img = Image.GetInstance(builder.Build(single.Task.Restrictions), System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit(doc.PageSize);
                    img.SpacingBefore = 30;
                    img.Alignment = Element.ALIGN_CENTER;
                    doc.Add(img);
                    variantNumber++;
                    doc.NewPage();
                }
                doc.Close();
                writer.Close();
            }
        }
        private void SaveAnswer(int clearA, int clearB)
        {
            string path = $"{System.AppDomain.CurrentDomain.BaseDirectory}/answer.pdf";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                Paragraph p1 = new Paragraph();
                p1.Alignment = Element.ALIGN_LEFT;
                p1.Add(new Chunk(("Розв'язки").ToUpper(), new iTextSharp.text.Font(baseFont, 22, 1, BaseColor.BLACK)));
                p1.SpacingAfter = 20;
                //doc.Add(p1);
                int variantNumber = 1;
                GraphBuilder builder;
                foreach (var single in answers)
                {
                    int buffer = single.Task.Senders.ToList().Sum();
                    builder = new GraphBuilder(single.Task.Senders.Length, single.Task.Recievers.Length, single.Task.Restrictions.GetLength(1));
                    p1 = new Paragraph();
                    p1.Alignment = Element.ALIGN_LEFT;
                    p1.Add(new Chunk(("Варіант #" + variantNumber).ToUpper(), new iTextSharp.text.Font(baseFont, 20, 1, BaseColor.BLACK)));
                    p1.SpacingAfter = 20;
                    doc.Add(p1);

                    doc.Add(new Paragraph(new Chunk(("Вхідна матриця"), new iTextSharp.text.Font(baseFont, 18, 1, BaseColor.BLACK))) { SpacingAfter = 20 });
                    
                    doc.Add(GetTable(single,clearA,clearB,buffer));

                   
                    doc.Add(new Paragraph(new Chunk(("Вихідна матриця"), new iTextSharp.text.Font(baseFont, 18, 1, BaseColor.BLACK))) { SpacingAfter = 20 });

                    doc.Add(GetTableRoads(single));

                    doc.Add(new Paragraph(new Chunk(("Z = " + single.Value), new iTextSharp.text.Font(baseFont, 18, 1, BaseColor.BLACK))) { SpacingAfter = 20 });

                    Image img = Image.GetInstance(builder.Build(single.Roads), System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit((float)(doc.PageSize.Width*0.8),(float)(doc.PageSize.Height*0.8));
                    img.SpacingBefore = 0;
                    img.Alignment = Element.ALIGN_CENTER;
                    doc.Add(img);
                    variantNumber++;
                    doc.NewPage();
                }
                doc.Close();
                writer.Close();
            }

        }
        private PdfPCell GetCell(string text)
        {
            return new PdfPCell(new Phrase(text, cellStyle)) { PaddingTop = 10, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER };
        }
        private PdfPTable GetTable(SolvedTask task,int a,int b,int buffer)
        {
            PdfPTable table = new PdfPTable(task.Task.Restrictions.GetLength(0)+2-a);
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(GetCell(" "));
            for (int i = 0; i < task.Task.Restrictions.GetLength(0)-a; i++)
            {
                table.AddCell(GetCell(GetLabelForNode(i, task)));
            }
            table.AddCell(GetCell(" "));
            for (int i = 0; i < task.Task.Restrictions.GetLength(0)-b; i++)
            {
                for (int j = a; j < task.Task.Restrictions.GetLength(1); j++)
                {
                    if (j < a+1)
                    {
                        table.AddCell(GetCell(GetLabelForNode(i+a, task)));
                    }
                    if (task.Task.Restrictions[i, j] == 1000000)
                    {

                        table.AddCell(GetCell("M"));
                    }
                    else
                    {
                        table.AddCell(GetCell(task.Task.Restrictions[i, j].ToString()));
                    }


                }
                if (i < task.Task.Senders.Length)
                {
                    if (i < a)
                    {
                        table.AddCell(GetCell(task.Task.Senders[i].ToString()));
                    }
                    else
                    {
                        table.AddCell(GetCell((task.Task.Senders[i] + buffer).ToString()));
                    }
                }
                else
                {
                    table.AddCell(GetCell(buffer.ToString()));
                }
            }
            table.AddCell(GetCell(" "));
            for (int i = a; i < task.Task.Restrictions.GetLength(1); i++)
            {
                if (i >= task.Task.Restrictions.GetLength(1) - task.Task.Recievers.Length)
                {
                    if (i >= task.Task.Restrictions.GetLength(1) - b)
                    {
                        table.AddCell(GetCell((task.Task.Recievers[i - task.Task.Restrictions.GetLength(1) + task.Task.Recievers.Length]).ToString()));
                    }
                    else
                    {
                        table.AddCell(GetCell((task.Task.Recievers[i - task.Task.Restrictions.GetLength(1) + task.Task.Recievers.Length] + buffer).ToString()));
                    }
                }
                else
                {
                    table.AddCell(GetCell(buffer.ToString()));
                }
            }
            table.AddCell(GetCell(" "));
            return table;
        }
        private PdfPTable GetTableRoads(SolvedTask task)
        {
            PdfPTable table = new PdfPTable(task.Roads.GetLength(1) + 1);
            table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.DefaultCell.PaddingTop = 10;
            table.DefaultCell.PaddingBottom = 10;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(GetCell(" "));
            for (int i = 0; i < task.Roads.GetLength(0); i++)
            {
                table.AddCell(GetCell(GetLabelForNode(i, task)));
            }
            for (int i = 0; i < task.Roads.GetLength(0); i++)
            {
                for (int j = 0; j < task.Roads.GetLength(1); j++)
                {
                    if (j < 1)
                    {
                        table.AddCell(GetCell(GetLabelForNode(i, task)));
                    }
                    if (task.Roads[i, j] == 1000000)
                    {

                        table.AddCell(GetCell("M"));
                    }
                    else
                    {
                        table.AddCell(GetCell(task.Roads[i, j].ToString()));
                    }

                }
            }
            return table;
        }
        private string GetLabelForNode(int number,SolvedTask task)
        {
            if (number < task.Task.Senders.Length) return (Convert.ToChar(65).ToString() + number).ToString();

            else if (task.Task.Restrictions.GetLength(0) - task.Task.Recievers.Length <= number) return (Convert.ToChar(66).ToString() + (number - (task.Task.Restrictions.GetLength(0) - task.Task.Recievers.Length))).ToString();

            else return (Convert.ToChar(67).ToString() + (number - task.Task.Senders.Length)).ToString();
        }
    }
}
