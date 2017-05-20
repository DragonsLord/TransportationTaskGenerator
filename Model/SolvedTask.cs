using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportTasksGenerator.Model
{
    public class SolvedTask
    {
        public int Value { get; set; }
        private int[,] roads;
        public int[,] Roads => roads;
    }
}
