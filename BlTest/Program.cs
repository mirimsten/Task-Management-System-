using DalTest;
using BO;
using System.Globalization;

namespace BlTest
{
    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        #region Input Functions
        private static object EnumInput(Type type, string message)
        {
            Console.WriteLine(message);
            return Enum.TryParse(type, Console.ReadLine(), out object? result) ? result : 0;
        }
        private static DateTime DateInput(string name)
        {
            Console.Write($"Enter {name} (yyyy-MM-dd): ");
            DateTime date;
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine($"Invalid input. Please enter {name} (yyyy-MM-dd): ");
            }
            return date;
        }
        private static TimeSpan SpanInput(string name)
        {
            Console.Write($"Enter {name} (yyyy-MM-dd): ");
            TimeSpan duration;
            while (!TimeSpan.TryParseExact(Console.ReadLine(), @"dd\.hh\:mm", null, out duration))
            {
                Console.WriteLine($"Invalid input. Please enter {name} (dd\\.hh\\:mm): ");
            }
            return duration;
        }
        private static double DoubleInput(string name)
        {
            Console.WriteLine($"Enter ${name}:");
            return double.Parse(Console.ReadLine() ?? "0");

        }
        private static int IntInput(string name)
        {
            Console.Write($"Enter {name}:");
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.WriteLine($"Invalid input. Please enter {name}: ");
            }
            return num;
        }
        private static string StringInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine() ?? "";
        }
        #endregion
        static void EngineerMenu()
        {
            while (true)
            {
                string choice = StringInput(

@"
Engineer menue: 
  1. add
  2. delete
  3. get
  4. get all 
  5. update
  6. assign a task
  7. back to main
");

                switch (choice)
                {
                    case "1":
                        try
                        {
                            int newEngineerId = s_bl.Engineer.CreateEngineer(new Engineer()
                            {
                                Id = IntInput("engineer ID"),
                                Name = StringInput("enter engineer's name:"),
                                Email = StringInput("enter engineer's email:"),
                                Level = (EngineerExperience)EnumInput(typeof(EngineerExperience), "Enter Complexity Level: "),
                                Cost = 200,
                                Task = null
                            });
                            Console.WriteLine($"Engineer with id: {newEngineerId} was succesfully created!");
                        }
                        catch (BlAlreadyExistsException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BlInvalidException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case "2":
                        try
                        {
                            s_bl.Engineer.DeleteEngineer(IntInput("engineer ID"));
                            Console.WriteLine($"Engineer was succesfully deleted!");
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case "3":

                        try
                        {
                            Console.WriteLine(s_bl.Engineer.GetEngineer(IntInput("engineer ID")));
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);

                        }
                        break;

                    case "4":

                        var engineers = s_bl.Engineer.ReadAllEngineers();
                        Console.WriteLine("All Engineers:");
                        foreach (var engineer in engineers)
                        {
                            Console.WriteLine(engineer);
                        }
                        break;

                    case "5":

                        int idUpdate = IntInput("engineer Id: ");
                        string NameUpdate = StringInput("Enter updated Name: ");
                        string EmailUpdate = StringInput("Enter updated Email: ");
                        EngineerExperience levelUpdate = (EngineerExperience)EnumInput(typeof(EngineerExperience), "Enter Complexity Level: ");
                        double CostUpdate = DoubleInput("engineer cost");
                        Engineer UpdateEngineer = new()
                        {
                            Id = idUpdate,
                            Name = NameUpdate,
                            Email = EmailUpdate,
                            Level = levelUpdate,
                            Cost = CostUpdate,
                        };
                        try
                        {
                            s_bl.Engineer.UpdateEngineer(UpdateEngineer);
                            Console.WriteLine($"Engineer with id: {idUpdate} was succesfully updated!");
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    
                    case "6":
                        try
                        {
                            Engineer engineer = s_bl.Engineer.GetEngineer(IntInput("engineer Id"));
                            engineer.Task = new() { Id = IntInput("task ID"), Alias = "" };
                            s_bl.Engineer.UpdateEngineer(engineer);
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BlIllegalException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "7":
                        Console.WriteLine("retuning to main");
                        return; 
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        static void TaskMenu()
        {
            while (true)
            {
                string choice = StringInput("\nTask Menu:\n 1.Add Task\n 2.Delete Task\n 3.Get Task Details\n 4.Get all Tasks\n 5.Update Task\n 6.Back to Main Menu");
                switch (choice)
                {
                    case "1":
                        string taskDescription = StringInput("Enter Task Description: ");
                        string taskAlias = StringInput("Enter Task Alias: ");
                        TimeSpan requiredEffortTime = SpanInput("Enter Required Effort Time: ");
                        string deliverables = StringInput("Enter Short Description for the Product: ");
                        string remarks = StringInput("Enter Remarks: ");
                        EngineerExperience complexityLevel = (EngineerExperience)EnumInput(typeof(EngineerExperience), "Enter Complexity Level: ");
                        try
                        {
                            s_bl.Task.CreateTask(new()
                            {
                                Description = taskDescription,
                                Alias = taskAlias,
                                Status = 0,
                                CreatedAtDate = DateTime.Now,
                                Duration = requiredEffortTime,
                                Deliverables = deliverables,
                                Remarks = remarks,
                                Complexity = complexityLevel,
                            });
                        }
                        catch (BlAlreadyExistsException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BlIllegalException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "2":
                        try
                        {
                            s_bl.Task.DeleteTask(IntInput("Enter Task ID to delete: "));
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BlIllegalException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "3":
                        try
                        {
                            Console.WriteLine(s_bl.Task.GetTask(IntInput("Enter Task ID to get details: ")));
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "4":
                        var tasks = s_bl.Task.ReadAllTasks();
                        Console.WriteLine("All Tasks:");
                        foreach (var task in tasks)
                        {
                            Console.WriteLine(task);
                        }
                        break;
                    case "5":
                        try
                        {
                            BO.Task updateTask = s_bl.Task.GetTask(IntInput("Enter Task ID: "));
                            string op = StringInput("What do you want to update?\r\n1. Dates\r\n2. Textual fields\r\n3. Dependencies\r\n4. Engineer assignment\r\n5. Level");
                            switch (op)
                            {
                                case "1":
                                    updateTask.StartDate = DateInput("start date");
                                    updateTask.CompleteDate = DateInput("complete date");
                                    break;
                                case "2":
                                    updateTask.Description = StringInput("Enter Task Description: ");
                                    updateTask.Alias = StringInput("Enter Task Alias: ");
                                    updateTask.Deliverables = StringInput("Enter Short Description for the Product: ");
                                    updateTask.Remarks = StringInput("Enter Remarks: ");
                                    break;
                                case "3":
                                    int dependency = IntInput("task Id");
                                    BO.Task task = s_bl.Task.GetTask(dependency);
                                    updateTask.Dependencies?.Add(new TaskInList()
                                    {
                                        Id = task.Id,
                                        Alias = task.Alias,
                                        Description = task.Description,
                                        Status = task.Status!.Value
                                    });
                                    break;
                                case "4":
                                    Engineer newEngineer = s_bl.Engineer.GetEngineer(IntInput("Enter Engineer Id: "));
                                    updateTask.Engineer = new EngineerInTask { Id = newEngineer.Id, Name = newEngineer.Name };
                                    break;
                                case "5":
                                    updateTask.Complexity = (EngineerExperience)EnumInput(typeof(EngineerExperience), "Enter Complexity Level: ");
                                    break;
                                default:
                                    Console.WriteLine("Back to menu...");
                                    break;
                            }
                            s_bl.Task.UpdateTask(updateTask);
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (BlIllegalException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "6":
                        Console.WriteLine("returning to main...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        static void MilestoneMenu()
        {
            while (true)
            {
                if (!s_bl.Milestone.ScheduleExists)
                {
                    Console.Write("Would you like to create schedual? (Y/N)");
                    if (Console.ReadLine() == "Y")
                    {
                        DateTime start = DateInput("project satrt date");
                        DateTime end = DateInput("project end date");

                        try
                        {
                            s_bl.Milestone.CreateSchedule(start, end);
                            Console.WriteLine("Project schedule created successfully!");
                        }
                        catch (BlIllegalException ex)
                        {
                            Console.WriteLine($"{ex.Message}, {ex.Details}");
                        }
                    }
                    return;
                }
                string choice = StringInput("\nMilestone Menu:\n 1.Get Milestone Details\n 2.Update Milestone\n 3.Back to Main Menu");

                switch (choice)
                {
                    case "1":
                        try
                        {
                            Console.WriteLine(s_bl.Milestone.GetMilestone(IntInput("milestone ID")));
                        }
                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case "2":
                        try
                        {
                            Console.WriteLine(s_bl.Milestone.UpdateMilestone(IntInput("milestone ID"), StringInput("Enter new alias:"), StringInput("Enter new description:"), StringInput("Enter new remarks:")));
                        }

                        catch (BlDoesNotExistException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "3":
                        Console.WriteLine("Exit...");
                        return; // retuning to the main menu
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        static void Main()
        {
            Console.Write("Would you like to create Initial data? (Y/N)");
            string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input");
            if (ans == "Y")
                Initialization.Do();
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Engineer");
                Console.WriteLine("2. Task");
                Console.WriteLine("3. Milestone");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        EngineerMenu();
                        break;

                    case "2":
                        TaskMenu();
                        break;

                    case "3":
                        MilestoneMenu();
                        break;

                    case "4":
                        Console.WriteLine("Exiting the program....");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

    }
}