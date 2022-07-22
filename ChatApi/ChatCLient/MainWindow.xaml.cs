using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private readonly HttpClient _client = new HttpClient();
        public string BaseURI => (_client.BaseAddress= new Uri("http://localhost:55437/chat-hub")).ToString();
        public MainWindow()
        {
            InitializeComponent();

            var hubAddress = "http://localhost:55437/chat";
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
        public async Task PostAsync(string uri, string item)
        {
            var stringyfiedItem = JsonConvert.SerializeObject(item);
            StringContent data = new StringContent(stringyfiedItem, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(uri, data);
            
        }
        private string receiverId;
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

            hubConnection.On<string, string>(methodName: "ReceiveMessageToUser", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var reconnectingMessage = $"{user} : {message}";
                    messages.Items.Add(reconnectingMessage);
                });
            });

            hubConnection.On<string>(methodName: "UserConnected", (connectedId) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                   
                    //var user = nameInput.Text;
                    messages.Items.Add($"{connectedId}");
                });
            });

            hubConnection.On<string, string>(methodName: "ReceiveMessageFromGroup", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    messages.Items.Add($"{user} : {message}");
                });
            });

            try
            {
                await hubConnection.StartAsync();
                messages.Items.Add("Connection started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;
                //connectionId.Text = GetConnectionId();
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
                var user = nameInput.Text;
                var message = inputMessage.Text;
                var groupname = groupName.Text;
                //await PostAsync(BaseURI + "/send-to-all", message);
                //await hubConnection.InvokeAsync(methodName: "SendMessageToGroup", user, message);
                await hubConnection.InvokeAsync(methodName: "SendMessageToGroup", groupname, user, message);
                nameInput.IsEnabled = false;

                inputMessage.Clear();
                
            }
            catch (Exception ex)
            {

                messages.Items.Add(ex.Message);
            }
        }
        private async void SendMessageToClient(string user, string receiver)
        {
            var message = inputMessage.Text;
            await hubConnection.InvokeAsync(methodName: "SendMessageToUser",receiver, user, message);
        }

        private void sendToCLeint_Click(object sender, RoutedEventArgs e)
        {
            var user = nameInput.Text;
            var cleintConnectedId = cleintId.Text;
            SendMessageToClient(user, cleintConnectedId);
        }

        //private async void GetConnectionId()
        //{
        //    await hubConnection.InvokeAsync(methodName: "GetConnectionId");
        //}

        private async void connectToCLeint_Click(object sender, RoutedEventArgs e)
        {
            var connectedId = await hubConnection.InvokeAsync<string>(methodName: "GetConnectionId");
            connectionId.Text = (connectedId);
        }

        private async void joinGroup_Click(object sender, RoutedEventArgs e)
        {
            var groupName1 = groupName.Text;
            await hubConnection.InvokeAsync(methodName: "JoinGroup", groupName1);
        }
    }
}
