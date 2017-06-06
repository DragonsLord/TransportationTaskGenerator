using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TransportTasksGenerator.Model
{
    public class SolvedTask
    {
        public TransportationTask Task { get; private set; }
        public int Value { get; private set; } = 0;
        private int[,] roads;
        public int[,] Roads => roads;
        public int[,] BalancedMatrix { get; set; }
        public int[] Senders { get; set; }
        public int[] Recievers { get; set; }

        public SolvedTask(TransportationTask task, int[,] answer)
        {
            Task = task;
            roads = answer;

            for (int i = 0; i < task.Restrictions.GetLength(0); i++)
            {
                for (int j = 0; j < task.Restrictions.GetLength(1); j++)
                {
                    if (i!=j) Value += answer[i, j] * task.Restrictions[i, j];
                }
            }
        }


        public IEnumerable<int> GetColumnsToDraw()
        {
            var r = BalancedMatrix.GetLength(0) - BalancedMatrix.GetLength(1);
            var columns = new List<int>();
            for (int j = 0; j < BalancedMatrix.GetLength(1); j++)
            {
                bool add = false;
                for (int i = 0; i < BalancedMatrix.GetLength(0); i++)
                {
                    if (BalancedMatrix[i, j] != Task.M && i != j && i < BalancedMatrix.GetLength(0) - r)
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
            var r = BalancedMatrix.GetLength(1) - BalancedMatrix.GetLength(0);
            var rows = new List<int>();
            for (int i = 0; i < BalancedMatrix.GetLength(0); i++)
            {
                bool add = false;
                for (int j = 0; j < BalancedMatrix.GetLength(1); j++)
                {
                    if (BalancedMatrix[i, j] != Task.M && i != j && j < BalancedMatrix.GetLength(1) - r)
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
