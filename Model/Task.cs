using System;
using System.Collections.Generic;
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

        public int[] A => _a;
        public int[] B => _b;
        public int[,] Restrictions => _restrictions;

    }
}
