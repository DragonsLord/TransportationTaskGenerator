using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TransportTasksGenerator.Model;
using System.ComponentModel;

namespace TransportTasksGenerator.ViewModel
{
    public class GeneratorViewModel : ViewModelBase, IDataErrorInfo
    {
        private Generator _generator = new Generator();
        private GenerationParametrs _parameters = new GenerationParametrs();

        public int TotalAmount
        {
            get => _parameters.totalAmount;
            set
            {
                Set<int>(() => this.TotalAmount, ref _parameters.totalAmount, value);
                RaisePropertyChanged("SendersAmount");
                RaisePropertyChanged("RecieversAmount");
            }
        }
        public int SendersAmount
        {
            get => _parameters.sendersAmount;
            set {
                Set<int>(() => this.SendersAmount, ref _parameters.sendersAmount, value);
                RaisePropertyChanged("TotalAmount");
                RaisePropertyChanged("RecieversAmount");
                RaisePropertyChanged("ClearSendersAmount");
            }
        }
        public int RecieversAmount
        {
            get => _parameters.recieversAmount;
            set {
                Set<int>(() => this.RecieversAmount, ref _parameters.recieversAmount, value);
                RaisePropertyChanged("TotalAmount");
                RaisePropertyChanged("SendersAmount");
                RaisePropertyChanged("ClearRecieversAmount");
            }
        }
        public int ClearSendersAmount
        {
            get => _parameters.clearSendersAmount;
            set {
                Set<int>(() => this.ClearSendersAmount, ref _parameters.clearSendersAmount, value);
            }
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
            set {
                Set<int>(() => this.FromPostBound, ref _parameters.postBound.From, value);
                RaisePropertyChanged("ToPostBound");
            }
        }
        public int ToPostBound
        {
            get => _parameters.postBound.To;
            set {
                Set<int>(() => this.ToPostBound, ref _parameters.postBound.To, value);
                RaisePropertyChanged("FromPostBound");
            }
        }
        public int FromRoadBound
        {
            get => _parameters.roadBound.From;
            set {
                Set<int>(() => this.FromRoadBound, ref _parameters.roadBound.From, value);
                RaisePropertyChanged("ToRoadBound");
            }
        }
        public int ToRoadBound
        {
            get => _parameters.roadBound.To;
            set {
                Set<int>(() => this.ToRoadBound, ref _parameters.roadBound.To, value);
                RaisePropertyChanged("FromRoadBound");
            }
        }

        public ICommand GenerateCommand { get; set; }

        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                Error = String.Empty;
                switch (columnName)
                {
                    case "TotalAmount":
                        if (TotalAmount < (SendersAmount + RecieversAmount) || TotalAmount <= 0)
                            Error = "Значення не може бути меншим за суму вихідних та пунктів призначення";
                        break;
                    case "SendersAmount":
                        if (SendersAmount <= 0 || SendersAmount > TotalAmount - RecieversAmount)
                            Error = "Повинно бути додатнє значення та не більше загальної к-сті пунктів";
                        break;
                    case "RecieversAmount":
                        if (RecieversAmount <= 0 || RecieversAmount > TotalAmount - SendersAmount)
                            Error = "Повинно бути додатнє значення та не більше загальної к-сті пунктів";
                        break;
                    case "ClearSendersAmount":
                        if (ClearSendersAmount <= 0 || ClearSendersAmount > SendersAmount)
                            Error = "Значення не може перевищуваати к-сть вихідних пунктів";
                        break;
                    case "ClearRecieversAmount":
                        if (ClearRecieversAmount <= 0 || ClearRecieversAmount > RecieversAmount)
                            Error = "Значення не може перевищуваати к-сть вхідних пунктів";
                        break;
                    case "FromPostBound":
                        if (FromPostBound >= ToPostBound)
                            Error = "Значення Від повинно буте менше значення До";
                        break;
                    case "ToPostBound":
                        if (ToPostBound <= FromPostBound)
                            Error = "Значення До повинно бути більше значення Від";
                        break;
                    case "FromRoadBound":
                        if (FromRoadBound >= ToRoadBound)
                            Error = "Значення Від повинно буте менше значення До";
                        break;
                    case "ToRoadBound":
                        if (ToRoadBound <= FromRoadBound)
                            Error = "Значення До повинно бути більше значення Від";
                        break;
                    case "TasksAmount":
                        if (TasksAmount > 100 || TasksAmount <= 0)
                            Error = "К-сть завдань повинна бути > 0 і <= 100";
                        break;
                }
                return Error;
            }
        }

        private async void Generate()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    await Task.Run(() => _generator.Generate(_parameters, dialog.SelectedPath));
            }
        }
        private bool InputCheck()
        {
            //bool c1, c2, c3, c0;
            //c1 = TotalAmount >= (SendersAmount + RecieversAmount) &&
            //    SendersAmount >= ClearSendersAmount && RecieversAmount >= ClearRecieversAmount;
            //c2 = FromPostBound < ToPostBound && FromRoadBound < ToRoadBound;
            //c3 = TasksAmount <= 100;
            //c0 = TotalAmount > 0 && SendersAmount > 0 && RecieversAmount > 0
            //    && ClearRecieversAmount > 0 && ClearSendersAmount > 0 && TasksAmount > 0;
            return String.IsNullOrEmpty(Error);
        }
        public GeneratorViewModel()
        {
            GenerateCommand = new RelayCommand(Generate, InputCheck);
        }
    }
}
