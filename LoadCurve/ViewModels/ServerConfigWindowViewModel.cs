using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoadCurve.Models;
using LoadCurve.Models.ServerConfigModels;
using LoadCurve.Service;

namespace LoadCurve.ViewModels
{
    internal partial class ServerConfigWindowViewModel : ViewModelBase
    {
        public Server Server { get; set; }
        private SSHService Service { get; set; }
        [ObservableProperty]
        private ServerConfigInfo _Info;
        public ServerConfigWindowViewModel()
        {
            Info = new();
            Service = new();
        }

        [RelayCommand]
        private void OnLoaded()
        {
            Info = Service.GetServerConfig(Server);
        }
    }
}
