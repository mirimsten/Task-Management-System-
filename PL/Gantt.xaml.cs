using PL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
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

namespace PL
{
    public class GanttRecord
    {
        public int id {  get; set; }
        public string alias { get; set; }
        public Color color { get; set; }
        public int startIndex { get; set; }
        public int span { get; set; }
        public bool warning { get; set; }
        public bool notStarted { get; set; }

        public GanttRecord(int id, string alias, Color color, int start, int end, bool warning, bool notStarted)
        {
            this.id = id;
            this.alias = alias;
            this.color = color;
            this.startIndex = start;
            this.span = end;
            this.warning = warning;
            this.notStarted = notStarted;
        }
    }

    public partial class Gantt : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        readonly Random random = new();
        readonly DateTime Current = s_bl.CurrentDate.ToDateTime(new TimeOnly());

        public ObservableCollection<GanttRecord> RecordsList
        {
            get { return (ObservableCollection<GanttRecord>)GetValue(RecordsListProperty); }
            set { SetValue(RecordsListProperty, value); }
        }

        public static readonly DependencyProperty RecordsListProperty =
            DependencyProperty.Register("RecordsList", typeof(ObservableCollection<GanttRecord>), typeof(Gantt), new PropertyMetadata(null));


        public int DaysCount
        {
            get { return (int)GetValue(DaysCountProperty); }
            set { SetValue(DaysCountProperty, value); }
        }

        public static readonly DependencyProperty DaysCountProperty =
            DependencyProperty.Register("DaysCount", typeof(int), typeof(Gantt), new PropertyMetadata(null));


        public List<DateTime> DatesList
        {
            get { return (List<DateTime>)GetValue(DatesListProperty); }
            set { SetValue(DatesListProperty, value); }
        }

        public static readonly DependencyProperty DatesListProperty =
            DependencyProperty.Register("DatesList", typeof(List<DateTime>), typeof(Gantt), new PropertyMetadata(null));

        public Gantt()
        {
            InitializeComponent();
            RecordsList = new(CreateRecords());
        }

        public List<GanttRecord> CreateRecords()
        {
            List<GanttRecord> ganttRecords = new();

            List<BO.Milestone> milestones = s_bl.Milestone.GetAllMilestones()
                .Select(m => s_bl.Milestone.GetMilestone(m.Id)).OrderBy(t => t.SchedualDate).ToList();

            var datesList = GetDateRange((DateTime)s_bl.Start()!, (DateTime)s_bl.End()!).ToList();

            DatesList = datesList;
            DaysCount = datesList.Count;

            foreach (BO.Milestone milestone in milestones)
            {
                Color color = GetRandomColor();
                bool warning = Current > milestone.DeadlineDate && milestone.Status < BO.Status.Done;
                bool notyet = Current > milestone.SchedualDate;

                ganttRecords.Add(new(milestone.Id, milestone.Alias, Colors.White, 0, datesList.Count, warning, notyet));

                List<BO.Task> tasks = s_bl.Task.ReadAllTasks()
               .Select(t => s_bl.Task.GetTask(t.Id)).Where(t => t.Milestone!.Id == milestone.Id).ToList();

                foreach (var task in tasks)
                {
                    int start = datesList.FindIndex(d => d.Date == task.SchedualDate!.Value.Date);
                    int span = task.DeadlineDate!.Value.Subtract(task.SchedualDate!.Value).Days;
                    bool war = Current > task.DeadlineDate && task.Status < BO.Status.Done;
                    bool early = Current > task.SchedualDate;
                    ganttRecords.Add(new(task.Id, task.Description, color, start, span, war, early));
                }
            }
            return ganttRecords;
        }


        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }

        public Color GetRandomColor()
        {
            // Generate random RGB values
            byte red = (byte)random.Next(0, 256);
            byte green = (byte)random.Next(0, 256);
            byte blue = (byte)random.Next(0, 256);

            return Color.FromRgb(red, green, blue);
        }
    }
}

