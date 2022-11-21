using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Global.Data.Data;
using Global.Data.Data.Web;
using OSCRelay.Services;
using OSCRelay.Services.Implementations;
using IServiceProvider = OSCRelay.Services.IServiceProvider;

namespace OSCRelay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IOSCService oscService;
        private IAvatarInfoService avatarInfoService;
        private ISettingsManagerService settingsManagerService;
        private IServiceProvider serviceProvider;

        private readonly ConsoleContent consoleContent = new ConsoleContent();

        public MainWindow(IOSCService oscService, IAvatarInfoService avatarInfoService, ISettingsManagerService settingsManagerService, IServiceProvider serviceProvider)
        {
            this.oscService = oscService;
            this.avatarInfoService = avatarInfoService;
            this.settingsManagerService = settingsManagerService;
            this.serviceProvider = serviceProvider;
            
            BindingOperations.EnableCollectionSynchronization(consoleContent.ConsoleOutput, consoleContent);
            
            InitializeComponent();
            SettingsManagerService.OnSettingsLoaded += InitializeTextBox;
            CustomLoggerService.OnLogged += OnLogged;
            serviceProvider.OnConnected += OnConnected;

            DataContext = consoleContent;
        }

        private void OnConnected(Token token)
        {
            Dispatcher.Invoke(() =>
            {
                LinkText.Inlines.Clear();
                LinkText.NavigateUri = new Uri($"https://vrcosc.huks.dev?token={token.token}");
                LinkText.Inlines.Add($"https://vrcosc.huks.dev?token={token.token}");
            });
        }

        private void OnLogged(string message)
        {
            Dispatcher.Invoke(() =>
                {
                    consoleContent.AddMessage(message);
                    Scroller.ScrollToBottom();
                }
            );
        }

        private void ConnectToOSCButton(object sender, RoutedEventArgs e)
        {
            oscService.ConnectToOSC(settingsManagerService.GetUserSettings().PortReceive, settingsManagerService.GetUserSettings().PortSend);
        }

        private void WebLinkButton(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo sInfo = new System.Diagnostics.ProcessStartInfo(LinkText.NavigateUri.ToString())
            {
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(sInfo);
        }

        private void AvatarParametersButton(object sender, RoutedEventArgs e)
        {
            AvatarParametersWindow avatarParametersWindow = new AvatarParametersWindow(avatarInfoService, settingsManagerService);
            avatarParametersWindow.Show();
        }

        private void InitializeTextBox(UserSettings userSettings)
        {
            UserId.Text = userSettings.AccountID;
            XToysToken.Text = userSettings.XToysToken;
            ListenPort.Text = userSettings.PortReceive.ToString();
            SendPort.Text = userSettings.PortSend.ToString();

            SettingsManagerService.OnSettingsLoaded -= InitializeTextBox;
        }

        private void SaveSettingsButton(object sender, RoutedEventArgs e)
        {
            int.TryParse(ListenPort.Text, out int listenPort);
            int.TryParse(SendPort.Text, out int sendPort);
            
            UserSettings userSettings = new UserSettings()
            {
                AccountID = UserId.Text,
                XToysToken = XToysToken.Text,
                PortReceive = listenPort,
                PortSend = sendPort
            };
            
            settingsManagerService.SetUserSettings(userSettings);
        }
    }

    public class ConsoleContent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>();
        public ObservableCollection<string> ConsoleOutput
        {
            get { return consoleOutput; }
            set
            {
                consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public void AddMessage(string message)
        {
            ConsoleOutput.Add(message);
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}