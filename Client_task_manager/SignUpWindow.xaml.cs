using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading.Tasks;
using Classes_for_transferring_users;
using System.Windows.Controls;

namespace Client_task_manager
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private TcpClient tcpClient = null;
        private NetworkStream networkStream = null;
        private IFormatter formatter = null;

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void singUpButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = firstNameTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;
            string passwordRepeat = passwordRepeatTextBox.Text;

            if(firstName == "" || lastName == "" || email == "" || password == "" || passwordRepeat == "" ||
                CommonMethods.IsEmail(email) || CommonMethods.IsPassword(password) || password != passwordRepeat)
            {
                return;
            }

            NewUser newUser = new NewUser {FirstName = firstName, LastName = lastName, UserEmail = email, UserPassword = password };

            SendPackageAsync(new ReadyPackage { ObjType = Constants.Registration, Data = newUser });
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

        private async void SendPackageAsync(ReadyPackage readyPackage)
        {
            bool taskResult = false;

            await Task.Run(() => { taskResult = SendPackage(readyPackage); });

            if (taskResult)
            {
                MessageBox.Show("Registration request has been sent!");
            }

            if (networkStream != null)
            {
                networkStream.Close();
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
            }

            Close();
        }
    }
}
