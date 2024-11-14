using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LoadCurve.ViewModels;
using LoadCurve.Views;
using System;
using System.Diagnostics;

namespace LoadCurve;

public partial class App : Application
{
    public static WidgetWindow WidgetWindow { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
          //      DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OpenMainWindowClick(object? sender, System.EventArgs e)
    {

        MainWindow mainWindow = new();
        mainWindow.Show();
    }

    private void CloseAppClick(object? sender, System.EventArgs e)
    {
        Environment.Exit(0);
    }
    private void CloseWidgetClick(object? sender, System.EventArgs e)
    {
        if (WidgetWindow is not null)
        {
            WidgetWindow.Close();
            WidgetWindow = null;
        }
    }
}