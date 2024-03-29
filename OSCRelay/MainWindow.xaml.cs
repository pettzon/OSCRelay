﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
            serviceProvider.UpdateExposedAvatarParameters(settingsManagerService.GetExposedAvatarParameters());
            
            Dispatcher.Invoke(() =>
            {
                Token.Text = token.token;
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
            oscService.ConnectToOSC(settingsManagerService.GetUserSettings().PortReceive, settingsManagerService.GetUserSettings().PortSend, settingsManagerService.GetUserSettings().Token ?? new Token(""));
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
            Token.Text = userSettings.Token.token;
            XToysToken.Text = userSettings.XToysToken;
            ListenPort.Text = userSettings.PortReceive.ToString();
            SendPort.Text = userSettings.PortSend.ToString();

            SettingsManagerService.OnSettingsLoaded -= InitializeTextBox;
        }

        public async Task SaveSettings()
        {
            int.TryParse(ListenPort.Text, out int listenPort);
            int.TryParse(SendPort.Text, out int sendPort);

            UserSettings userSettings = settingsManagerService.GetUserSettings() ?? new UserSettings();

            userSettings.AccountID = UserId.Text;
            userSettings.Token = new Token(Token.Text);
            userSettings.XToysToken = XToysToken.Text;
            userSettings.PortReceive = listenPort;
            userSettings.PortSend = sendPort;

            settingsManagerService.SetUserSettings(userSettings);
            await settingsManagerService.SaveUserSettings();
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