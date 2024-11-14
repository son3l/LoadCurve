using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LoadCurve;

public partial class MessageWindow : Window
{
    public string MessageText { get; set; }
    public string MessageTitle { get; set; }
    public string MessageIconName { get; set; }
    public MessageWindow()
    {
        MessageText = "";
        MessageTitle = "Ошибка";
        InitializeComponent();
        DataContext = this;
    }
    public MessageWindow(string MessageText, string MessageTitle, string MessageIconName)
    {
        DataContext = this;
        this.MessageText = MessageText;
        this.MessageTitle = MessageTitle;
        this.MessageIconName = MessageIconName;
        InitializeComponent();
    }
    public MessageWindow(string MessageText, string MessageTitle)
    {
        DataContext = this;
        MessageIconName = "AlertBoxOutline";
        this.MessageText = MessageText;
        this.MessageTitle = MessageTitle;
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}