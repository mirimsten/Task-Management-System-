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
    public class EngineerList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for EngineerListWindow.xaml
    /// </summary>
    public partial class EngineerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.EngineerExperience Level { get; set; } = BO.EngineerExperience.None;
        
        public ObservableCollection<EngineerList> EngineerList
        {
            get { return (ObservableCollection<EngineerList>)GetValue(EngineerListProperty); }
            set { SetValue(EngineerListProperty, value); }
        }
       
        public static readonly DependencyProperty EngineerListProperty =
            DependencyProperty.Register("EngineerList", typeof(ObservableCollection<EngineerList>), typeof(EngineerListWindow), new PropertyMetadata(null));       

        public EngineerListWindow()
        {
            InitializeComponent();
            EngineerList = new(s_bl.Engineer.ReadAllEngineers().Select(e => new EngineerList { Id = e.Id, Name = e.Name }));

        }

        private void LevelSelector_SelectionChanged(object sender, EventArgs e)
        {
            var engineerInLists = (Level == BO.EngineerExperience.None) ? 
                s_bl.Engineer.ReadAllEngineers() :
                s_bl.Engineer.ReadAllEngineers(e => (int)e.Level == (int)Level)!;

            ObservableCollection<EngineerList> newEngineerList = new(
                    engineerInLists.Select(e => new EngineerList { Id = e.Id, Name = e.Name }));
            EngineerList = newEngineerList;
        }

        private void ShowWindowAddEngineer_Click(object sender, RoutedEventArgs e)
        {
            var singleEngineerWindow = new Engineer.SingleEngineerWindow();
            singleEngineerWindow.Closed += LevelSelector_SelectionChanged!;
            singleEngineerWindow.ShowDialog();
        }

        private void ToUpdateEngineer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EngineerList engineerInList = ((sender as ListView)!.SelectedItem as EngineerList)!;
            var singleEngineerWindow = new Engineer.SingleEngineerWindow(engineerInList.Id);
            singleEngineerWindow.Closed += LevelSelector_SelectionChanged!;
            singleEngineerWindow.ShowDialog();
        }
    }
}