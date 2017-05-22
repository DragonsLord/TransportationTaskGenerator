using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TransportTasksGenerator.Model;

namespace TransportTasksGenerator.ViewModel
{
    public class GeneratorViewModel : ViewModelBase
    {
        private Generator _generator = new Generator();
        private GenerationParametrs _parameters = new GenerationParametrs();

        public int TotalAmount
        {
            get => _parameters.totalAmount;
            set => Set<int>(() => this.TotalAmount, ref _parameters.totalAmount, value);
        }
        public int SendersAmount
        {
            get => _parameters.sendersAmount;
            set => Set<int>(() => this.SendersAmount, ref _parameters.sendersAmount, value);
        }
        public int RecieversAmount
        {
            get => _parameters.recieversAmount;
            set => Set<int>(() => this.RecieversAmount, ref _parameters.recieversAmount, value);
        }
        public int ClearSendersAmount
        {
            get => _parameters.clearSendersAmount;
            set => Set<int>(() => this.ClearSendersAmount, ref _parameters.clearSendersAmount, value);
        }
        public int ClearRecieversAmount
        {
            get => _parameters.clearRecieversAmount;
            set => Set<int>(() => this.ClearRecieversAmount, ref _parameters.clearRecieversAmount, value);
        }

        public int TasksAmount
        {
            get => _parameters.tasksAmount;
            set => Set<int>(() => this.TasksAmount, ref _parameters.tasksAmount, value);
        }
        public bool IsBalanced
        {
            get => _parameters.isBalanced;
            set => Set<bool>(() => this.IsBalanced, ref _parameters.isBalanced, value);
        }
        public int FromPostBound
        {
            get => _parameters.postBound.From;
            set => Set<int>(() => this.FromPostBound, ref _parameters.postBound.From, value);
        }
        public int ToPostBound
        {
            get => _parameters.postBound.To;
            set => Set<int>(() => this.ToPostBound, ref _parameters.postBound.To, value);
        }
        public int FromRoadBound
        {
            get => _parameters.roadBound.From;
            set => Set<int>(() => this.FromRoadBound, ref _parameters.roadBound.From, value);
        }
        public int ToRoadBound
        {
            get => _parameters.roadBound.To;
            set => Set<int>(() => this.ToRoadBound, ref _parameters.roadBound.To, value);
        }

        public ICommand GenerateCommand { get; set; }

        private async void Generate()
        {
            await Task.Run(() => _generator.Generate(_parameters, "test.pdf"));
        }
        private bool InputCheck()
        {
            return true;
        }
        public GeneratorViewModel()
        {
            GenerateCommand = new RelayCommand(Generate, InputCheck);
        }
    }
}
