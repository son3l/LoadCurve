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
        if (SelectedServer is null)
        {
            new MessageWindow("Нет выбранного сервера", "Ошибка").Show();
            return;
        }
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
    [RelayCommand]
    private void OnOpenConfigWindow()
    {
        if (SelectedServer is null)
        {
            new MessageWindow("Нет выбранного сервера", "Ошибка").Show();
            return;
        }
        ServerConfigWindow window = new();
        ServerConfigWindowViewModel data = (ServerConfigWindowViewModel)window.DataContext;
        data.Server = SelectedServer;
        window.Show();
    }
    [RelayCommand]
    private void OnExit()
    {
        Environment.Exit(0);
    }
    [RelayCommand]
    private void OnAddWidget()
    {
        if (SelectedServer is null)
        {
            new MessageWindow("Нет выбранного сервера","Ошибка").Show();
            return;
        }
        if (App.WidgetWindow is not null)
        {
            WidgetWindowViewModel data =((WidgetWindowViewModel)App.WidgetWindow.DataContext);
            data.CPUPoints.Clear();
            data.MemoryPoints.Clear();
            foreach (var point in ((CurveViewModel)CurrentView.DataContext).CPUPoints)
            {
                data.CPUPoints.Add(point);
            }
            foreach (var point in ((CurveViewModel)CurrentView.DataContext).MemoryPoints)
            {
                data.MemoryPoints.Add(point);
            }
            data.CurrentServer = ((CurveViewModel)CurrentView.DataContext).CurrentServer;
            data.SSHService = new(data.CurrentServer);
        }
        else
        {
            WidgetWindow window = new();
            foreach (var point in ((CurveViewModel)CurrentView.DataContext).CPUPoints)
            {
                ((WidgetWindowViewModel)window.DataContext).CPUPoints.Add(point);
            }
            foreach (var point in ((CurveViewModel)CurrentView.DataContext).MemoryPoints)
            {
                ((WidgetWindowViewModel)window.DataContext).MemoryPoints.Add(point);
            }
            ((WidgetWindowViewModel)window.DataContext).CurrentServer = ((CurveViewModel)CurrentView.DataContext).CurrentServer;
            window.Show();
            App.WidgetWindow = window;
        }
    }
}