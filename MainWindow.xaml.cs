using System.Diagnostics;
using Wpf.Ui.Controls;

namespace WpfApp1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        this.ExtendsContentIntoTitleBar = true;
        this.WindowBackdropType=WindowBackdropType.Tabbed;
        InitializeComponent();
    }

    private void ToggleSwitch_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        Debug.WriteLine("Activated !");
        if (sender is ToggleSwitch toggleSwitch)
        {
            toggleSwitch.Content = "Stop Cleaning";
        }
    }

    private void ToggleSwitch_Unchecked(object sender, System.Windows.RoutedEventArgs e)
    {
        Debug.WriteLine("Unactivated !");
        if (sender is ToggleSwitch toggleSwitch)
        {
            toggleSwitch.Content = "Start Cleaning";
        }

    }
}