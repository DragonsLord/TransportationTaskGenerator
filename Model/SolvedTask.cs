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
    }
}
