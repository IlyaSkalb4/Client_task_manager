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

        private UserLogin userLogin = null;

        private int timerInterval = 30;

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

        public UserLogin MainUserLogin
        {
            get
            {
                return userLogin;
            }
            set
            {
                userLogin = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            networkManager = new NetworkManager();

            timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMinutes(timerInterval);
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
                ObjType = Constants.Login,
                Data = userLogin
            };

            ReceiveUserTasksAsync(sendPackage);
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                return;
            }

            UserTask currentUserTask = (UserTask)currentTasksListBox.SelectedItem;

            TaskAccomplished taskAccomplished = new TaskAccomplished
            {
                UserEmailAndPassword = new UserLogin
                {
                    UserEmail = userLogin.UserEmail,
                    UserPassword = userLogin.UserPassword
                },
                SubTaskTitle = currentUserTask.SubTaskTitle,
                IsTaskCompleted = currentUserTask.IsTaskCompleted
            };

            ReadyPackage readyPackage = new ReadyPackage
            {
                ObjType = Constants.CompletedUserTask,
                Data = taskAccomplished
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
            else
            {
                CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
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
                CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
            }
        }

        private void UpdateListsBox(List<UserTask> userTasks)
        {
            foreach (UserTask userTask in userTasks)
            {
                if (userTask.IsTaskCompleted.HasValue)
                {
                    if (userTask.IsTaskCompleted.Value)
                    {
                        completeTasksListBox.Items.Add(userTask);
                    }
                    else
                    {
                        expiredTasksListBox.Items.Add(userTask);
                    }
                }
                else
                {
                    if (userTask.DeadlineValue == 100)
                    {
                        completeTasksListBox.Items.Add(userTask);
                    }
                    else
                    {
                        currentTasksListBox.Items.Add(userTask);
                    }
                }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

        }
    }
}
