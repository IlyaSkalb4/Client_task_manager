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
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_task_manager
{
    public partial class SignInWindow : Window //Клас, який реалізує вікно входу.
    {
        private NetworkManager networkManager = null;

        private PasswordHiding passwordHiding = null;

        private UserLogin userLogin = null;

        private string passwordLine;

        public SignInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Метод, який виділяє пам'ять для необхідних класів під час завантаження вікна.
        {
            ServerSide.Start();

            networkManager = new NetworkManager();

            passwordHiding = new PasswordHiding();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на кнопку logInButton перевіряє поля, введені користувачем, і якщо всі перевірки пройшли успішно, запускає відправлення запиту на вхід у систему.
        {
            ReadyPackage sendPackage;

            string email = emailTextBox.Text;

            bool flagCheck = false;

            emailWarningTextBlock.Text = "";
            passwordWarningTextBlock.Text = "";

            if (String.IsNullOrEmpty(email) || email == Constants.Email)
            {
                emailWarningTextBlock.Text = Constants.EnterEmail;
                flagCheck = true;
            }
            else if (!CommonMethods.IsEmail(email))
            {
                emailWarningTextBlock.Text = Constants.IncorrectEmail;
                flagCheck = true;
            }

            if (String.IsNullOrEmpty(passwordLine))
            {
                passwordWarningTextBlock.Text = Constants.EnterPassword;
                flagCheck = true;
            }
            else if(!CommonMethods.IsPassword(passwordLine))
            {
                passwordWarningTextBlock.Text = Constants.IncorrectPassword;
                flagCheck = true;
            }

            if(flagCheck)
            { 
                return;
            }

            userLogin = new UserLogin { UserEmail = email, UserPassword = passwordLine };

            sendPackage = new ReadyPackage { ObjType = Constants.Login, Data = userLogin };

            SendAndReceivePackageAsync(sendPackage);
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на кнопку signUpButton запускає вікно реєстрації.
        {
            Hide();

            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.ShowDialog();

            Show();
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при отриманні фокусу emailTextBox. 
        {
            TextBox textBox = (TextBox)sender;

            if(textBox.Text == Constants.Email)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при втраті фокусу emailTextBox. 
        {
            TextBox textBox = (TextBox)sender;

            if(String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = Constants.Email;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при отриманні фокусу passwordTextBox.
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == Constants.Password)
            {
                passwordLine = "";
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e) //Метод, який змінює колір шрифту при втраті фокусу passwordTextBox.
        {
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = Constants.Password;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e) //Метод, який змінює введені символи на крапку у passwordTextBox. 
        {
            TextBox textBox = (TextBox)sender;

            string textBoxText = textBox.Text;

            if (textBoxText != Constants.Password)
            {
                passwordHiding.HidePasswordLine(textBoxText);

                passwordLine = passwordHiding.OriginalPasswordLine;

                textBox.Text = passwordHiding.Points;

                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private async void SendAndReceivePackageAsync(ReadyPackage readyPackage) //Метод, який надсилає запит на вхід у систему. У разі успіху запускає основне вікно, у разі невдачі відображає помилку.
        {
            logInButton.IsEnabled = false;
            emailTextBox.IsEnabled = false;
            passwordTextBox.IsEnabled = false;
            signUpButton.IsEnabled = false;

            if (await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
                if (networkManager.UserTasks != null)
                {
                    Hide();

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.UserTasks = networkManager.UserTasks;
                    mainWindow.MainUserLogin = userLogin;

                    passwordLine = "";
                    passwordHiding = new PasswordHiding();

                    passwordTextBox.Text = Constants.Password;
                    passwordTextBox.Foreground = Brushes.Gray;

                    emailTextBox.Text = Constants.Email;
                    emailTextBox.Foreground = Brushes.Gray;

                    mainWindow.ShowDialog();

                    if (mainWindow.FlagToExit)
                    {
                        Close();
                    }

                    Show();
                }
            }
            else
            {
                string errorType = networkManager.ErrorType;

                if (errorType == Constants.Email)
                {
                    emailWarningTextBlock.Text = Constants.IncorrectEmailSignIn;
                }
                else if (errorType == Constants.Password)
                {
                    passwordWarningTextBlock.Text = Constants.IncorrectPasswordSignIn;
                }
                else
                {
                    CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
                }
            }

            logInButton.IsEnabled = true;
            emailTextBox.IsEnabled = true;
            passwordTextBox.IsEnabled = true;
            signUpButton.IsEnabled = true;
        }
    }
}
