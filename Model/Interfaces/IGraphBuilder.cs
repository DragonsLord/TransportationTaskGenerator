using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TransportTasksGenerator.Model.Interfaces
{
    public interface IGraphBuilder
    {
        System.Drawing.Image Build(int[,] matrix);
    }
}
