using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
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

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerSide.Start();
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;

            if (email == "" || password == "" ||
                !CommonMethods.IsEmail(email) || !CommonMethods.IsPassword(password))
            {
                return;
            }

            UserLogin userLogin = new UserLogin { UserEmail = email, UserPassword = password };

            SendAndReceivePackageAsync(new ReadyPackage { ObjType = Constants.Login, Data = userLogin });
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

                if(receivedPackage.ObjType == Constants.UserTask) 
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
                tcpClient.Connect(Constants.ServerIP, Constants.Port1024);

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
