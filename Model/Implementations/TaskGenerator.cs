using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TransportTasksGenerator.Model.Interfaces;

namespace TransportTasksGenerator.Model.Implementations
{
    class TaskGenerator : ITaskGenerator
    {
        private int[,] matrix;
        private Dictionary<int, int> SendersID = new Dictionary<int, int>();
        private Dictionary<int, int> ReceiveID = new Dictionary<int, int>();
        private int postFrom;
        private int postTo;
        private int totalCount;

        public IEnumerable<TransportationTask> Generate(GenerationParametrs parametrs)
        {
            postFrom = parametrs.postBound.From;
            postTo = parametrs.postBound.To;
            totalCount = parametrs.totalAmount;
            Random rd = new Random();
            matrix = new int[totalCount, totalCount];

            GenerateSenders(parametrs.sendersAmount);

            GenerateReciever(parametrs.recieversAmount, parametrs.isBalanced);

            //generate matrix 
            for (int i = 0; i < totalCount; i++)
            {
                for (int j = 0; j < totalCount; j++)
                {
                    matrix[i, j] = rd.Next(parametrs.roadBound.From, parametrs.roadBound.To);
                }
            }

            //make Senders and Recievers
            foreach (var KeyValuePair in SendersID)
            {

                for (int i = 0; i < totalCount; i++)
                {

                }
            }
            return new List<TransportationTask>();
        }
        private void GenerateSenders(int count)
        {
            int number;
            Random rd = new Random();
            while (SendersID.Count!=count)
            {
                number = rd.Next(0, totalCount);
                if (!ReceiveID.ContainsKey(number))
                    ReceiveID.Add(number, rd.Next(postFrom, postTo));
            }
        }
        private void GenerateReciever(int count,bool balance)
        {
            int number;
            Random rd = new Random();
            while (ReceiveID.Count != count)
            {
                number = rd.Next(0, totalCount);
                if (!ReceiveID.ContainsKey(number))
                    ReceiveID.Add(number, rd.Next(postFrom, postTo));

                if (balance && ReceiveID.Count == count - 1)
                {
                    int diff = SendersID.Sum(t => t.Value) - ReceiveID.Sum(t => t.Value);
                    if (diff>0)
                    {
                        while (ReceiveID.Count != count)
                        {
                            number = rd.Next(0, totalCount);
                            if (!ReceiveID.ContainsKey(number))
                                ReceiveID.Add(number, diff);
                        }
                    }
                    else
                    {
                        int cost = rd.Next(0, totalCount);
                        int keyMinValue = SendersID.First(t => t.Value == SendersID.Min(k => k.Value)).Key;
                        SendersID[keyMinValue] += (Math.Abs(diff) + cost);
                        
                        while (ReceiveID.Count != count)
                        {
                            number = rd.Next(0, totalCount);
                            if (!ReceiveID.ContainsKey(number))
                                ReceiveID.Add(number, cost);
                        }
                    }
                }
            }
        }
    }
}