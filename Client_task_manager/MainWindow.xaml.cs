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
    public partial class MainWindow : Window //Клас, який реалізує основне вікно.
    {
        private NetworkManager networkManager;

        private List<UserTask> tasks = null;

        private List<UserTask> currentTasks = null;

        private DispatcherTimer timer = null;

        private UserLogin userLogin = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        public List<UserTask> UserTasks //Властивість, яка зберігає список завдань користувача.
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

        public UserLogin MainUserLogin //Властивість, яка зберігає логін і пароль користувача.
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

        public bool FlagToExit //Властивість, яка показує яким способом користувач закрив вікно.
        {
            get;
            private set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Метод, що запускається під час завантаження вікна, виділяє пам'ять для основних класів, запускає таймер, оновлює завдання, встановлює початкові значення змінних.
        {
            networkManager = new NetworkManager();

            currentTasks = new List<UserTask>();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(Constants.TimerInterval);
            timer.Tick += Timer_Tick;
            timer.Start();

            StringTruncationConverter.NumberOfSymbols = Constants.NumberOfSymbols;

            FlagToExit = true;

            executeTaskButton.IsEnabled = false;

            if (tasks == null)
                return;

            UpdateListsBox(tasks);
        }

        private void Timer_Tick(object sender, EventArgs e) //Метод, який запускає відправлення запиту на оновлення завдань під час тику таймера.
        {
            ReadyPackage sendPackage = new ReadyPackage
            {
                ObjType = Constants.Login,
                Data = userLogin
            };

            ReceiveUserTasksAsync(sendPackage);
        }

        private void ExecuteTaskButton_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на кнопку executeTaskButton запускає надсилання запиту на виконання завдання.
        {
            SendCompletedUserTask();

            executeTaskButton.IsEnabled = false;
        }

        private void CurrentTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Метод, який у разі зміни вибору нинішнього завдання змінює доступ до кнопки executeTaskButton.
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                executeTaskButton.IsEnabled = false;
                return;
            }

            executeTaskButton.IsEnabled = true;
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на 'Execute' елемент у контекстному меню, запускає надсилання запиту на виконання завдання.
        {
            if (currentTasksListBox.SelectedItems.Count < 1)
            {
                return;
            }

            SendCompletedUserTask();
        }

        private void AllPrioritiesMenuItem_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на 'All priorities' елемент у контекстному меню, запускає показує завдання за всіма пріоритетами.
        {
            currentTasksListBox.Items.Clear();

            foreach(UserTask userTask in currentTasks) 
            {
                currentTasksListBox.Items.Add(userTask);
            }
        }

        private void PriorityMenuItem_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на конкретний пріоритет у контекстному меню, показує завдання за цим пріоритетом.
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

        private void CustomInstructionsMenuItem_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на 'Custom instructions' елемент у меню три точки, запускає вікно користувацької інструкції.
        {
            UserManual userManual = new UserManual();

            userManual.ShowDialog();
        }

        private void LogOutMenuItem_Click(object sender, RoutedEventArgs e) //Метод, який при натисканні на 'Log out' елемент у меню три точки, закриває основне вікно.
        {
            FlagToExit = false;

            userLogin = null;

            Close();
        }

        private async void ReceiveUserTasksAsync(ReadyPackage readyPackage) //Метод, який надсилає запит оновлення завдань на сервер. У разі успіху оновлює завдання, у разі невдачі показує помилку.
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

        private async void SendCompletedUserTaskAsync(ReadyPackage readyPackage) //Метод, який надсилає запит виконати завдання на сервер. У разі успіху додає завдання до виконаних, у разі невдачі показує помилку.
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

        private void SendCompletedUserTask() //Метод, який готує виконане завдання і запускає запит на виконання.
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

        private void UpdateListsBox(List<UserTask> userTasks) //Метод, який оновлює списки завдань.
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
