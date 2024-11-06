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
using System.Windows.Shapes;

namespace PL.Milestone
{
    /// <summary>
    /// Interaction logic for Milestone.xaml
    /// </summary>
    public partial class Milestone : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Milestone CurrentMilestone
        {
            get { return (BO.Milestone)GetValue(CurrentMilestoneProperty); }
            set { SetValue(CurrentMilestoneProperty, value); }
        }

        public static readonly DependencyProperty CurrentMilestoneProperty =
            DependencyProperty.Register("CurrentMilestone", typeof(BO.Milestone), typeof(Milestone), new PropertyMetadata(null));

        public Milestone(int id)
        {
            InitializeComponent();
            CurrentMilestone = s_bl.Milestone.GetMilestone(id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
                CurrentMilestone = s_bl.Milestone.UpdateMilestone(CurrentMilestone.Id, CurrentMilestone.Alias, CurrentMilestone.Description, "");
                MessageBox.Show("Milestone successfully updated!", "OK", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
