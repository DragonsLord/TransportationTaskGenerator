using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model.Interfaces;
using TransportTasksGenerator.Model.Implementations;

namespace TransportTasksGenerator.Model
{
    public class Generator
    {
        public ITaskGenerator TaskGenerator { get; set; } = new TZPPGenerator();
        public IGraphBuilder GraphBuider { get; set; }
        public ITaskSolver TaskSolver { get; set; } = new TZPPSolver();
        public ISaver Saver { get; set; }

        public void Generate(GenerationParametrs gen_params, string filepath)
        {
            var tasks = TaskGenerator.Generate(gen_params);
            List<SolvedTask> answers = new List<SolvedTask>();
            foreach (var task in tasks)
            {
                Task.Run(() => task.Graph = GraphBuider.Build(task.Restrictions));
                var result = TaskSolver.Solve(task);
                Task.Run(() => result.Graph = GraphBuider.Build(result.Roads));
            }
            Saver.Save(tasks, answers);
        }
    }
}
