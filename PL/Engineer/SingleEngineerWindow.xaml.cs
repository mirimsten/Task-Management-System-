using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Engineer
{
    /// <summary>
    /// Interaction logic for SingleEngineerWindow.xaml
    /// </summary>
    public partial class SingleEngineerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
         
        public BO.Engineer CurrentEngineer
        {
            get { return (BO.Engineer)GetValue(CurrentEngineerProperty); }
            set { SetValue(CurrentEngineerProperty, value); }
        }

        public static readonly DependencyProperty CurrentEngineerProperty =
            DependencyProperty.Register("CurrentEngineer", typeof(BO.Engineer), typeof(SingleEngineerWindow), new PropertyMetadata(null));

        public SingleEngineerWindow(int Id = 0)
        {
              InitializeComponent();
            if (Id == 0)
            {
                CurrentEngineer = new BO.Engineer
                {
                    Task = new BO.TaskInEngineer()
                };

            }
            else
            {
                try
                {
                    CurrentEngineer = s_bl.Engineer.GetEngineer(Id);
                    if (CurrentEngineer.Task == null) CurrentEngineer.Task = new BO.TaskInEngineer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void BtnAddOrUpdateEngineer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            try
            {
                if ((string)button!.Content == "Add")
                {
                    // Creating the new engineer in the BL

                    s_bl.Engineer.CreateEngineer(CurrentEngineer);
                    MessageBox.Show("The engineer has been added successfully!");
                }
                else
                {
                    // Updating the existing engineer
                    if (CurrentEngineer.Task!.Id == 0) CurrentEngineer.Task = null;
                    s_bl.Engineer.UpdateEngineer(CurrentEngineer);
                    MessageBox.Show("The engineer has been updated successfully!");
                }
                // Closing the window
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is a problem: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { Close(); }
        }
    }
}
