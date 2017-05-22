using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model.Implementations;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model
{
    public class Generator
    {
        public ITaskGenerator TaskGenerator { get; set; } = new TZPPGenerator();
        public IGraphBuilder GraphBuider { get; set; } = new GraphBuilder();
        public ITaskSolver TaskSolver { get; set; } = new TZPPSolver();
        public ISaver Saver { get; set; }

        public void Generate(GenerationParametrs gen_params, string filepath)
        {
            var tasks = TaskGenerator.Generate(gen_params);
            List<SolvedTask> answers = new List<SolvedTask>();
            foreach (var task in tasks)
            {
                Task.Run(() => GraphBuider.Build(task.Restrictions, "task"));
                var result = TaskSolver.Solve(task);
                Task.Run(() => GraphBuider.Build(result.Roads, "result"));
            }
            //Saver.Save(tasks, answers);
        }
    }
}
