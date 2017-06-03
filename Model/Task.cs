using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportTasksGenerator.Model
{
    public class TransportationTask
    {
        private int[] _a;
        private int[] _b;
        private int[,] _restrictions;

        public int[] Senders => _a;
        public int[] Recievers => _b;
        public int[,] Restrictions => _restrictions;
        public int M { get; set; }
        public int D { get; set; }

        public TransportationTask(int[] senders, int[] recievers, int[,] restricts)
        {
            _a = senders;
            _b = recievers;
            _restrictions = restricts;
        }

        public IEnumerable<int> GetColumnsToDraw()
        {
            var columns = new List<int>();
            for (int j = 0; j < _restrictions.GetLength(1); j++)
            {
                bool add = false;
                for (int i = 0; i < _restrictions.GetLength(0); i++)
                {
                    if (_restrictions[i, j] != M && i != j)
                    {
                        add = true;
                        break;
                    }
                }
                if (add)
                    columns.Add(j);
            }
            return columns;
        }

        public IEnumerable<int> GetRowsToDraw()
        {
            var rows = new List<int>();
            for (int i = 0; i < _restrictions.GetLength(0); i++)
            {
                bool add = false;
                for (int j = 0; j < _restrictions.GetLength(1); j++)
                {
                    if (_restrictions[i, j] != M && i != j)
                    {
                        add = true;
                        break;
                    }
                }
                if (add)
                    rows.Add(i);
            }
            return rows;
        }
    }
}
