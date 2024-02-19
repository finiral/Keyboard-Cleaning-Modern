using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace WpfApp1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    ulong totalRam = GetTotalRAM();
    Boolean refreshStats = false;
    PerformanceCounter cpuCounter;
    PerformanceCounter ramCounter;
    System.Threading.Timer timer;
    public MainWindow()
    {
        InitializeComponent();
        this.refreshStats = true;
        this.ExtendsContentIntoTitleBar = true;
        this.WindowBackdropType = WindowBackdropType.Mica;
        this.Closing += form_close;
        this.Loaded += AppUi_Load;
    }

    private void AppUi_Load(object sender, EventArgs e)
    {
        Thread thread = new Thread(new ThreadStart(PrepareCounter));
        thread.Start();
    }
    void PrepareCounter()
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
        }
    }

    private void ToggleSwitch_Unchecked(object sender, System.Windows.RoutedEventArgs e)
    {
        Debug.WriteLine("Unactivated !");
        if (sender is ToggleSwitch toggleSwitch)
        {
            KeyboardHook.Unhook();
        }

    }
    private void form_close(object sender, CancelEventArgs e)
    {
        KeyboardHook.Unhook();
        this.refreshStats = false;
        Debug.WriteLine("closed");
    }
    public int getCurrentCpuUsage()
    {
        return (int)cpuCounter.NextValue();
    }

    public decimal getAvailableRAM()
    {
        decimal availableRAM = (decimal)ramCounter.NextValue() / 1024;
        return Math.Round(availableRAM, 1);
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
                int cpu_usage = getCurrentCpuUsage();
                decimal ram_left = getAvailableRAM();  
                Debug.WriteLine("cpu " + cpu_usage);
                if (this.FindName("cpu") is Label cpu && this.FindName("cpu_bar") is ProgressRing cpu_bar)
                {
                    cpu.Content = cpu_usage +" %";
                    cpu_bar.Progress=cpu_usage;
                }
                if (this.FindName("ram") is Label ram && this.FindName("ram_bar") is ProgressRing ram_bar)
                {
                    decimal ram_usage = Math.Round((decimal)totalRam,1) - ram_left;
                    ram.Content = ram_usage +" GB";
                    ram_bar.Progress=(double)Math.Round((ram_usage/(decimal)totalRam)*100,1);
                }
            }));
        }
    }

    public static ulong GetTotalRAM()
    {
        ulong totalRAM = 0;

        try
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject obj in searcher.Get())
            {
                totalRAM = Convert.ToUInt32(obj["TotalVisibleMemorySize"]);
                break; // Assuming there's only one result
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return totalRAM / (1024*1024); // Convert bytes to megabytes
    }
}