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

namespace Client_task_manager
{
    public partial class MainWindow : Window
    {
        private List<MyTask> tasks = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tasks = new List<MyTask>
            {
                new MyTask { TaskTitle = "Task1", TaskText = "Do this and that", DeadlineValue = 33 },
                new MyTask { TaskTitle = "Task2", TaskText = "Do that and this", DeadlineValue = 67 },
                new MyTask { TaskTitle = "Task3", TaskText = "Do this and that", DeadlineValue = 14 },
                new MyTask { TaskTitle = "Task4", TaskText = "Do this and that", DeadlineValue = 77 },
            };

            foreach (MyTask task in tasks)
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

    public class MyTask
    {
        public string TaskTitle { get; set; }

        public string TaskText { get; set; }

        public int DeadlineValue { get; set; }

        public override string ToString()
        {
            return TaskTitle;
        }
    }
}
