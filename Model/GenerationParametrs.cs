using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportTasksGenerator.Model
{
    public struct Bound
    {
        public int From;
        public int To;
    }
    public class GenerationParametrs
    {
        public int totalAmount = 12;
        public int sendersAmount = 3;
        public int recieversAmount = 3;
        public int clearSendersAmount = 0;
        public int clearRecieversAmount = 0;

        public int M = 1000000;
        public int tasksAmount = 1;
        public bool isBalanced = true;
        public Bound postBound = new Bound() { From = 10, To = 100 };
        public Bound roadBound = new Bound() { From = 1, To = 50 };
    }
}
