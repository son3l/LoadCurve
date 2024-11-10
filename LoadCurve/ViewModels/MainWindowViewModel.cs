using CommunityToolkit.Mvvm.Input;
using LoadCurve.Models;
using LoadCurve.Service;

namespace LoadCurve.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //TODO 
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
    [RelayCommand]
    private void OnConnect()
    {
        SSHService service = new(new Server() { Address="91.230.211.118",Password="6bgH2j5hK88k",UserName="root"});
        service.GetServerLoadInfo();
    }
}