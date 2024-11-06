using BO;
using PL.Engineer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace PL.Task
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public bool ProjectRunning
        {
            get { return (bool)GetValue(ProjectRunningProperty); }
            set { SetValue(ProjectRunningProperty, value); }
        }

        public static readonly DependencyProperty ProjectRunningProperty =
            DependencyProperty.Register("ProjectRunning", typeof(bool), typeof(TaskWindow), new PropertyMetadata(null));

        public BO.Task CurrentTask
        {
            get { return (BO.Task)GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(TaskWindow), new PropertyMetadata(null));

        public ObservableCollection<TaskInList> DependenedTaskList
        {
            get { return (ObservableCollection<TaskInList>)GetValue(DependenedTaskListProperty); }
            set { SetValue(DependenedTaskListProperty, value); }
        }

        public static readonly DependencyProperty DependenedTaskListProperty =
            DependencyProperty.Register(nameof(DependenedTaskList),
                typeof(ObservableCollection<TaskInList>), typeof(TaskWindow), new PropertyMetadata(null));

        public int TaskDuration
        {
            get { return (int)GetValue(TaskDurationProperty); }
            set { SetValue(TaskDurationProperty, value); }
        }

        public static readonly DependencyProperty TaskDurationProperty =
            DependencyProperty.Register("TaskDuration", typeof(int), typeof(TaskWindow), new PropertyMetadata(null));

        public TaskWindow(int Id = 0)
        {
            ProjectRunning = s_bl.Status() != ProjectStatus.Execution;
            InitializeComponent();
            if (Id == 0)
            {
                CurrentTask = new BO.Task
                {
                    Milestone = new BO.MilestoneInTask() { Alias = "Milestone alias is empty" },
                    Engineer = new BO.EngineerInTask()
                };
                TaskDuration = 1;


            }
            else
            {
                try
                {
                    CurrentTask = s_bl.Task.GetTask(Id);
                    if (CurrentTask.Milestone == null) CurrentTask.Milestone = new BO.MilestoneInTask() { Alias = "milston alias is empty" };
                    if (CurrentTask.Engineer == null) CurrentTask.Engineer = new BO.EngineerInTask();
                    DependenedTaskList = CurrentTask.Dependencies is not null ? new ObservableCollection<TaskInList>(CurrentTask.Dependencies) : new();
                    TaskDuration = CurrentTask.Duration!.Value.Days;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void BtnAddOrUpdateTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            CurrentTask.Duration = new TimeSpan(TaskDuration, 0, 0, 0);
            try
            {
                if ((string)button!.Content == "Add")
                {
                    // Creating the new task in the BL

                    s_bl.Task.CreateTask(CurrentTask);
                    MessageBox.Show("The task has been added successfully!");
                }
                else
                {
                    // Updating the existing task
                    if (CurrentTask.Milestone!.Id == 0) CurrentTask.Milestone = null;
                    if (CurrentTask.Engineer!.Id == 0) CurrentTask.Engineer = null;
                    CurrentTask.Dependencies = DependenedTaskList.ToList();
                    s_bl.Task.UpdateTask(CurrentTask);
                    MessageBox.Show("The task has been updated successfully!");
                }
                // Closing the window
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is a problem: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { Close(); }
        }


        private void AddDependedTask_Click(object sender, RoutedEventArgs e)
        {
            TaskListWindow taskListWindow = new(true, CurrentTask.Id,
                ((task, id) =>
                {
                    var t = s_bl.Task.GetTask(id);
                    if (t.Id != task.Id)
                    {
                        t.Dependencies ??= new List<TaskInList>();
                        t.Dependencies.Add(task);
                        s_bl.Task.UpdateTask(t);
                    }
                }));

            taskListWindow.ShowDialog();

            CurrentTask = s_bl.Task.GetTask(CurrentTask.Id);
            DependenedTaskList = new (CurrentTask.Dependencies!);
        }

        private void AddDurationButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTask.Duration!.Value.Add(TimeSpan.FromDays(1));
            TaskDuration++;
        }

        private void SubDurationButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskDuration == 1) { MessageBox.Show("Task duration must be at list 1 day long!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            else
            {
                TaskDuration--;
                CurrentTask.Duration!.Value.Add(TimeSpan.FromDays(-1));

            }
        }
    }
}
