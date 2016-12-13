// mcs discordstubwriter.cs /reference:System.Windows.Forms /reference:System.Drawing

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
 
namespace DiscordGameStubWriter {
    class Program {
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		
		[DllImport("user32.dll")]
		static extern IntPtr GetShellWindow();
		
		[DllImport("user32.dll")]
		static extern IntPtr GetDesktopWindow(); 
		
		[DllImport("kernel32.dll", ExactSpelling = true)]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		
		[DllImport("user32.dll")]
		private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow);
		private const int SW_MINIMIZE = 6;
		
		static NotifyIcon notifyIcon;
		static IntPtr processHandle;
		static IntPtr WinShell;
		static IntPtr WinDesktop;
		
		static string currentPath = AppDomain.CurrentDomain.BaseDirectory;
		static string steamRoot = @"C:\Program Files (x86)\Steam\steamapps\common"; // Maybe use a prefs file for this?
		static bool isVisible = true;
		
		static void Main(string[] args) {
			notifyIcon = new NotifyIcon();
			notifyIcon.Icon = new Icon("HitBlock.ico");
			notifyIcon.Text = "Discord Stub Writer";
			notifyIcon.Visible = true;
			notifyIcon.Click +=new System.EventHandler(VisibilityToggle);
			
			Task.Factory.StartNew(Run);
			
			processHandle = Process.GetCurrentProcess().MainWindowHandle;
			WinShell = GetShellWindow();
			WinDesktop = GetDesktopWindow();

			Application.Run();
		} // End Main
		
        static void Run() {
			//Console.WriteLine(currentPath);
			//Console.WriteLine(steamRoot);
			//Console.WriteLine(new System.IO.DirectoryInfo(steamRoot).Name);
   
			// Main does some prep and calls NewGame, NewGame will call itself until the user exits the program.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Title = "Discord Stub Writer";
			Console.Write("Currently playing: \nPress enter to send a game to Discord or leave blank to exit.\n\nNew game: ");
			string newtitle = Console.ReadLine();
			if (newtitle != "") {
				Console.Title = newtitle+" - Discord Stub Writer";
				NewGame(newtitle);
			} else {
				Abort();
			}
		} // End Run
		
		static void NewGame(string gametitle){
			bool alreadyExisted;
			
			string fileName = "discordstub.exe";
			string steamPath = System.IO.Path.Combine(steamRoot, gametitle);
			string sourceFile = System.IO.Path.Combine(currentPath, fileName);
			string destFile = System.IO.Path.Combine(steamPath, fileName);
			
			// Check if the folder already exists, create it if it doesn't.
			// We need the boolean later to avoid accidentally 'cleaning up' folders that had real games in them already.
			if (!System.IO.Directory.Exists(steamPath)) {
				alreadyExisted = false;
				System.IO.Directory.CreateDirectory(steamPath);
			} else {
				alreadyExisted = true;
			}
			
			System.IO.File.Copy(sourceFile, destFile, true);
			System.Diagnostics.Process.Start(destFile);
			
			// Prep the new prompt.
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("Currently playing: "+gametitle+"\nPress enter to send a game to Discord or leave blank to exit.\n\nNew game: ");
			string newtitle = Console.ReadLine();
			
			// File and folder cleanup below this comment
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("Cleaning up after ourselves...\n\n");
			
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("discordstub");
			foreach (var process in processes) {
				process.Kill();
				process.WaitForExit();
			}
			
			if (System.IO.File.Exists(destFile) & alreadyExisted) {
				try {
					System.IO.File.Delete(destFile);
				} catch (System.IO.IOException e) {
					Console.WriteLine(e.Message);
					Abort();
				}
			} else if (System.IO.Directory.Exists(steamPath) & !alreadyExisted) {
				try {
					System.IO.Directory.Delete(steamPath, true);
				} catch (System.IO.IOException e) {
					Console.WriteLine(e.Message);
					Abort();
				}
			} else {
				Abort();
			}
			
			if (newtitle != "") {
				Console.Title = newtitle+" - Discord Stub Writer";
				NewGame(newtitle);
			} else {
				Abort();
			}
		} // End NewGame
		
		static void Abort(){
			notifyIcon.Visible = false;
			Application.Exit();
			Environment.Exit(1);
		} // End Abort
		
		static void VisibilityToggle(object sender, System.EventArgs e) {
			if (isVisible) {
				SetParent(processHandle, WinShell);
				ShowWindow(processHandle, SW_MINIMIZE);
				notifyIcon.Icon = new Icon("QuestionBlock.ico");
			} else {
				SetParent(processHandle, WinDesktop);
				SetForegroundWindow(GetConsoleWindow());
				notifyIcon.Icon = new Icon("HitBlock.ico");
			}
			isVisible = !isVisible;
		} // End VisibilityToggle
	} // End Prog
}
