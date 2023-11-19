using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Shapes;

namespace Client_task_manager
{
    public partial class UserManual : Window //Клас, який реалізує вікно інструкції користувача.
    {
        private const string instuctionFileName = "user_manual.txt";

        private const string userManualNotFound = "You are seeing this text because the program could not find the user manual file. Please contact me if you want to solve this problem. \n\nMy e-mail: ilaskalozub10@gmail.com";

        public UserManual()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Метод, який заносить користувацьку інструкцію з файлу в instructionTextBox під час завантаження вікна.
        {
            try
            {
                string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), instuctionFileName);

                string fileContent = File.ReadAllText(filePath);

                instructionTextBox.AppendText(fileContent);
            }
            catch (IOException ex)
            {
                instructionTextBox.AppendText(userManualNotFound);

                CommonMethods.ShowErrorMessage(ex.Message);
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e) //Метод, який закриває вікно інструкції при натисканні на кнопку 'Go back'.
        {
            Close();
        }
    }
}
