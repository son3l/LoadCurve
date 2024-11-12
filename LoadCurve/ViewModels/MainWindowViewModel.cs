using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
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
using SkiaSharp;

namespace LoadCurve.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly SKColor s_gray = new(195, 195, 195);
    private static readonly SKColor s_gray1 = new(160, 160, 160);
    private static readonly SKColor s_gray2 = new(90, 90, 90);
    private static readonly SKColor s_dark3 = new(60, 60, 60);
    [ObservableProperty]
    private ObservableCollection<ISeries> _Series;
    [ObservableProperty]
    private ObservableCollection<ICartesianAxis> _XAxes;
    [ObservableProperty]
    private ObservableCollection<DateTimePoint> _Points;
    [ObservableProperty]
    private ObservableCollection<ICartesianAxis> _YAxes;
    [ObservableProperty]
    private DrawMarginFrame _Frame;
    [ObservableProperty]
    private ObservableCollection<ServerProcess> _Processes;
    private SSHService _SSHService;
    public MainWindowViewModel()
    {
        _SSHService = new(new Server() { Address="91.230.211.118",Password="6bgH2j5hK88k",UserName="root"});
        Points = new();
        Processes = [.._SSHService.GetServerLoadInfo().Processes];

        #region Настройка графика
        Series = [new LineSeries<DateTimePoint>(Points)
        {
            Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
            Fill = null,
            GeometrySize = 0
        }];
        XAxes = [new DateTimeAxis(TimeSpan.FromSeconds(5), date => date.ToString("hh:mm:ss"))
        {
            Name="Время",
             NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 18,
            Padding = new Padding(5, 15, 5, 5),
            LabelsPaint = new SolidColorPaint(s_gray),
            SeparatorsPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1,
                PathEffect = new DashEffect([3, 3])
            },
            SubseparatorsPaint = new SolidColorPaint
            {
                Color = s_gray2,
                StrokeThickness = 0.5f
            },
            SubseparatorsCount = 9,
            ZeroPaint = new SolidColorPaint
            {
                Color = s_gray1,
                StrokeThickness = 2
            },
            TicksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1.5f
            },
            SubticksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        }];
        YAxes = [
        new Axis
        {
            Name = "Загрука ЦП(%)",
            MaxLimit= 100,
            MinLimit=0,
            NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 18,
            Padding = new Padding(5, 0, 15, 0),
            LabelsPaint = new SolidColorPaint(s_gray),
            SeparatorsPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1,
                PathEffect = new DashEffect([3, 3])
            },
            SubseparatorsPaint = new SolidColorPaint
            {
                Color = s_gray2,
                StrokeThickness = 0.5f
            },
            SubseparatorsCount = 9,
            ZeroPaint = new SolidColorPaint
            {
                Color = s_gray1,
                StrokeThickness = 2
            },
            TicksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1.5f
            },
            SubticksPaint = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        }
    ];
        Frame = new()
        {
            Fill = new SolidColorPaint(s_dark3),
            Stroke = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        };
        #endregion
    }

    [RelayCommand]
    private void OnConnect()
    {
        Dispatcher.UIThread.InvokeAsync( async() =>
        {
            while (true)
            {
                ServerLoadInfo info = _SSHService.GetServerLoadInfo();
                if (Points.Count >= 3600)
                    Points.RemoveAt(0);
                Points.Add(new DateTimePoint()
                    { DateTime = DateTime.Now, Value = info.TotalCPUUsage });
                Processes = [.. info.Processes];
                await Task.Delay(5000);
            }
        });
    }
}