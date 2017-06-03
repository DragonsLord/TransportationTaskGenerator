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
        private string folder = "Results";
        static readonly BaseFont baseFont = BaseFont.CreateFont($"{ System.AppDomain.CurrentDomain.BaseDirectory}OpenSans-Regular.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        static readonly Font cellStyle = new Font(baseFont, 10, 0, BaseColor.BLACK);
        static readonly Font paragraphStyle = new Font(baseFont, 12, 1, BaseColor.BLACK);
        private IEnumerable<SolvedTask> answers;
        public void Save(IEnumerable<SolvedTask> answers,int clearA,int clearB,string folderpath)
        {
            this.answers = answers;
            folder = folderpath;
            SaveTask();
            SaveAnswer(clearA, clearB);

        }
        public void SaveTask()
        {
            string path = $"{folder}/task.pdf";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                Paragraph p1 = new Paragraph();
                p1.Alignment = Element.ALIGN_CENTER;
                p1.Add(new Chunk(("Завдання").ToUpper(), new Font(baseFont, 20, 1, BaseColor.BLACK)));
                p1.SpacingAfter = 20;
                doc.Add(p1);
                int variantNumber = 1;
                GraphBuilder builder;
                foreach (var single in answers)
                {
                    builder = new GraphBuilder(single.Task.Senders.Length, single.Task.Recievers.Length, single.Task.Restrictions.GetLength(0), single.Task.Restrictions.GetLength(1));
                    p1 = new Paragraph();
                    p1.Alignment = Element.ALIGN_LEFT;
                    p1.Add(new Chunk(("Варіант #" + variantNumber).ToUpper(), new Font(baseFont, 16, 1, BaseColor.BLACK)));
                    doc.Add(p1);
                    
                    PdfPTable forTable = new PdfPTable(2);
                    forTable.SpacingBefore = 10;

                    Paragraph pSenders = new Paragraph(new Chunk("Вихідні пункти", paragraphStyle)) { SpacingAfter = 20 };
                    PdfPTable tSender = new PdfPTable(2);
                    tSender.HorizontalAlignment = Element.ALIGN_CENTER;
                    for (int i = 0; i < single.Task.Senders.Length; i++)
                    {
                        tSender.AddCell(GetCell((Convert.ToChar(65).ToString() + (i + 1)).ToString()));
                        tSender.AddCell(GetCell(single.Task.Senders[i].ToString()));
                    }
                    
                    Paragraph pRecievers = new Paragraph(new Chunk(("Кінцеві пункти"), paragraphStyle)) { SpacingAfter = 20 };
                    PdfPTable tReciver= new PdfPTable(2);
                    tReciver.HorizontalAlignment = Element.ALIGN_CENTER;
                    for (int i = 0; i < single.Task.Recievers.Length; i++)
                    {
                        tReciver.AddCell(GetCell((Convert.ToChar(66).ToString() + (i + 1)).ToString()));
                        tReciver.AddCell(GetCell(single.Task.Recievers[i].ToString()));
                    }
                    forTable.AddCell(new PdfPCell(pSenders) { Border = 0, PaddingBottom = 10,HorizontalAlignment = Element.ALIGN_CENTER, PaddingRight = 10 });
                    forTable.AddCell(new PdfPCell(pRecievers) { Border = 0, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER, PaddingLeft = 10 });
                    forTable.AddCell(new PdfPCell(tSender) { Border = 0, PaddingRight = 10 });
                    forTable.AddCell(new PdfPCell(tReciver) { Border = 0,PaddingLeft = 10 });

                    doc.Add(forTable);
                    doc.Add(new Paragraph(new Chunk(("Список ребер:"), paragraphStyle)) { SpacingAfter = 20 });
                    
                    doc.Add(GetListOfEdges(single));

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
            string path = $"{folder}/answer.pdf";
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                Paragraph p1 = new Paragraph();
                p1.Alignment = Element.ALIGN_LEFT;
                p1.Add(new Chunk(("Розв'язки").ToUpper(), new Font(baseFont, 20, 1, BaseColor.BLACK)));
                p1.SpacingAfter = 20;
                doc.Add(p1);
                int variantNumber = 1;
                GraphBuilder builder;
                foreach (var single in answers)
                {
                    int buffer = single.Task.Senders.ToList().Sum();
                    builder = new GraphBuilder(single.Task.Senders.Length, single.Task.Recievers.Length, single.Task.Restrictions.GetLength(0), single.Task.Restrictions.GetLength(1));
                    p1 = new Paragraph();
                    p1.Alignment = Element.ALIGN_LEFT;
                    p1.Add(new Chunk(("Варіант #" + variantNumber).ToUpper(), new Font(baseFont, 16, 1, BaseColor.BLACK)));
                    p1.SpacingAfter = 10;

                    doc.Add(p1);

                    doc.Add(new Paragraph(new Chunk(($"Вхідна матриця ( Буфер : {single.Task.D.ToString()} )"), paragraphStyle)) { SpacingAfter = 20});
                    
                    doc.Add(GetTable(single,clearA,clearB,single.Task.D));
                
                    doc.Add(new Paragraph(new Chunk(("Вихідна матриця"), paragraphStyle)) { SpacingAfter = 20 });

                    doc.Add(GetTableRoads(single));

                    doc.Add(new Paragraph(new Chunk(("Z = " + single.Value), paragraphStyle)) { SpacingAfter = 20 });

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

        private PdfPTable GetListOfEdges(SolvedTask task)
        {
            PdfPTable table = new PdfPTable(2);
            int cellCount = 0;
            for (int i = 0; i < task.Task.Restrictions.GetLength(0); i++)
            {
                for (int j = 0; j < task.Task.Restrictions.GetLength(1); j++)
                {
                    if (task.Task.Restrictions[i, j] != 0 && task.Task.Restrictions[i, j] != 1000000 && i < j)
                    {
                        table.AddCell(GetCell(GetLabelForNode(i,task)+" -> "+GetLabelForNode(j, task)+": "+ task.Task.Restrictions[i, j]));
                        cellCount++;
                    }
                }
            }
            if (cellCount%2!=0) table.AddCell(GetCell(""));
            return table;
        }
        private PdfPCell GetCell(string text)
        {
            return new PdfPCell(new Phrase(text, cellStyle)) { PaddingTop = 10, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER };
        }
        private PdfPTable GetTable(SolvedTask task,int a,int b,int buffer)
        {

            PdfPTable table = new PdfPTable(task.GetColumnsToDraw().Count() + 2);
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(GetCell(" "));
            foreach (var item in task.GetColumnsToDraw())
            {
                table.AddCell(GetCell(GetLabelForNode(item, task)));
            }
           
            table.AddCell(GetCell(" "));
            var rows = task.GetRowsToDraw();
            var columns = task.GetColumnsToDraw();
            foreach (var i in rows)
            {
                table.AddCell(GetCell(GetLabelForNode(i, task)));
                foreach (var j in columns)
                {
                    if (task.BalancedMatrix[i, j] == 1000000)
                    {

                        table.AddCell(GetCell("M"));
                    }
                    else
                    {
                        table.AddCell(GetCell(task.BalancedMatrix[i, j].ToString()));
                    }
                }
                if (columns.Contains(i) || task.Senders[i] - buffer <= 0)
                    table.AddCell(GetCell(task.Senders[i].ToString()));
                else table.AddCell(GetCell((task.Senders[i] - buffer).ToString()));
            }
            table.AddCell(GetCell(" "));
            foreach (var i in columns)
            {
                if (rows.Contains(i) || task.Recievers[i] - buffer <= 0)
                    table.AddCell(GetCell(task.Recievers[i].ToString()));
                else table.AddCell(GetCell((task.Recievers[i] - buffer).ToString()));
            }
            table.AddCell(GetCell(" "));
            return table;
        }

        private PdfPTable GetTableRoads(SolvedTask task)
        {
            PdfPTable table = new PdfPTable(task.GetColumnsToDraw().Count() + 1);
            table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.DefaultCell.PaddingTop = 10;
            table.DefaultCell.PaddingBottom = 10;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(GetCell(" "));
            foreach (var i in task.GetColumnsToDraw())
            {
                table.AddCell(GetCell(GetLabelForNode(i, task)));
            }
            foreach (var i in task.GetRowsToDraw())
            {
                table.AddCell(GetCell(GetLabelForNode(i, task)));
                foreach (var j in task.GetColumnsToDraw())
                {
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
            int fict = task.Task.Restrictions.GetLength(0);

            if (number==fict ) return(Convert.ToChar(70).ToString()).ToString();
            //Senders
            else if (number < task.Task.Senders.Length) return (Convert.ToChar(65).ToString() + (number + 1)).ToString();
            //Intermediate
            else if (task.Task.Restrictions.GetLength(0) - task.Task.Recievers.Length <= number) return (Convert.ToChar(66).ToString() + (number + 1 - (task.Task.Restrictions.GetLength(0) - task.Task.Recievers.Length))).ToString();
            //Recievers
            else return (Convert.ToChar(67).ToString() + (number + 1 - task.Task.Senders.Length)).ToString();
            
        }
    }
}
