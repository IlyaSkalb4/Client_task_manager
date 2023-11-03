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
using System.Windows.Threading;
using Classes_for_transferring_users;

namespace Client_task_manager
{
    public partial class MainWindow : Window
    {
        private NetworkManager networkManager;

        private List<UserTask> tasks = null;

        private DispatcherTimer timer = null;

        private int timerInterval = 10;

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
            networkManager = new NetworkManager();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(timerInterval);
            timer.Interval = TimeSpan.FromSeconds(timerInterval);
            timer.Tick += Timer_Tick;
            timer.Start();

            if (tasks == null)
                return;

            UpdateListsBox(tasks);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ReadyPackage sendPackage = new ReadyPackage
            {
                ObjType = Constants.UserTask,
                Data = "",
                RepeatStatus = true
            };

            ReceiveUserTasksAsync(sendPackage);
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                return;
            }

            ReadyPackage readyPackage = new ReadyPackage
            {
                ObjType = Constants.CompletedUserTask,
                Data = (UserTask)currentTasksListBox.SelectedItem,
                RepeatStatus = true
            };

            SendCompletedUserTaskAsync(readyPackage);
        }

        private async void ReceiveUserTasksAsync(ReadyPackage readyPackage)
        {
            if (await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
                currentTasksListBox.Items.Clear();
                completeTasksListBox.Items.Clear();
                expiredTasksListBox.Items.Clear();

                UpdateListsBox(networkManager.UserTasks);

                networkManager.ClearUserTasks();
            }
        }

        private async void SendCompletedUserTaskAsync(ReadyPackage readyPackage)
        {
            if (await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
                completeTasksListBox.Items.Add(currentTasksListBox.SelectedItem);

                currentTasksListBox.Items.RemoveAt(currentTasksListBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show(networkManager.ErrorMessage, networkManager.ErrorType, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateListsBox(List<UserTask> userTasks)
        {
            foreach (UserTask userTask in userTasks)
            {
                if (userTask.IsTaskCompleted)
                {
                    completeTasksListBox.Items.Add(userTask);
                }
                else if (userTask.DeadlineValue == 100 && !userTask.IsTaskCompleted)
                {
                    expiredTasksListBox.Items.Add(userTask);
                }
                else
                {
                    currentTasksListBox.Items.Add(userTask);
                }
            }
        }
    }
}
