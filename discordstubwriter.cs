// mcs discordstubwriter.cs /reference:System.Windows.Forms /reference:System.Drawing
		static string currentPath = AppDomain.CurrentDomain.BaseDirectory;
		static string steamRoot = @"C:\Program Files (x86)\Steam\steamapps\common"; // Maybe use a prefs file for this?
		static void Main(string[] args) {
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
				NewGame(newtitle);
			} else {
		
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
				NewGame(newtitle);
			} else {
		} // End NewGame
		
		static void Abort(){
		} // End Abort