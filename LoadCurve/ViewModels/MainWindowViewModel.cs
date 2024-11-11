using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LoadCurve.Models;
using LoadCurve.Service;

namespace LoadCurve.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ISeries[] _Series;
    [ObservableProperty]
    private ICartesianAxis[] _XAxes;
private SSHService _SSHService;
    public MainWindowViewModel()
    {
        _SSHService = new(new Server() { Address="91.230.211.118",Password="6bgH2j5hK88k",UserName="root"});
        ServerLoadInfo info = _SSHService.GetServerLoadInfo();
        Series = [
            new LineSeries<DateTimePoint>
            {
                Values = []
            }
        ];
        ((LineSeries<DateTimePoint>)Series[0]).Values.Add(new DateTimePoint() { DateTime = DateTime.Now, Value = info.TotalCPUUsage });
        XAxes = [new DateTimeAxis(TimeSpan.FromSeconds(5), date => date.ToString("hh:mm:ss"))];
    }

    [RelayCommand]
    private void OnConnect()
    {
        Dispatcher.UIThread.InvokeAsync( async() =>
        {
            while (true)
            {
                ServerLoadInfo info = _SSHService.GetServerLoadInfo();
                ((LineSeries<DateTimePoint>)Series[0]).Values.Add(new DateTimePoint()
                    { DateTime = DateTime.Now, Value = info.TotalCPUUsage });
                await Task.Delay(5000);
            }
        });
    }
}