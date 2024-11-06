using BO;
using DO;
using PL.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PL.Engineer
{
    /// <summary>
    /// Interaction logic for EngineerView.xaml
    /// </summary>
    public partial class EngineerView : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        #region Dependency Properties

        public BO.Engineer Engineer { get; set; }

        public Visibility ShowTask
        {
            get { return (Visibility)GetValue(ShowTaskProperty); }
            set { SetValue(ShowTaskProperty, value); }
        }

        public static readonly DependencyProperty ShowTaskProperty =
            DependencyProperty.Register("ShowTask", typeof(Visibility), typeof(EngineerView), new PropertyMetadata(null));

        public BO.Task CurrentTask
        {
            get { return (BO.Task)GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(EngineerView), new PropertyMetadata(null));

        #endregion

        public EngineerView(int Id)
        {
            InitializeComponent();
            Engineer = s_bl.Engineer.GetEngineer(Id);
            if (Engineer.Task is not null)
            {
                try
                {
                    CurrentTask = s_bl.Task.GetTask(Engineer.Task!.Id);
                    ShowTask = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("You don't have a task yet! Please select a task from the list to continue.", "No Task", MessageBoxButton.OK, MessageBoxImage.Error);
                NewTaskButton_Click(new object(),new RoutedEventArgs());
            }

        }

        private void NewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = s_bl.Task
                .ReadAllTasks()
                .Select(t => s_bl.Task.GetTask(t.Id))
                .Where(t => t.Complexity <= Engineer.Level && t.Engineer == null && s_bl.Milestone.GetMilestone(t.Milestone!.Id).Status == Status.Done)
                .Select(t => t.Id);

            var listTasks = s_bl.Task.ReadAllTasks(t => tasks.Contains(t.Id)).ToList();

            TaskListWindow taskListWindow = new(true, Engineer.Id,
               ((task, id) =>
               {
                   var e = s_bl.Engineer.GetEngineer(id);
                   e.Task = new() { Id = task.Id, Alias = task.Alias };
                   s_bl.Engineer.UpdateEngineer(e);
               }), listTasks);

            taskListWindow.ShowDialog();

            Engineer = s_bl.Engineer.GetEngineer(Engineer.Id);

            CurrentTask = s_bl.Task.GetTask(Engineer.Task!.Id);
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTask.CompleteDate = s_bl.CurrentDate.ToDateTime(new TimeOnly());
            s_bl.Task.UpdateTask(CurrentTask);
            CurrentTask = s_bl.Task.GetTask(CurrentTask.Id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e) => s_bl.Task.UpdateTask(CurrentTask);
    }
}
