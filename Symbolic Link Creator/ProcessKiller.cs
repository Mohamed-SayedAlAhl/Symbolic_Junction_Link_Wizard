using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Diagnostics;
using System.IO;

using System.Windows.Forms;

namespace SymbolicLinkCreator
{
    public class ProcessKiller
    {

        private static string currentProcessId = "";
        private static string LinkType="";
        private static string FolderToDeletePath = "";


        public static string FullMessage = "";

        public static List<string> AppNames = new List<string>();
        public static List<int> Pids = new List<int>();


        public static List<string> Files = new List<string>();
        public static List<string> Folders = new List<string>();

        public static List<string> ActiveApps = new List<string>();

        public static List<bool> AcceptedCloseApps = new List<bool>();

        public static List<bool> isSubFolder = new List<bool>();

        public enum condition : short
        {
            Accepted = 1,
            Refused = 2,
            ProcessNotFound = 3,
            ProcessWasFound = 4,
            HandleNotFound = 5,
            ProcessKillFailed = 6
        }

        public static void AddtoAcceptedCloseApps(bool newBool)
        {
            // Add the new boolean to the list

            AcceptedCloseApps.Add(newBool);
        }

        public static string GetActiveAppNamesInUse()
        {


            return string.Join(",", ActiveApps);



        }



        // the folder path where handle.exe will be extracted
        private static string symbolicFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".symbolic link wizard");

        // Method to extract the embedded Handle.exe resource to the .delete it folder
        private static string ExtractHandleExe()
        {
            // Define the path to where handle.exe will be extracted in the .delete it folder
            string handleExePath = Path.Combine(symbolicFolderPath, "handle.exe");

            // Ensure the folder exists
            if (!Directory.Exists(symbolicFolderPath))
            {
                Directory.CreateDirectory(symbolicFolderPath);
            }

            // Extract handle.exe if it doesn't already exist in the .delete it folder
            if (!File.Exists(handleExePath))
            {
                // Write the embedded resource to the file
                File.WriteAllBytes(handleExePath, Symbolic_Link_Creator.Properties.Resources.handle);


                AcceptingHandleEula(handleExePath);
            }

            return handleExePath; // Return the path to the extracted handle.exe
        }



        private static void AcceptingHandleEula(string handleExePath)
        {
            // Execute handle.exe with /accepteula argument first to accept the EULA
            Process acceptEulaProcess = new Process();
            acceptEulaProcess.StartInfo.FileName = handleExePath;
            acceptEulaProcess.StartInfo.Arguments = "/accepteula"; // Automatically accept the EULA
            acceptEulaProcess.StartInfo.UseShellExecute = false;
            acceptEulaProcess.StartInfo.CreateNoWindow = true;

            // Run the process to accept the EULA silently
            acceptEulaProcess.Start();
            acceptEulaProcess.WaitForExit();
        }

        private static void SaveProcessesDataForFolder(string processId, string filePath)
        {

            string FileName = Path.GetFileNameWithoutExtension(filePath);

            if (!FileName.StartsWith("~$") && !Files.Contains(FileName))
            {


                // Getting the Folder path of the file
                string FolderPath = Path.GetDirectoryName(filePath);
                // Checking if folder is sub directory
                bool ab = FolderPath.StartsWith(FolderToDeletePath, StringComparison.OrdinalIgnoreCase) && !string.Equals(FolderPath, FolderToDeletePath, StringComparison.OrdinalIgnoreCase); ;

                isSubFolder.Add(ab);

                string FolderName = Path.GetFileName(FolderPath);
                Folders.Add(FolderName);
                Files.Add(FileName);

                int pid = int.Parse(processId);
                Pids.Add(pid);
                Process process = Process.GetProcessById(pid);


                string App = GetApplicationName(process, filePath);
                string appName = (App == "" ? process.ProcessName : App);
                AppNames.Add(appName);

            }



        }
        private static void ClearOldDataSavedFromPreviousProcess()
        {
            Folders.Clear();
            Files.Clear();
            Pids.Clear();
            AppNames.Clear();
            ActiveApps.Clear();

            AcceptedCloseApps.Clear();


        }
        private static string ConstructFullProcessMessage()
        {
            FullMessage = "";
            for (int i = 0; i < Pids.Count; i++)
            {
                string FolderPosition = isSubFolder[i] ? $"inside a subfolder:'{Folders[i]}' inside the selected folder." : $"inside the selected folder:'{Folders[i]}'.";
                FullMessage += $"\nApp:'{AppNames[i]}' is using the file: '{Files[i]}' {FolderPosition}";
            }

            return FullMessage;
        }
        private static DialogResult ShowFullProcessMessageForFolders()
        {
            string Message = ConstructFullProcessMessage();

            DialogResult dialogResult = CustomMessageBox.Show($"{Message}\n Do you want to close all apps to continue creating the {LinkType}?",
                "Files in Use", MessageBoxIcon.Warning, false, "Close All", "No");


            return dialogResult;
        }

        private static condition killAllProcesses()
        {
            Process process;
            foreach (int pid in Pids)
            {
                try
                {
                    process = Process.GetProcessById(pid);

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch (ArgumentException)
                {


                }

            }

            return condition.Accepted;
        }

        private static void ShowUserEachProcessAlone()
        {
            DialogResult dialogResult;
            Process process;

            for (int i = 0; i < Pids.Count; i++)
            {
                try
                {
                    process = Process.GetProcessById(Pids[i]);
                }
                catch (ArgumentException)
                {
                    continue;

                }

                string FolderPosition = isSubFolder[i] ? $"inside a subfolder:'{Folders[i]}' inside the selected folder" : $"inside the selected folder:'{Folders[i]}'";

                dialogResult = CustomMessageBox.Show($"App: '{AppNames[i]}' is using the file: '{Files[i]}' {FolderPosition}.\nDo you want to close this app to continue creating the {LinkType}?",
              "Files in Use", MessageBoxIcon.Warning, false, "Close", "No");

                if (dialogResult == DialogResult.Yes)
                {

                    if (!process.HasExited)
                    {
                        process.Kill();

                    }
                    AddtoAcceptedCloseApps(true);
                }


                else
                {
                    ActiveApps.Add(AppNames[i]);
                }

            }
        }

        public static int CheckIfTxtFileIsOpen()
        {


            Process[] processes = Process.GetProcessesByName("notepad");
            return processes.Count();

        }

        public static condition checkCurrentProcess(string filePath)
        {
            try
            {


                // Path to handle.exe (adjust if necessary)
                string handleExePath = ExtractHandleExe(); 

                if (!File.Exists(handleExePath))
                {
                    CustomMessageBox.Show("Handle.exe not found!", "Error", MessageBoxIcon.Error, true, "OK");
                    return condition.HandleNotFound;
                }

                // Execute handle.exe with file path as argument
                Process handleProcess = new Process();
                handleProcess.StartInfo.FileName = handleExePath;
                handleProcess.StartInfo.Arguments = $"\"{filePath}\""; // Pass the file path as an argument
                handleProcess.StartInfo.RedirectStandardOutput = true;
                handleProcess.StartInfo.UseShellExecute = false;
                handleProcess.StartInfo.CreateNoWindow = true;

                handleProcess.Start();

                // Read output from handle.exe
                string output = handleProcess.StandardOutput.ReadToEnd();
                handleProcess.WaitForExit();

                // Find the process ID (PID) in handle.exe output
                string processId = ExtractProcessId(output, filePath);
                if (!string.IsNullOrEmpty(processId))
                {

                    currentProcessId = processId;
                    return condition.ProcessWasFound;
                }

                else
                    return condition.ProcessNotFound;


            }
            catch (Exception)
            {
                return condition.ProcessNotFound;
            }

        }

        public static condition CloseProcessUsingFile(string filePath)
        {

            condition currentCondition = checkCurrentProcess(filePath);
            if (currentCondition == condition.ProcessNotFound)
                return currentCondition;

            else
            {
                return KillProcessById(currentProcessId, filePath);
            }


        }


        private static void GetProcessesDataForFolders(string filePath)
        {

            condition currentCondition = checkCurrentProcess(filePath);
            if (currentCondition == condition.ProcessNotFound)
                return;





            SaveProcessesDataForFolder(currentProcessId, filePath);
        }




        public static condition CloseProcessUsingFolder(string FolderPath,string CurrentLinkType)
        {
            LinkType = CurrentLinkType;

            FolderToDeletePath = FolderPath;

            ClearOldDataSavedFromPreviousProcess();
            RecursiveThroughFolder(FolderPath);

            if (Pids.Count > 1)
            {
                if (ShowFullProcessMessageForFolders() == DialogResult.Yes)
                {
                    return killAllProcesses();

                }
                else
                {
                    ShowUserEachProcessAlone();

                }
            }

            else
            {
                ShowUserEachProcessAlone();

            }




            return AcceptedCloseApps.Any() ? condition.Accepted : condition.Refused;

        }
        public static string GetCurrentActiveAppsInFolder(string FolderPath,string CurrentLinkType)
        {
            LinkType = CurrentLinkType;
            FolderToDeletePath = FolderPath;

            ClearOldDataSavedFromPreviousProcess();
            RecursiveThroughFolder(FolderPath);

            return ConstructFullProcessMessage();

        }
        public static void RecursiveThroughFolder(string FolderPath)
        {



            foreach (var filePath in Directory.GetFiles(FolderPath))
            {
                GetProcessesDataForFolders(filePath);


            }


            // Recursively process subdirectories
            foreach (var subDirectoryPath in Directory.GetDirectories(FolderPath))
            {

                RecursiveThroughFolder(subDirectoryPath);
            }




        }


        // Extracts the process ID from the handle.exe output
        private static string ExtractProcessId(string handleOutput, string filePath)
        {
            string processId = null;

            // Split the output into lines and search for the file path in the output
            string[] lines = handleOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);



            foreach (string line in lines)
            {
                if (line.Contains("pid"))
                {
                    // Look for PID in the line, typically in the format: "pid: XXXX"
                    int pidIndex = line.IndexOf("pid:");
                    if (pidIndex != -1)
                    {
                        // Extract the PID (after "pid:")
                        processId = line.Substring(pidIndex + 5).Split(' ')[0];
                        break;
                    }
                }
            }

            return processId;
        }


        private static string GetApplicationNameForExtension(string fileExtension)
        {
            switch (fileExtension.ToLower()) // Convert to lowercase for case-insensitivity
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return "Image Viewer"; // Generic image viewer
                case ".mp4":
                case ".ts":
                    return "Video player";
                case ".mp3":
                    return "Audio player";

                case ".zip":
                case ".rar":
                    return "7zip"; // Compressed files

                case ".html":
                case ".htm":
                    return "Web Browser"; // Web files


                default:
                    return "Unknown"; // No associated application
            }
        }

        private static string GetOfficeApplicationNames(string ProcessName)
        {
            switch (ProcessName.ToLower()) // Convert to lowercase for case-insensitivity
            {
                case "winword":
                    return "Microsoft Word";
                case "excel":
                    return "Microsoft Excel";
                case "powerpnt":
                    return "Microsoft PowerPoint";
                case "outlook":
                    return "Microsoft Outlook";
                case "msaccess":
                    return "Microsoft Access";
                case "onenote":
                    return "Microsoft OneNote";
                case "visio":
                    return "Microsoft Visio";
                case "mspub":
                    return "Microsoft Publisher";
                case "winproj":
                    return "Microsoft Project";
                case "infopath":
                    return "Microsoft InfoPath";
                case "groove":
                    return "Microsoft Groove";
                case "lync":
                case "skypeforbusiness":
                    return "Microsoft Skype for Business";
                case "frontpg":
                    return "Microsoft FrontPage";
                case "ois":
                    return "Microsoft Office Picture Manager";
                case "offdiag":
                    return "Microsoft Office Diagnostics";
                case "mstore":
                    return "Microsoft Clip Organizer";
                case "msqdoc":
                    return "Microsoft Office Document Imaging";
                case "communicator":
                    return "Microsoft Office Communicator";
                case "msouc":
                    return "Microsoft Office Upload Center";
                case "teams":
                    return "Microsoft Teams";


                default:
                    return "Unknown"; // No associated office application
            }
        }

        private static string GetApplicationName(Process process, string filePath)
        {
            try
            {
                // Get the full path of the executable
                string appPath = process.MainModule.FileName;

                // Return the executable name
                return Path.GetFileNameWithoutExtension(appPath); ;
            }
            catch
            {
                string appName = GetOfficeApplicationNames(process.ProcessName);
                if (appName != "Unknown")
                    return appName;
                else
                {
                    appName = GetApplicationNameForExtension(Path.GetExtension(filePath).ToLowerInvariant());
                    if (appName != "Unknown")
                        return appName;
                    else
                        return "";
                }


            }
        }
        // Kills the process by its PID
        private static condition KillProcessById(string processId, string filePath)
        {


            DialogResult dialogResult;
            string fileName = Path.GetFileNameWithoutExtension(filePath);



            try
            {


                int pid = int.Parse(processId);
                Process process = Process.GetProcessById(pid);


                string App = GetApplicationName(process, filePath);
                string appName = (App == "" ? process.ProcessName : App);


                // Show a message box asking the user if they want to kill the process
                dialogResult = CustomMessageBox.Show($"App: '{appName}' is using the file: '{fileName}'.\nDo you want to close it to continue creating the {LinkType}?",
              "Files in Use", MessageBoxIcon.Warning, false, "Close", "No");


                if (dialogResult == DialogResult.Yes)
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }

                    return condition.Accepted;
                }

                else
                {
                    ActiveApps.Clear();
                    ActiveApps.Add(appName);
                    return condition.Refused;
                }


            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to kill the process: {ex.Message}", "Error", MessageBoxIcon.Error, true, "OK");
                return condition.ProcessKillFailed;
            }
        }

    }

}

