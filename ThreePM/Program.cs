using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ThreePM
{
	static class Program
	{
		private const int WM_USER = 0x400;
		[DllImport("User32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string [] args)
		{
			if (args.Length == 1)
			{
				IntPtr hWnd = IntPtr.Zero;

				// find the other instance
				Process process = Process.GetCurrentProcess();
				Process[] processes = Process.GetProcessesByName(process.ProcessName);
				foreach (Process _process in processes)
				{
					// Get the first instance that is not this instance, has the
					// same process name and was started from the same file name
					// and location. Also check that the process has a valid 
					// window handle in this session to filter out other user's
					// processes.
					if (_process.Id != process.Id &&
						_process.MainModule.FileName == process.MainModule.FileName &&
						_process.MainWindowHandle != IntPtr.Zero)
					{
						hWnd = _process.MainWindowHandle;
						break;
					}
				}
				if (hWnd != IntPtr.Zero)
				{
					if (MainForm.Commands.ContainsKey(args[0]))
					{
						SendMessage(hWnd, WM_USER, 0, (int)MainForm.Commands[args[0]]);
					}
					return;
				}
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			try
			{
				Application.Run(new MainForm());
			}
			catch (Exception e)
			{
				MessageBox.Show("Appliacation.Run Exception:\n\n" + e.ToString());
			}
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("Unhandled Exception:\n\n" + e.ExceptionObject.ToString());
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show("Thread Exception:\n\n" + e.Exception.ToString());
		}
	}
}