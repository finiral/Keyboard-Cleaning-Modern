using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace WpfApp1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    Boolean refreshStats = false;
    PerformanceCounter cpuCounter;
    PerformanceCounter ramCounter;
    System.Threading.Timer timer;
    public MainWindow()
    {
        InitializeComponent();
        this.refreshStats = true;
        this.ExtendsContentIntoTitleBar = true;
        this.WindowBackdropType = WindowBackdropType.Tabbed;
        this.Closing += form_close;
        this.Loaded += AppUi_Load;
    }

    private void AppUi_Load(object sender, EventArgs e)
    {
        cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        // Initialize timer to update stats every second
        timer = new System.Threading.Timer(UpdateStats, null, 0, 1000);
    }

    private void ToggleSwitch_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        Debug.WriteLine("Activated !");
        if (sender is ToggleSwitch toggleSwitch)
        {
            KeyboardHook.SetHook();
            toggleSwitch.Content = "Stop Cleaning";
        }
    }

    private void ToggleSwitch_Unchecked(object sender, System.Windows.RoutedEventArgs e)
    {
        Debug.WriteLine("Unactivated !");
        if (sender is ToggleSwitch toggleSwitch)
        {
            KeyboardHook.Unhook();
            toggleSwitch.Content = "Start Cleaning";
        }

    }
    private void form_close(object sender, CancelEventArgs e)
    {
        KeyboardHook.Unhook();
        this.refreshStats = false;
        Debug.WriteLine("closed");
    }
    public string getCurrentCpuUsage()
    {
        return (int)cpuCounter.NextValue() + "%";
    }

    public string getAvailableRAM()
    {
        return ramCounter.NextValue() + "MB";
    }

    private void UpdateStats(object state)
    {
        if (!this.refreshStats)
        {
            // Stop the timer if refreshStats is false
            timer.Dispose();
            return;
        }

        else
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Debug.WriteLine("cpu " + getCurrentCpuUsage());
                if (this.FindName("cpu") is Label cpu)
                {
                    cpu.Content = getCurrentCpuUsage();
                }
                if (this.FindName("ram") is Label ram)
                {
                    ram.Content = getAvailableRAM();
                }
            }));
        }
    }
}