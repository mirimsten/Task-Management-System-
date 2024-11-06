namespace DalTest;
using DalApi;
using DO;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class Initialization
{
    private static IDal? s_dal; //stage 2
    public static void Do()
    {
        s_dal = Factory.Get; //stage 4
        CraeteEngineers();
        CraeteTask();
        CraeteDependency();
    }

    private static readonly Random s_rand = new();
    private static void CraeteEngineers()
    {
        string[] engineerNames =
        {
            "Raizy Gutman", "Yeudit Itamar", "Dani Levi", "Eli Amar", "Yair Cohen",
            "Ariela Levin", "Dina Klein", "Shira Israelof"
        };
        //variable for engineer level, so that engineers of all levels will be created.
        int level = 0;
        for (int i = 0; i < engineerNames.Length; i++)
        {
            int _id = s_rand.Next(200000000, 400000000);
            string _email = engineerNames[i].Replace(" ", "") + "@gmail.com";
            double _cost = s_rand.Next(300, 700);
            EngineerExperience _level = (EngineerExperience)((level++) % 5);
            Engineer newEngineer = new(_id, engineerNames[i], _email, _level, _cost);
            try
            {
                s_dal!.Engineer.Create(newEngineer);
            }
            catch (DalAlreadyExistsException)
            {
                i--;
            }
        }
    }

    private static void CraeteTask()
    {
        string[] Descriptions = {
    "Define Project Scope",
    "Create Wireframes",
    "Develop Database Schema",
    "Set Up Version Control",
    "Implement User Authentication",
    "Design UI/UX",
    "Develop Frontend",
    "Develop Backend",
    "Integrate Frontend and Backend",
    "Implement API Endpoints",
    "Implement Error Handling",
    "Perform Unit Testing",
    "Conduct System Testing",
    "Optimize Performance",
    "Create Documentation",
    "Deploy to Testing Environment",
    "User Acceptance Testing (UAT)",
    "Address UAT Feedback",
    "Perform Security Audit",
    "Prepare for Production Deployment",
    "Deploy to Production",
    "Monitor Application Performance",
    "Provide User Training",
    "Create Marketing Materials",
    "Launch Marketing Campaign",
    "Collect User Feedback",
    "Implement Feature Requests"
};
        string[] Aliases = {
    "Gather requirements and define the scope of the project.",
    "Design the basic layout and structure of the application.",
    "Design the database structure for the application.",
    "Implement version control system (e.g., Git) for collaborative development.",
    "Set up user authentication and authorization functionality.",
    "Create a visually appealing and user-friendly interface design.",
    "Write code for the client-side/frontend of the application.",
    "Write code for the server-side/backend of the application.",
    "Combine frontend and backend components to create a functional application.",
    "Create API endpoints for communication between frontend and backend.",
    "Handle errors gracefully to enhance user experience.",
    "Test individual components and functions for correctness.",
    "Test the entire system to ensure all components work together seamlessly.",
    "Optimize code and database queries for better performance.",
    "Document code, APIs, and system architecture for future reference.",
    "Deploy the application to a testing environment for user acceptance testing.",
    "Allow end-users to test the application and provide feedback.",
    "Incorporate user feedback and make necessary adjustments.",
    "Conduct a security audit to identify and address vulnerabilities.",
    "Finalize configurations and prepare the application for production deployment.",
    "Deploy the application to the production environment for public use.",
    "Set up monitoring tools to track and analyze application performance.",
    "Train end-users on how to use the new application effectively.",
    "Prepare marketing materials to promote the new application.",
    "Execute a marketing campaign to introduce the new application to the target audience.",
    "Gather feedback from users after the application is in use.",
    "Incorporate user-requested features based on collected feedback."
};
        string[] Deliverables = {
    "Project Scope Document",
    "Wireframes",
    "Database Schema",
    "Version Control System",
    "User Authentication System",
    "UI/UX Designs",
    "Frontend Codebase",
    "Backend Codebase",
    "Integrated Application",
    "API Endpoints",
    "Error Handling Mechanism",
    "Unit Test Reports",
    "System Test Reports",
    "Optimized Application",
    "Documentation",
    "Testing Environment Deployment",
    "UAT Reports",
    "Revised Application",
    "Security Audit Report",
    "Production-ready Application",
    "Production Deployment",
    "Monitoring System",
    "User Training Materials",
    "Marketing Materials",
    "Marketing Campaign",
    "User Feedback Reports",
    "Enhanced Application"
};
        for (int i = 0; i < Descriptions.Length; i++)
        {
            DateTime createdAtDate = DateTime.Now.AddDays(0 - s_rand.Next(0, 14));
            TimeSpan duration = new(s_rand.Next(1, 4), 0, 0, 0);
            EngineerExperience complexityLevel = (EngineerExperience)s_rand.Next(0, 5);
            DO.Task newTask = new(0, Descriptions[i], Aliases[i], false, createdAtDate, null, null, duration, null, null, Deliverables[i], "", null, complexityLevel);
            s_dal!.Task.Create(newTask);
        }
    }

    private static void CraeteDependency()
    {
        int[][] dependencyArray = new int[][] {
    Array.Empty<int>(),
    new int[]{1},
    new int[]{1},
    new int[]{2,3},
    new int[]{4},
    new int[]{4},
    new int[] {6},
    new int[] {6},
    new int[] {7,8},
    new int[] {5 },
    new int[] {5 },
    new int[] {10,11},
    new int[] {9,12 },
    new int[] {13 },
    new int[] {13 },
    new int[] {14,15 },
    new int[] { 14,15 },
    new int[] {17},
    new int[] {16},
    new int[] {18,19 },
    new int[] { 20 },
    new int[] {21},
    new int[] { 21 },
    new int[] { 22 },
    new int[] { 24 },
    new int[] { 23 },
    new int[] { 25,26 },
};

        for (int i = 0; i < dependencyArray.Length; i++)
        {
            foreach (int j in dependencyArray[i])
            {
                s_dal!.Dependency.Create(new(0, i + 1, j));
            }
        }
    }

    public static void Reset()
    {
        s_dal = Factory.Get;

        s_dal!.Engineer.Reset();
        s_dal!.Task.Reset();
        s_dal!.Dependency.Reset();
    }
}
