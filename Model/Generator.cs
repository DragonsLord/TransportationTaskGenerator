using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model
{
    public class Generator
    {
        public ITaskGenerator TaskGenerator { get; set; }
        public IGraphBuilder GraphBuider { get; set; }
        public ITaskSolver TaskSolver { get; set; }

        public void Generate(GenerationParametrs gen_params, string filepath)
        {
            var tasks = TaskGenerator.Generate(gen_params);
            foreach (var task in tasks)
            {
                var result = TaskSolver.Solve(task);
            }
        }
    }
}
