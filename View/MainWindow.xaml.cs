using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransportTasksGenerator.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace TransportTasksGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, m => MessageBox.Show(m));
            Messenger.Default.Register<WorkerStatus>(this, s =>
            {
                if (s == WorkerStatus.Working)
                    Cursor = Cursors.Wait;
                else Cursor = Cursors.Arrow;
            });
        }
    }
}
