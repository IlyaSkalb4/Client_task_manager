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
using System.Windows.Media.Animation;
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

        private List<UserTask> currentTasks = null;

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

        public bool FlagToExit { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            networkManager = new NetworkManager();

            currentTasks = new List<UserTask>();

            timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMinutes(timerInterval);
            timer.Interval = TimeSpan.FromSeconds(timerInterval);
            timer.Tick += Timer_Tick;
            timer.Start();

            StringTruncationConverter.NumberOfSymbols = 28;

            FlagToExit = true;

            if (tasks == null)
                return;

            UpdateListsBox(tasks);
        }

        private void ButtonAnimation_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            double fromPercentage = 0.5;
            double toPercentage = 1.5;

            double fromValue = button.ActualWidth * fromPercentage;
            double toValue = button.ActualWidth * toPercentage;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(1)
            };

            button.BeginAnimation(Button.WidthProperty, animation);

            button.IsEnabled = false;
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

        private void executeTaskButton_Click(object sender, RoutedEventArgs e)
        {
            SendCompletedUserTask();

            executeTaskButton.IsEnabled = false;
        }

        private void currentTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                executeTaskButton.IsEnabled = false;
                return;
            }

            executeTaskButton.IsEnabled = true;
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                return;
            }

            SendCompletedUserTask();
        }

        private void AllPrioritiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            currentTasksListBox.Items.Clear();

            foreach(UserTask userTask in currentTasks) 
            {
                currentTasksListBox.Items.Add(userTask);
            }
        }

        private void PriorityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string menuItemName = ((MenuItem)sender).Name;
            string priority = "";

            if(menuItemName == "highPriorityMenuItem") 
            {
                priority = Constants.High;
            }
            else if (menuItemName == "lowPriorityMenuItem")
            {
                priority = Constants.Low;
            }
            else if (menuItemName == "normalPriorityMenuItem")
            {
                priority = Constants.Normal;
            }
            else
            {
                return;
            }

            currentTasksListBox.Items.Clear();

            foreach (UserTask userTask in currentTasks)
            {
                if (userTask.Priority == priority)
                {
                    currentTasksListBox.Items.Insert(0, userTask);
                }
            }
        }

        private void CustomInstructionsMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FlagToExit = false;

            userLogin = null;

            Close();
        }

        private async void ReceiveUserTasksAsync(ReadyPackage readyPackage)
        {
            if (await networkManager.SendAndReceivePackageAsync(readyPackage))
            {
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
                UserTask userTask = (UserTask)currentTasksListBox.SelectedItem;

                userTask.DateTimeCompleted = ((TaskAccomplished)readyPackage.Data).DateTimeCompleted;

                completeTasksListBox.Items.Insert(0, userTask);

                currentTasks.RemoveAt(currentTasksListBox.SelectedIndex);
                currentTasksListBox.Items.RemoveAt(currentTasksListBox.SelectedIndex);
            }
            else
            {
                CommonMethods.ShowErrorMessage(networkManager.ErrorMessage);
            }
        }

        private void SendCompletedUserTask()
        {
            UserTask currentUserTask = (UserTask)currentTasksListBox.SelectedItem;

            TaskAccomplished taskAccomplished = new TaskAccomplished
            {
                UserEmailAndPassword = new UserLogin
                {
                    UserEmail = userLogin.UserEmail,
                    UserPassword = userLogin.UserPassword
                },
                SubTaskTitle = currentUserTask.SubTaskTitle,
                IsTaskCompleted = currentUserTask.IsTaskCompleted,
                DateTimeCompleted = DateTime.Now
            };

            ReadyPackage readyPackage = new ReadyPackage
            {
                ObjType = Constants.CompletedUserTask,
                Data = taskAccomplished
            };

            SendCompletedUserTaskAsync(readyPackage);
        }

        private void UpdateListsBox(List<UserTask> userTasks)
        {
            if (userTasks != null)
            {
                currentTasks.Clear();

                currentTasksListBox.Items.Clear();
                completeTasksListBox.Items.Clear();
                expiredTasksListBox.Items.Clear();

                foreach (UserTask userTask in userTasks)
                {
                    if (userTask.IsTaskCompleted.HasValue)
                    {
                        if (userTask.IsTaskCompleted.Value)
                        {
                            completeTasksListBox.Items.Insert(0, userTask);
                        }
                        else
                        {
                            expiredTasksListBox.Items.Insert(0, userTask);
                        }
                    }
                    else
                    {
                        if (userTask.DeadlineValue == 100)
                        {
                            completeTasksListBox.Items.Insert(0, userTask);
                        }
                        else
                        {
                            currentTasks.Insert(0, userTask);
                            currentTasksListBox.Items.Insert(0, userTask);
                        }
                    }
                }
            }
        }
    }
}
