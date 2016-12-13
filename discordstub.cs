using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
 
namespace DiscordGameStub {
    class Program {
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
	
        static void Main(string[] args) {
			string gameTitle = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
			Console.Title = gameTitle;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("Title: "+Console.Title+"\n\nAfter Discord detects the game, press any key to hide window.\n");
			Console.ReadKey();
			
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_HIDE);
			Console.ReadKey();
        }
    }
}