using System;
using System.Collections;
using System.Runtime.InteropServices;
using TaskSchedulerInterop;

namespace TaskScheduler
{


//    //Get a ScheduledTasks object for the local computer.  
//    ScheduledTasks st = new ScheduledTasks();

//    // Create a task  
//    Task t;  
//try  
//{  
//    st.DeleteTask("Editer");  
//    t = st.CreateTask("Editer");  
//}  
//catch (ArgumentException e)  
//{  
//    Console.WriteLine("Task name already exists");  
//    return;  
//}  
  
//// Fill in the program info  
//t.ApplicationName = @"C:\Windows\system32\notepad.exe";  
//t.Comment = "Open notepad to do some editing.";  
  
  
//// Declare that the task runs only for the user who created it.  
//// And will run even if the system goes to sleep mode.  
//t.Flags = TaskFlags.RunOnlyIfLoggedOn | TaskFlags.SystemRequired;  
  
//// Set the account under which the task should run.  
//t.SetAccountInformation(Environment.UserName, (string)null);  
  
//// Declare that the system must have been idle for ten minutes before   
//// the task will start  
//t.IdleWaitMinutes = 10;  
  
//// Allow the task to run permanently.  
//t.MaxRunTimeLimited = false;  
  
//// Set priority to only run when system is idle.  
//t.Priority = System.Diagnostics.ProcessPriorityClass.Idle;  
  
//// Create a repeated RunOnce trigger with duration 0x196e6a minutes and interval 1 minute.  
//// 0x196e6a is the maximum minutes we can set because XP dose not support indefinitely setting.  
//RunOnceTrigger runOnceTrigger = new RunOnceTrigger(DateTime.Now);
//runOnceTrigger.DurationMinutes = 0x196e6a;  
//runOnceTrigger.IntervalMinutes = 1;  
  
////t.Triggers.Add(new DailyTrigger(17, minute));  
//t.Triggers.Add(runOnceTrigger);  
  
//// Save the changes that have been made.  
//t.Save();  
//// Close the task to release its COM resources.  
//t.Close();  
//// Dispose the ScheduledTasks to release its COM resources.  
//st.Dispose();  

	/// <summary>
	/// Deprecated.  For V1 compatibility only. 
	/// </summary>
	/// <remarks>
	/// <p>Scheduler is just a wrapper around the TaskList class.</p>
	/// <p><i>Provided for compatibility with version one of the library.  Use of Scheduler
	/// and TaskList will normally result in COM memory leaks.</i></p>
	/// </remarks>
	public class Scheduler
	{
		/// <summary>
		/// Internal field which holds TaskList instance
		/// </summary>
		private readonly TaskList tasks = null;

		/// <summary>
		/// Creates instance of task scheduler on local machine
		/// </summary>
		public Scheduler()
		{
			tasks = new TaskList();
		}

		/// <summary>
		/// Creates instance of task scheduler on remote machine
		/// </summary>
		/// <param name="computer">Name of remote machine</param>
		public Scheduler(string computer)
		{
			tasks = new TaskList();
			TargetComputer = computer;
		}

		/// <summary>
		/// Gets/sets name of target computer. Null or emptry string specifies local computer.
		/// </summary>
		public string TargetComputer
		{
			get
			{
				return tasks.TargetComputer;
			}
			set
			{
				tasks.TargetComputer = value;
			}
		}

		/// <summary>
		/// Gets collection of system tasks
		/// </summary>
		public TaskList Tasks
		{
			get
			{
				return tasks;
			}
		}

	}
}
