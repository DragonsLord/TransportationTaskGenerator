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
    }
}
