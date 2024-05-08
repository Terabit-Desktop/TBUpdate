using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Update.Core;

namespace Update
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CurrentState _state;

        public CurrentState State { get => _state; set { _state = value; CurrentState.Content = $"{State.ToString().ToUpper()}..."; } }
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnContentRendered(object sender, EventArgs e)
        {
            if (await CheckForUpdates())
                UpdateApplication();
            else
                StartTerabit();
        }

        private void StartApplication()
        {
            try
            {
                State = Update.CurrentState.Starting;
                Process MainProc = Process.Start("Terabit Desktop.exe");
                MainProc.WaitForInputIdle();
            }
            catch
            {
                MessageBox.Show("There was an error while trying to start 'Terabit Desktop.exe'. Please check the file exists and try again.", "Error | ERR_EXE001", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Application.Current.Shutdown();
        }

        public async Task<bool> CheckForUpdates()
        {
            State = Update.CurrentState.Checking;

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "version.txt")))
                return true;

            Core.Version LocalVersion = new Core.Version(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "version.txt")));
            Core.Version RemoteVersion;

            using (HttpClient _client = new HttpClient())
            {
                HttpResponseMessage response = await _client.GetAsync("http://207.174.40.81/public/app/version.txt");

                if (response.IsSuccessStatusCode)
                    RemoteVersion = new Core.Version(await response.Content.ReadAsStringAsync());
                else
                    return false;
            }

            return LocalVersion.IsDifferent(RemoteVersion);
        }

        private async void UpdateApplication()
        {
            State = Update.CurrentState.Downloading;

            using (HttpClient _client = new HttpClient())
            {
                HttpResponseMessage response = await _client.GetAsync("http://207.174.40.81/public/app/app.asar");

                if (response.IsSuccessStatusCode)
                {
                    State = Update.CurrentState.Installing;
                    Directory.CreateDirectory("resources");
                    File.WriteAllBytes("resources/app.asar", await response.Content.ReadAsByteArrayAsync()); // Application File.
                    File.WriteAllBytes("version.txt", await _client.GetByteArrayAsync("http://207.174.40.81/public/app/version.txt")); // Version File.

                    StartTerabit();
                }

                return;
            }
        }

        private void StartTerabit()
        {
            State = Update.CurrentState.Starting;
            string ExecPath = Path.Combine(Directory.GetCurrentDirectory(), "Terabit Desktop.exe");
            Process DesktopProc = Process.Start(new ProcessStartInfo() { FileName = ExecPath });

            DesktopProc.WaitForInputIdle();
            Environment.Exit(0);
        }
    }

    public enum CurrentState
    {
        Checking, // Checking for updates.
        Downloading, // Downloading the update.
        Installing, // Installing the update.
        Starting // Starting the application.
    }
}
