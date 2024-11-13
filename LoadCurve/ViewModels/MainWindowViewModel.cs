using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LoadCurve.Models;
using LoadCurve.Service;
using LoadCurve.Views;
using SkiaSharp;

namespace LoadCurve.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserControl _CurrentView;
    [ObservableProperty]
    private ObservableCollection<Server> _Servers;
    [ObservableProperty]
    private Server _SelectedServer;
    private SavingService _SavingService;
    public MainWindowViewModel()
    {
        _SavingService = new();
        Servers = new();
        CurrentView = new();
    }
    [RelayCommand]
    private void OnAddServer() 
    {
        AddNewServerWindow window = new();
        AddNewServerWindowViewModel data = ((AddNewServerWindowViewModel)window.DataContext);
        data.AddNewServerHandler += (server) => { Servers.Add(server); };
        window.Show();
    }
    [RelayCommand]
    private void OnDeleteServer()
    {
        Servers.Remove(SelectedServer);
    }
    [RelayCommand]
    private void OnChangeServer() 
    {
        CurrentView = new CurveView();
        ((CurveViewModel)CurrentView.DataContext).CurrentServer = SelectedServer;

    }
    [RelayCommand]
    private void OnClosing() 
    {
       _SavingService.SaveServer(Servers.ToList());
    }
    [RelayCommand]
    private void OnLoaded() 
    {
        Servers = [.. _SavingService.LoadServers()];
    }
}