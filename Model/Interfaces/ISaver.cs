using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model;

namespace TransportTasksGenerator.Model.Interfaces
{
    public interface ISaver
    {
        void Save(IEnumerable<TransportationTask> tasks, IEnumerable<SolvedTask> answers);
    }
}
