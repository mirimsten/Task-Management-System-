using BO;
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

namespace PL.Milestone
{
    /// <summary>
    /// Interaction logic for MilestoneList.xaml
    /// </summary>
    public partial class MilestoneList : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MilestoneInList SelectedMilestone { get; set; }

        public BO.Status Status { get; set; } = BO.Status.Unscheduled;

        #region Dependency Properties

        public ObservableCollection<MilestoneInList> Milestones
        {
            get { return (ObservableCollection<MilestoneInList>)GetValue(MilestonesProperty); }
            set { SetValue(MilestonesProperty, value); }
        }

        public static readonly DependencyProperty MilestonesProperty =
            DependencyProperty.Register("Milestones", typeof(ObservableCollection<MilestoneInList>), typeof(MilestoneList), new PropertyMetadata(null));

        public string SearchValue
        {
            get { return (string)GetValue(SearchValueProperty); }
            set { SetValue(SearchValueProperty, value); }
        }

        public static readonly DependencyProperty SearchValueProperty =
            DependencyProperty.Register("SearchValue", typeof(string), typeof(MilestoneList), new PropertyMetadata(null));

        #endregion

        public MilestoneList()
        {
            InitializeComponent();
            Milestones = new(s_bl.Milestone.GetAllMilestones());
            SearchValue = "";
        }

        private void StatusSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Milestones = new(s_bl.Milestone.GetAllMilestones(m => m.Status == Status));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Milestones = new(s_bl.Milestone.GetAllMilestones(m => (m.Alias.Contains(SearchValue)) || m.Description.Contains(SearchValue)));
            SearchValue = "";
        }

        private void MilstoneDetails(object sender, RoutedEventArgs e)
        {
            new PL.Milestone.Milestone(SelectedMilestone.Id).ShowDialog();
            Milestones = new(s_bl.Milestone.GetAllMilestones());
        }
    }
}
