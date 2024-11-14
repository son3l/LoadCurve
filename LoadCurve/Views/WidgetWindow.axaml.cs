using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace LoadCurve;

public partial class WidgetWindow : Window
{
    public WidgetWindow()
    {
        InitializeComponent();
        SystemDecorations = SystemDecorations.None;
        Position = new PixelPoint(Convert.ToInt32(Screens.All[0].Bounds.Width - Width), -31);
        Background = Brushes.Transparent;
    }
}