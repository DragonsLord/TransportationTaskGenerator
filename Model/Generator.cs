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
        public ITaskSolver TaskSolver { get; set; } = new TZPPSolver();
        public ISaver Saver { get; set; } = new PdfSaver();

        public void Generate(GenerationParametrs gen_params, string path)
        {
            var tasks = TaskGenerator.Generate(gen_params);
            List<SolvedTask> answers = new List<SolvedTask>();
            foreach (var task in tasks)
            {
                answers.Add(TaskSolver.Solve(task));
            }
            Saver.Save(answers,gen_params.clearRecieversAmount,gen_params.clearSendersAmount, path);
        }
    }
}
