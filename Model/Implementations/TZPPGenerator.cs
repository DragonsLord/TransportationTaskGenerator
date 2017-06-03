using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model.Interfaces;
using TransportTasksGenerator.Model;

namespace TransportTasksGenerator.Model.Implementations
{
    public class TZPPGenerator : ITaskGenerator
    {

        public IEnumerable<TransportationTask> Generate(GenerationParametrs parametrs)
        {
            var tasks = new List<TransportationTask>();

            for (int i = 0; i < parametrs.tasksAmount; i++)
            {
                var posts = GeneratePosts(parametrs.sendersAmount, parametrs.recieversAmount, parametrs.isBalanced, parametrs.postBound);
                int[,] c = GetRestrictions(parametrs);

                tasks.Add(new TransportationTask(posts.A, posts.B, c) { M = parametrs.M});
            }

            return tasks;
        }

        private dynamic GeneratePosts(int a_count, int b_count, bool is_balanced, Bound bound)
        {
            int[] a = new int[a_count];
            int[] b = new int[b_count];

            int d = 0;
            Random rand = new Random();

            for (int i = 0; i < a_count; i++)
            {
                a[i] = rand.Next(bound.From, bound.To);
                d += a[i];
            }

            for (int i = 0; i < b_count-1; i++)
            {
                b[i] = rand.Next(0, d - (b_count - i - 1));
                d -= b[i];
            }

            if (is_balanced)
                b[b_count - 1] = d;
            else b[b_count - 1] = rand.Next(bound.From, bound.To);

            return new { A = a, B = b };
        }

        private int[,] GetRestrictions(GenerationParametrs parametrs)
        {
            Random rand = new Random();
            int[,] c = new int[parametrs.totalAmount, parametrs.totalAmount];

            for (int i = 0; i < parametrs.totalAmount; i++)
            {
                for (int j = 0; j < parametrs.totalAmount; j++)
                {
                    int prob = rand.Next(0, 100);
                    if (prob < 80 || i < parametrs.sendersAmount || j > (parametrs.totalAmount - parametrs.recieversAmount)) // For senders and recievers always generate road
                        c[i, j] = rand.Next(parametrs.roadBound.From, parametrs.roadBound.To);
                    else c[i, j] = parametrs.M;
                }
            }
            for (int i = 0; i < parametrs.clearSendersAmount; i++)
            {
                for (int j = 0; j < parametrs.totalAmount; j++)
                    c[j, i] = parametrs.M;
            }
            for (int i = 0; i < parametrs.clearRecieversAmount; i++)
            {   
                for (int j = 0; j < parametrs.totalAmount; j++)
                    c[parametrs.totalAmount - 1 - i, j] = parametrs.M;
            }

            // if there are intermediate post forbid roads from A to B and also From B to A as unneccessery 
            if (parametrs.totalAmount > parametrs.recieversAmount + parametrs.sendersAmount)
            {
                for (int i = 0; i < parametrs.sendersAmount; i++)
                {
                    for (int j = parametrs.totalAmount - parametrs.recieversAmount; j < parametrs.totalAmount; j++)
                    {
                        if (i != j)
                        {
                            c[i, j] = parametrs.M;
                            c[j, i] = parametrs.M;
                        }
                    }
                }
            }
            for (int i = 0; i < parametrs.totalAmount; i++)
                c[i, i] = 0;

            return c;
        }
    }
}
