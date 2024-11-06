using PL.Engineer;
using System;
using System.Linq;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private string SenderMode = "manager";
       
        #region Dependency Properties

        public Visibility InputMode
        {
            get { return (Visibility)GetValue(InputModeProperty); }
            set { SetValue(InputModeProperty, value); }
        }

        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.Register("InputMode", typeof(Visibility), typeof(MainWindow), new PropertyMetadata(null));


        public DateOnly CurrentDate
        {
            get { return (DateOnly)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateOnly), typeof(MainWindow), new PropertyMetadata(null));

        public string CurrentID
        {
            get { return (string)GetValue(CurrentIDProperty); }
            set { SetValue(CurrentIDProperty, value); }
        }

        public static readonly DependencyProperty CurrentIDProperty =
            DependencyProperty.Register("CurrentID", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            CurrentDate = s_bl.CurrentDate;
            InputMode = Visibility.Hidden;
            CurrentID = "";
        }

        public void AddDay(object sender, RoutedEventArgs e) { s_bl.AddDay(); CurrentDate = s_bl.CurrentDate; }

        public void ResetDate(object sender, RoutedEventArgs e) { s_bl.ResetDate(); CurrentDate = s_bl.CurrentDate; }

        private void ButtonEngineer_Click(object sender, RoutedEventArgs e)
        {
            SenderMode = "engineer";
            InputMode = Visibility.Visible;
        }

        private void ButtonManager_Click(object sender, RoutedEventArgs e)
        {
            SenderMode = "manager";
            InputMode = Visibility.Visible;
        }

        private void SendIdButton_Click(object sender, RoutedEventArgs e)
        {
            InputMode = Visibility.Hidden;
            var engineersId = s_bl.Engineer.ReadAllEngineers().Select(x => x.Id).ToList();
            if (!int.TryParse(CurrentID, out int id)) MessageBox.Show("An ID must contains only 9 digits!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (engineersId.Count > 0 && !engineersId.Contains(id)) MessageBox.Show("Wrong Id number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (SenderMode == "engineer")
                {
                    if(s_bl.Status() != BO.ProjectStatus.Execution)
                    {
                        MessageBox.Show("Do not approach the task or assign a task to the engineer before the project begins!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return;
                    }
                    try
                    {
                        s_bl.Engineer.GetEngineer(id);
                        new Engineer.EngineerView(id).Show();
                    }
                    catch
                    {
                        MessageBox.Show("Wrong engineer Id number!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (id == 325907210) new PL.Manager().Show();
                    else MessageBox.Show("Wrong manager Id number!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
