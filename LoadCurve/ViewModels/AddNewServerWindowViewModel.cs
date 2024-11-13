using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoadCurve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadCurve.ViewModels
{
    internal partial class AddNewServerWindowViewModel: ViewModelBase
    {
        public delegate void AddNewServer(Server server);
        public AddNewServer AddNewServerHandler { get; set; }
        [ObservableProperty]
        private Server _NewServer;
        public AddNewServerWindowViewModel() 
        {
            NewServer = new();
        }
        [RelayCommand]
        private void OnConnect(Window window)
        {
            AddNewServerHandler.Invoke(NewServer);
            window.Close();
        }
        [RelayCommand]
        private void OnCancel(Window window)
        {
            window.Close();
        }
    }
}
