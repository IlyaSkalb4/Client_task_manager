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
using Classes_for_transferring_users;

namespace Client_task_manager
{
    public partial class MainWindow : Window
    {
        private List<UserTask> tasks = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        public List<UserTask> UserTasks
        {
            get
            {
                return tasks;
            }
            set
            {
                tasks = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tasks == null)
                return;

            foreach (UserTask task in tasks)
            {
                taskListBox.Items.Add(task);
            }
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (taskListBox.SelectedItems.Count < 1)
            {
                return;
            }

            taskListBox.Items.RemoveAt(taskListBox.SelectedIndex);
        }
    }
}
