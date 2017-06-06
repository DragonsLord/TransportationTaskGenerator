using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportTasksGenerator.Model;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model.Implementations
{
    public class TZPPSolver : ITaskSolver
    {
        class Matrix
        {
            public int[,] C;
            public int[,] Values;
            public List<int> SenderBounds;
            public List<int> RecieverBounds;
            public List<Position> X = new List<Position>();
        }

        struct Position
        {
            public int i;
            public int j;
        }

        private Matrix m;
        private void SetBounds(TransportationTask task)
        {
            int d;
            int d_a = task.Senders.Sum();
            int d_b = task.Recievers.Sum();
            List<int> senders = new List<int>(task.Senders);
            List<int> recievers = new List<int>(task.Recievers);
            if (d_b > d_a)
            {
                d = d_b;
                int[,] c = new int[m.C.GetLength(0) + 1, m.C.GetLength(1)];
                for (int j = 0; j < m.C.GetLength(1); j++)
                {
                    for (int i = 0; i < m.C.GetLength(0); i++)
                    {
                        c[i, j] = m.C[i, j];
                    }
                    c[m.C.GetLength(0), j] = 0;
                }
                m.C = c;
                m.SenderBounds = Enumerable.Repeat<int>(d, m.C.GetLength(0)).ToList();
                m.RecieverBounds = Enumerable.Repeat<int>(d, m.C.GetLength(1)).ToList();
                for (int i = 0; i < task.Senders.Length; i++)
                    m.SenderBounds[i] += task.Senders[i];
                m.SenderBounds[m.SenderBounds.Count - 1] = d_b - d_a;
                for (int i = 0; i < task.Recievers.Length; i++)
                    m.RecieverBounds[m.RecieverBounds.Count - task.Recievers.Length + i] += task.Recievers[i];
            }
            else if (d_a > d_b)
            {
                d = d_a;
                int[,] c = new int[m.C.GetLength(0), m.C.GetLength(1) + 1];
                for (int i = 0; i < m.C.GetLength(0); i++)
                {
                    for (int j = 0; j < m.C.GetLength(1); j++)
                    {
                        c[i, j] = m.C[i, j];
                    }
                    c[i, m.C.GetLength(1)] = 0;
                }
                m.C = c;
                m.SenderBounds = Enumerable.Repeat<int>(d, m.C.GetLength(0)).ToList();
                m.RecieverBounds = Enumerable.Repeat<int>(d, m.C.GetLength(1)).ToList();
                for (int i = 0; i < task.Senders.Length; i++)
                    m.SenderBounds[i] += task.Senders[i];
                for (int i = 0; i < task.Recievers.Length; i++)
                    m.RecieverBounds[m.RecieverBounds.Count - task.Recievers.Length - 1 + i] += task.Recievers[i];
                m.RecieverBounds[m.RecieverBounds.Count - 1] = d_a - d_b;
            }
            else
            {
                d = d_a;
                m.SenderBounds = Enumerable.Repeat<int>(d, m.C.GetLength(0)).ToList();
                m.RecieverBounds = Enumerable.Repeat<int>(d, m.C.GetLength(1)).ToList();
                for (int i = 0; i < task.Senders.Length; i++)
                    m.SenderBounds[i] += task.Senders[i];
                for (int i = 0; i < task.Recievers.Length; i++)
                    m.RecieverBounds[m.RecieverBounds.Count - task.Recievers.Length + i] += task.Recievers[i];
            }

            task.D = d;
        }

        private void CalcFirstIteration()
        {
            int[] senders = m.SenderBounds.ToArray();
            int[] recievers = m.RecieverBounds.ToArray();
            int[,] limits = m.C.Clone() as int[,];
            List<int> rows = Enumerable.Range(0, m.C.GetLength(0)).ToList();
            List<int> columns = Enumerable.Range(0, m.C.GetLength(1)).ToList();
            Position GetMin()
            {
                Position p = new Position();
                int min = int.MaxValue;
                foreach(var i in rows)
                {
                    foreach(var j in columns)
                    {
                        if (limits[i,j] < min)
                        {
                            p.i = i;
                            p.j = j;
                            min = limits[i, j];
                        }
                    }
                }
                limits[p.i, p.j] = int.MaxValue;
                return p;
            }
            int N = m.C.GetLength(0) + m.C.GetLength(1) - 1;
            for (int i = 0; i < N; i++)
            {
                var pos = GetMin();
                m.X.Add(pos);
                int min;
                if (senders[pos.i] > recievers[pos.j])
                {
                    min = recievers[pos.j];
                    columns.Remove(pos.j);
                }
                else if (senders[pos.i] < recievers[pos.j])
                {
                    min = senders[pos.i];
                    rows.Remove(pos.i);
                }
                else {
                    min = senders[pos.i];
                    if (columns.Count > rows.Count)
                        columns.Remove(pos.j);
                    else rows.Remove(pos.i);
                }
                m.Values[pos.i, pos.j] = min;
                senders[pos.i] -= min;
                recievers[pos.j] -= min;
            }
        }

        private void PotencialMethod()
        {
            
            Dictionary<Position, int> n = new Dictionary<Position, int>();
            //List<Position> x_pos = new List<Position>();
            for (int i = 0; i < m.Values.GetLength(0); i++)
                for (int j = 0; j < m.Values.GetLength(1); j++)
                {
                    Position p = new Position() { i = i, j = j };
                    if (!m.X.Contains(p))
                        n.Add(p, 0);
                }
            bool end = false;
            do
            {
                #region Potencials
                int?[] u = Enumerable.Repeat<int?>(null, m.C.GetLength(0)).ToArray();
                int?[] v = Enumerable.Repeat<int?>(null, m.C.GetLength(1)).ToArray();
                u[0] = 0;
                Queue<Position> queue = new Queue<Position>();
                queue.Enqueue(new Position() { i = 0, j = -1 });
                while (queue.Count != 0)
                {
                    var p = queue.Dequeue();
                    if (p.i != -1)
                    {
                        for (int j = 0; j < m.C.GetLength(1); j++)
                        {
                            if ( m.X.Exists(pos => pos.i == p.i && pos.j == j) && v[j] == null)
                            {
                                v[j] = m.C[p.i, j] - u[p.i];
                                queue.Enqueue(new Position() { i = -1, j = j });
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m.C.GetLength(0); i++)
                        {
                            if (m.X.Exists(pos => pos.i == i && pos.j == p.j) && u[i] == null)
                            {
                                u[i] = m.C[i, p.j] - v[p.j];
                                queue.Enqueue(new Position() { i = i, j = -1 });
                            }
                        }
                    }
                }
                #endregion

                for (int i = 0; i < n.Count; i++)
                {
                    var key = n.Keys.ElementAt(i);
                    n[key] = u[key.i].Value + v[key.j].Value - m.C[key.i, key.j];
                }

                var max_z = n.Aggregate((l,r) => l.Value > r.Value ? l : r); // get max y value of Xn
                if (max_z.Value <= 0)
                    end = true;
                else
                {
                    m.X.Add(max_z.Key);
                    var poss = m.X.ToList();
                    #region Cycle

                    #region RemoveSingles
                    int[] rows = Enumerable.Repeat(0, m.C.GetLength(0)).ToArray();
                    int[] columns = Enumerable.Repeat(0, m.C.GetLength(1)).ToArray();
                    foreach (var pos in poss)
                    {
                        rows[pos.i]++;
                        columns[pos.j]++;
                    }
                    bool delete = true;
                    while (delete) {
                        delete = false;
                        for (int i = 0; i < rows.Length; i++)
                        {
                            if (rows[i] == 1)
                            {
                                var p = poss.Find(item => item.i == i);
                                columns[p.j]--;
                                rows[p.i]--;
                                poss.Remove(p);
                                delete = true;
                            }
                        }
                        for (int j = 0; j < columns.Length; j++)
                        {
                            if (columns[j] == 1)
                            {
                                var p = poss.Find(item => item.j == j);
                                rows[p.i]--;
                                columns[p.j]--;
                                poss.Remove(p);
                                delete = true;
                            }
                        }
                    }
                    #endregion
                    #region Apply changes to cycle
                    var plus = new List<Position>();
                    var minus = new List<Position>();
                    int curr_column = max_z.Key.j, curr_row = max_z.Key.i, min_x = int.MaxValue;
                    Position min_x_pos = max_z.Key;
                    do
                    {
                        var p_r = poss.Find(p => p.i == curr_row && p.j != curr_column);
                        minus.Add(p_r);
                        if (m.Values[p_r.i,p_r.j] < min_x)
                        {
                            min_x = m.Values[p_r.i, p_r.j];
                            min_x_pos = p_r;
                        }
                        var p_c = poss.Find(p => p.i != p_r.i && p.j == p_r.j);
                        plus.Add(p_c);
                        curr_column = p_c.j;
                        curr_row = p_c.i;
                    } while (curr_column != max_z.Key.j);

                    for (int k = 0; k < plus.Count; k++)
                    {
                        m.Values[plus[k].i, plus[k].j] += min_x;
                        m.Values[minus[k].i, minus[k].j] -= min_x;
                    }

                    m.X.Remove(min_x_pos);
                    #endregion

                    #endregion
                }
            } while (!end);
        }
        public SolvedTask Solve(TransportationTask task)
        {
            m = new Matrix();
            m.C = task.Restrictions;
            SetBounds(task);
            m.Values = new int[m.C.GetLength(0), m.C.GetLength(1)];
            for (int i = 0; i < m.C.GetLength(0); i++)
                for (int j = 0; j < m.C.GetLength(1); j++)
                    m.Values[i, j] = 0;

            CalcFirstIteration();
            PotencialMethod();
            return new SolvedTask(task, m.Values) { BalancedMatrix = m.C, Recievers = m.RecieverBounds.ToArray(), Senders = m.SenderBounds.ToArray()};
        }
    }
}
