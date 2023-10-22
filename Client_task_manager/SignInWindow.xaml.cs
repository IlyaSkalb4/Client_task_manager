using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Classes_for_transferring_users;

namespace Client_task_manager
{
    public partial class SignInWindow : Window
    {
        private TcpClient tcpClient = null;

        private List<UserTask> myTasks = null;

        private ReadyPackage receivedPackage = null;

        private NetworkStream networkStream = null;

        private IFormatter formatter = null;

        private const string serverIP = "127.0.0.1";
        //private const string serverIP = "77.122.231.142";  
        private const string emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";
        private const string passwordPattern = @"\w{8,}";
        private const string login = "Login";
        private const string registration = "Registration";
        private const string userTask = "UserTasks";

        private const int port1024 = 1024;

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;

            if (email == "" || password == "" || !Regex.IsMatch(email, emailPattern) || !Regex.IsMatch(password, passwordPattern))
            {
                return;
            }

            UserLogin userLogin = new UserLogin { UserEmail = email, UserPassword = password };

            SendAndReceivePackageAsync(new ReadyPackage { ObjType = login, Data = userLogin });
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.ShowDialog();

            Show();
        }

        private void ReceivePaсkage()
        {
            try
            {
                receivedPackage = (ReadyPackage)formatter.Deserialize(networkStream);

                if(receivedPackage.ObjType == userTask) 
                {
                    myTasks = (List<UserTask>)receivedPackage.Data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                }

                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }
        }

        private bool SendPackage(ReadyPackage readyPackage)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIP, port1024);

                networkStream = tcpClient.GetStream();

                formatter = new BinaryFormatter();

                formatter.Serialize(networkStream, readyPackage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private async void SendAndReceivePackageAsync(ReadyPackage readyPackage)
        {
            logInButton.IsEnabled = false;
            emailTextBox.IsEnabled = false;
            passwordTextBox.IsEnabled = false;

            bool taskRunResult = false;

            await Task.Run(() =>
            {
                taskRunResult = SendPackage(readyPackage);
            });

            if(!taskRunResult)
            {
                logInButton.IsEnabled = true;
                emailTextBox.IsEnabled = true;
                passwordTextBox.IsEnabled = true;
                return;
            }

            await Task.Run(ReceivePaсkage);

            if (myTasks != null)
            {
                Hide();

                MainWindow mainWindow = new MainWindow();
                mainWindow.UserTasks = myTasks;
                mainWindow.ShowDialog();

                Close();
            }

            logInButton.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            passwordTextBox.IsEnabled = true;
        }
    }
}
