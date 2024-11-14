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
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LoadCurve.ViewModels
{
    internal partial class WidgetWindowViewModel : ViewModelBase
    {
        #region Цвета графика
        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);
        #endregion
        [ObservableProperty]
        private ObservableCollection<ISeries> _CPUSeries;
        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> _CPUXAxes;
        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _CPUPoints;
        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> _CPUYAxes;
        [ObservableProperty]
        private DrawMarginFrame _CPUFrame;
        [ObservableProperty]
        private ObservableCollection<ISeries> _MemorySeries;
        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> _MemoryXAxes;
        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _MemoryPoints;
        [ObservableProperty]
        private ObservableCollection<ICartesianAxis> _MemoryYAxes;
        [ObservableProperty]
        private DrawMarginFrame _MemoryFrame;
        public SSHService SSHService;
        public Server CurrentServer { get; set; }

        public WidgetWindowViewModel()
        {
            ConfigureMemoryCurve();
            ConfigureCPUCurve();
        }
        private void ConfigureCPUCurve()
        {
            CPUPoints = new();
            CPUSeries = [new LineSeries<DateTimePoint>(CPUPoints)
        {
            Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
            Fill = null,
            GeometrySize = 0
        }];
            CPUXAxes = [new DateTimeAxis(TimeSpan.FromSeconds(5), date => date.ToString("hh:mm:ss"))
        {
             NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 12,
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
            CPUYAxes = [
            new Axis
        {
            Name = "CPU",
            MaxLimit= 100,
            MinLimit=0,
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 12,
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
            CPUFrame = new()
            {
                Fill = new SolidColorPaint(s_dark3),
                Stroke = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            };
        }
        private void ConfigureMemoryCurve()
        {
            MemoryPoints = new();
            MemorySeries = [new LineSeries<DateTimePoint>(MemoryPoints)
        {
            Stroke = new SolidColorPaint(new SKColor(33, 150, 243), 4),
            Fill = null,
            GeometrySize = 0
        }];
            MemoryXAxes = [new DateTimeAxis(TimeSpan.FromSeconds(5), date => date.ToString("hh:mm:ss"))
        {
             NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 12,
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
            MemoryYAxes = [
            new Axis
        {
            Name = "RAM",
            MaxLimit= 100,
            MinLimit=0,
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(s_gray1),
            TextSize = 12,
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
            MemoryFrame = new()
            {
                Fill = new SolidColorPaint(s_dark3),
                Stroke = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            };
        }
        [RelayCommand]
        private void OnClosingApp()
        {
            SSHService.CloseConnection();
        }
        [RelayCommand]
        private void OnLoadedApp()
        {
            SSHService = new(CurrentServer);
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                while (true)
                {
                    ServerLoadInfo info = SSHService.GetServerLoadInfo();
                    if (CPUPoints.Count >= 3600)
                    {
                        CPUPoints.RemoveAt(0);
                        MemoryPoints.RemoveAt(0);
                    }
                    CPUPoints.Add(new DateTimePoint()
                    { DateTime = DateTime.Now, Value = info.TotalCPUUsage });
                    MemoryPoints.Add(new DateTimePoint()
                    { DateTime = DateTime.Now, Value = info.TotalMemoryUsage });
                    await Task.Delay(5000);
                }
            });
        }
    }
}
