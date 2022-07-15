using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatCLient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;
        public MainWindow()
        {
            InitializeComponent();

            var hubAddress = "http://localhost:44341/chat-hub";
            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubAddress)
                .WithAutomaticReconnect()
                .Build();

            hubConnection.Reconnecting += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var reconnectingMessage = "Attempting to reconnect....";
                    messages.Items.Add(reconnectingMessage);
                });

                return Task.CompletedTask;
            };

            hubConnection.Reconnected += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var reconnectingMessage = "Reconnected to the server";
                    messages.Items.Clear();
                    messages.Items.Add(reconnectingMessage);
                });

                return Task.CompletedTask;
            };

            hubConnection.Closed += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var reconnectingMessage = "Connected closed";
                    messages.Items.Add(reconnectingMessage);
                    openConnection.IsEnabled = true;
                    sendMessage.IsEnabled = false;
                });

                return Task.CompletedTask;
            };
        }

        private async void openConnection_Click(object sender, RoutedEventArgs e)
        {
            hubConnection.On<string, string>(methodName: "ReceiveMessage", (user, message) =>
             {
                 this.Dispatcher.Invoke(() =>
                 {
                     var reconnectingMessage = $"{user}:{message}";
                     messages.Items.Add(reconnectingMessage);
                 });
             });

            try
            {
                await hubConnection.StartAsync();
                messages.Items.Add("Connection started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;
            }
            catch (Exception ex)
            {

                messages.Items.Add(ex.Message);
            }
        }

        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = "WPF Client";
                var message = messageInput.Text;
                await hubConnection.InvokeAsync(methodName: "SendMessage", user, message);
            }
            catch (Exception ex)
            {

                messages.Items.Add(ex.Message);
            }
        }
    }
}
