using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using System.Runtime.InteropServices;

using System.Linq;


namespace SymbolicLinkCreator
{
    

  

    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            btnBrowseSource.BackColor = Color.Transparent;
            btnBrowseSource.FlatStyle = FlatStyle.Flat;
            btnBrowseSource.FlatAppearance.BorderSize = 0;


            btnBrowseDestination.BackColor = Color.Transparent;
            btnBrowseDestination.FlatStyle = FlatStyle.Flat;
            btnBrowseDestination.FlatAppearance.BorderSize = 0;

            CheckDir_symbolicFolderPath();
            this.WindowState = FormWindowState.Normal;




            LoadScheduledPaths();



            btnBrowseSource.BackColor = Color.Transparent;
            btnBrowseSource.FlatStyle = FlatStyle.Flat;
            btnBrowseSource.FlatAppearance.BorderSize = 0;

            btnBrowseDestination.BackColor = Color.Transparent;
            // Remove the button's border
            btnBrowseDestination.FlatStyle = FlatStyle.Flat;
            btnBrowseDestination.FlatAppearance.BorderSize = 0;


           

        }


        private static string symbolicFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".symbolic link wizard");

        private  string logFilePath = Path.Combine(symbolicFolderPath, "symboliclinkwizard.log");
        private  string ScheduledPaths = Path.Combine(symbolicFolderPath, "symboliclinkwizard_scheduled_paths.txt");

        private string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        
        private bool ScheduledRecovery = false;
        private bool sheduledforReboot = false;
        private static int numberOfSourcePaths=0,numberOfDestinationPaths=0,numberOfRecoveredPaths=0, numberOfSuccessMessages = 0;
        private string CreationMessage = "";


        private static List<bool> sheduledforRebootList = new List<bool>();
      


        // Create lists to gather all source and destination paths
        private static List<string> scheduled_sourcePaths = new List<string>();
        private static List<string> scheduled_destinationPaths = new List<string>();

        private static List <bool> LinkType = new List<bool>();

        private static void CheckDir_symbolicFolderPath()
        {
            // Ensure the directory exists
            if (!Directory.Exists(symbolicFolderPath))
            {
                Directory.CreateDirectory(symbolicFolderPath);
            }

        }


        private void CreateSymbolicForOnReboot()
        {

            

            string[] sources = ExtractPathsFromQuotes(string.Join("", scheduled_sourcePaths));
            

            string[] destinations = ExtractPathsFromQuotes(string.Join("", scheduled_destinationPaths));



            string sourceName;
            string fullDestination;
            List<string> remainingLines = new List<string>();

            if (ScheduledRecovery)
            {

                for (int i = 0; i < numberOfRecoveredPaths; i++)
                {

                    sourceName = Path.GetFileName(sources[i]);
                    fullDestination = Path.Combine(destinations[i], sourceName);
                    chk_Directory_SymbolicLink.Checked = LinkType[i];

                    bool CreatedSuccessfully = CreateSymbolicLink(sources[i], fullDestination);

                    if (!CreatedSuccessfully)
                    {
                        remainingLines.Add(LinkType[i].ToString());
                        remainingLines.Add($"\"{sources[i]}\"");
                        remainingLines.Add($"\"{destinations[i]}\"");

                    }

                    if (CreatedSuccessfully)
                        ConstructCreationMessage(sources[i], fullDestination);

                    if (i == numberOfRecoveredPaths - 1&&numberOfSuccessMessages>0)
                        ShowSymbolicLinkCreationMessage();
                }



            }

            chk_Directory_SymbolicLink.Checked = false;

            if (!remainingLines.Any())
                File.Delete(ScheduledPaths);
            else
                File.WriteAllLines(ScheduledPaths, remainingLines);

           

        }

        private void WritePathsToTextFile(string sourcePath, string destinationPath)
        {
            try
            {
                // Check if the file exists
                bool fileExists = File.Exists(ScheduledPaths);

                using (StreamWriter sw = new StreamWriter(ScheduledPaths, true))
                {
                    

                    sw.WriteLine(chk_Directory_SymbolicLink.Checked.ToString());
                    // Write source and destination paths in double quotes
                    sw.WriteLine($"\"{sourcePath}\"");
                    sw.WriteLine($"\"{destinationPath}\"");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error Sceduling Paths : {ex.Message}","Error", MessageBoxIcon.Error,true,"OK");
            }
        }


       private bool GetLinkTypeFromString(string s)
        {
            return s != "False";
        }


        private void LoadScheduledPaths()
        {

            try
            {
                // Check if the file exists
                if (File.Exists(ScheduledPaths))
                {
                    // Read all lines from the text file
                    var lines = File.ReadAllLines(ScheduledPaths);

                    int Counter = lines.Length - 1;


                    // Starting from the second line for source and destination pairs
                    for (int i = 0; i < lines.Length; i += 3)
                    {
                        if(string.IsNullOrEmpty(lines[i])||string.IsNullOrWhiteSpace(lines[i]))
                        {
                            Counter--;
                            continue;
                        }

                        if (i < lines.Length)
                        {
                            LinkType.Add(GetLinkTypeFromString(lines[i]));

                        }
                        
                        if (i+1 < lines.Length)
                        {
                            scheduled_sourcePaths.Add(lines[i+1]); // Add source path
                        }
                        if (i + 2 < lines.Length)
                        {
                            scheduled_destinationPaths.Add(lines[i + 2]); // Add destination path
                        }
                    }

                  

                    // Flagging as recovery so we won't copy the file
                    ScheduledRecovery = true;

                    // Disable auto startup
                    DisableStartup();

                    numberOfRecoveredPaths = (Counter) / 2;

                    
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error loading Sceduled Paths: {ex.Message}", "Error", MessageBoxIcon.Error, true, "OK");
            }
        }

        public static string[] ExtractPathsFromQuotes(string input)
        {
            // Define the regex pattern to match all text inside double quotes
            string pattern = "\"([^\"]*)\"";

            // Use Regex.Matches to find all matches
            MatchCollection matches = Regex.Matches(input, pattern);

            // Create an array to store all the matched strings
            string[] result = new string[matches.Count];

            // Iterate over all matches and add them to the result array
            for (int i = 0; i < matches.Count; i++)
            {
                result[i] = matches[i].Groups[1].Value; // Extract the string inside quotes
            }

            // Return the array of matched strings
            return result;
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {

            txtSource.Text = string.Empty;

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            IntPtr ownerHandle = this.Handle;




            dialog.Multiselect = true;


            bool? DialogResult = dialog.ShowDialog(ownerHandle);

            if(DialogResult==true)
            {
                
                foreach (string SelectedPath in dialog.SelectedPaths)
                {
                    txtSource.Text += "\""+SelectedPath+"\"";
                   
                }

            }

            

        }
        

        private void btnBrowseDestination_Click(object sender, EventArgs e)
        {
            txtDestination.Text = string.Empty;

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            IntPtr ownerHandle = this.Handle;


            

            dialog.Multiselect = true;


            bool? DialogResult = dialog.ShowDialog(ownerHandle);

            if (DialogResult == true)
            {
                
                foreach (string SelectedPath in dialog.SelectedPaths)
                {
                    txtDestination.Text += "\"" + SelectedPath + "\"";
                    
                }

            }


        }


        private void ShowSymbolicLinkCreationMessage()
        {
           
            CustomMessageBox.Show($"{CreationMessage}", "Success", MessageBoxIcon.Information,true,"OK");
            
        }

        private void ConstructCreationMessage(string source, string fullDestination)
        {

            if (chk_Directory_SymbolicLink.Checked)
                CreationMessage += $"Symbolic link created : {source} <==> {fullDestination}\n";
            else
                CreationMessage += $"Junction Created : {source} <==> {fullDestination}\n";

            numberOfSuccessMessages++;

        }

        private void btnCreateSymlink_Click(object sender, EventArgs e)
        {
            

            string [] sources = ExtractPathsFromQuotes(txtSource.Text);
            string[] destinations = ExtractPathsFromQuotes(txtDestination.Text);

            numberOfSourcePaths = sources.Length;
            numberOfDestinationPaths = destinations.Length;

            string sourceName;
            string fullDestination;

            if (numberOfSourcePaths == 0 || numberOfDestinationPaths == 0)
            {
                CustomMessageBox.Show("Both source and destination must be specified.", "Error", MessageBoxIcon.Error,true,"OK");
                return;
            }
            else if (numberOfSourcePaths != numberOfDestinationPaths && numberOfDestinationPaths != 1)
            {
                CustomMessageBox.Show("Both source and destination must be of same number of paths or\n you could choose one destination for them all.", "Error", MessageBoxIcon.Error,true,"OK");
                return;
            }
            
            
            try
            {

                if(numberOfDestinationPaths==1)
                {
                    for(int i=0;i<numberOfSourcePaths;i++)
                    {
                        sourceName= Path.GetFileName(sources[i]);
                        fullDestination = Path.Combine(destinations[0], sourceName);

                        if(sources[i]== destinations[0])
                        {
                            MessageBox.Show($"Trying to Generate Link For The Same Folder With itself \n {sources[i]} cannot link with itself\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (!ScheduledRecovery)
                        {
                            CopyDirectory(sources[i], fullDestination);

                            // Step 2: Delete the original source directory
                            DeleteFolder(sources[i], destinations[0]);

                        }


                        // Step 3: Create the symbolic link at the original source location pointing to the new destination

                        if (!sheduledforReboot)
                        {
                            CreateSymbolicLink(sources[i], fullDestination);

                            ConstructCreationMessage(sources[i], fullDestination);
                            

                        }

                        if (i == numberOfSourcePaths - 1)
                        {
                            if(numberOfSuccessMessages>0)
                                ShowSymbolicLinkCreationMessage();

                            ScheduleReboot();
                        }
                            

                    }
                    

                }
                else
                {
                    for (int i = 0; i < numberOfSourcePaths; i++)
                    {
                        sourceName = Path.GetFileName(sources[i]);
                        fullDestination = Path.Combine(destinations[i], sourceName);

                        if (sources[i] == destinations[i])
                        {
                            MessageBox.Show($"Trying to Generate Link For The Same Folder With itself \n {sources[i]} cannot link with itself\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        if (!ScheduledRecovery)
                        {
                            CopyDirectory(sources[i], fullDestination);

                            // Step 2: Delete the original source directory
                            
                              DeleteFolder(sources[i], destinations[i]);
                            



                        }

                        // Step 3: Create the symbolic link at the original source location pointing to the new destination

                        if (!sheduledforReboot)
                        {
                            CreateSymbolicLink(sources[i], fullDestination);

                            ConstructCreationMessage(sources[i], fullDestination);

                        }

                        if (i == numberOfSourcePaths - 1)
                        {
                            if (numberOfSuccessMessages > 0)
                                ShowSymbolicLinkCreationMessage();

                            ScheduleReboot();
                        }
                            
                    }
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void AddtoRebootList(bool newBool)
        {
            // Add the new boolean to the list
            sheduledforRebootList.Add(newBool);
        }

        public static bool IsRebootRequired()
        {
            
            return sheduledforRebootList.Contains(true);  
        }

        public void HandleAccessDeniedError(string directoryPath,string DestinationPath)
        {
            sheduledforReboot = true;
            AddtoRebootList(true);

            try
            {
                

                // Rename all files in the directory
                RenameFilesInDirectory(directoryPath);
                // Rename all subdirectories in the directory
                RenameDirectoriesInDirectory(directoryPath);


               
            }

            catch (Exception)
            {

               //Renaming Failed

            }

            finally
            {

                LogAction($"Scheduled  files and folders in '{directoryPath}' for deletion upon reboot due to access denial.");

                WritePathsToTextFile(directoryPath, DestinationPath);

                EnableStartup();

                CleanDirectoryRecursively(directoryPath);

            }
        }


        [Flags]
        internal enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool MoveFileEx(
                string lpExistingFileName,
                string lpNewFileName,
                MoveFileFlags dwFlags);
        }


        public void EnableStartup()
        {
            try
            {
                

                // Name of the batch file and the VBS file
                string batchFileName = "Symlinkwizardstartup.bat";
                string vbsFileName = "Symlinkwizardstartup.vbs";

                // Path to save the batch file in Roaming folder
                string batchFilePath = Path.Combine(symbolicFolderPath, batchFileName);

                // Path to save the VBS file in the Startup folder
                string vbsFilePath = Path.Combine(startupFolderPath, vbsFileName);

                // Prepare the content of the batch file
                string batchContent = $"@echo off\n" +
                                      $"\"{Application.ExecutablePath}\"\n" +
                                      "exit";

                // Write the batch file content in the Roaming folder
                File.WriteAllText(batchFilePath, batchContent);

                // Prepare the content of the VBS file that will run the batch file hidden
                string vbsContent = $"Set WshShell = CreateObject(\"WScript.Shell\")\n" +
                                    $"WshShell.Run chr(34) & \"{batchFilePath}\" & chr(34), 0, False";

                // Write the VBS file content to the Startup folder (0 means hidden, False means don't wait)
                File.WriteAllText(vbsFilePath, vbsContent);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error enabling startup: {ex.Message}", "Error", MessageBoxIcon.Error,true,"OK");
            }
        }

        public void DisableStartup()
        {
            try
            {


                string batchFileName = "Symlinkwizardstartup.bat";

                // Name of the VBS file
                string vbsFileName = "Symlinkwizardstartup.vbs";

                // Full path to the VBS file in the Startup folder
                string vbsFilePath = Path.Combine(startupFolderPath, vbsFileName);

                string batchFilePath = Path.Combine(symbolicFolderPath, batchFileName);


                // Check if the VBS file exists
                if (File.Exists(vbsFilePath))
                {
                    // Delete the VBS file
                    File.Delete(vbsFilePath);
                    

                }
               

                if (File.Exists(batchFilePath))
                {
                    // Delete the VBS file
                    File.Delete(batchFilePath);


                }
               



            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error While Disabling Start Up : {ex.Message}", "Error", MessageBoxIcon.Error, true, "OK");

            }
        }


        public void ScheduleFolderDeletion(string folderPath)
        {
            
            folderPath = folderPath.Replace("\\", "\\\\");



            // Schedule deletion using MoveFileEx without quoting the path
            if (!NativeMethods.MoveFileEx(folderPath, null, MoveFileFlags.DelayUntilReboot))
            {
                int errorCode = Marshal.GetLastWin32Error();
                CustomMessageBox.Show($"Failed to schedule deletion: {errorCode} - {new System.ComponentModel.Win32Exception(errorCode).Message}", "Error", MessageBoxIcon.Error, true, "OK");
            }
           
        }



        private void CleanDirectoryRecursively(string directoryPath)
        {
            // Process all files in the current directory
            foreach (var filePath in Directory.GetFiles(directoryPath))
            {

                ScheduleFolderDeletion(filePath);
            }

            // Recursively process subdirectories
            foreach (var subDirectoryPath in Directory.GetDirectories(directoryPath))
            {
                CleanDirectoryRecursively(subDirectoryPath);
            }

            // Schedule the current directory for deletion on reboot
            ScheduleFolderDeletion(directoryPath);
        }


        private void TakeOwnership(string path)
        {
            

            try
            {
                ProcessStartInfo takeOwnInfo = new ProcessStartInfo("cmd.exe", $"/c takeown /f \"{path}\" /r /d y")
                {
                    Verb = "runas",  // Run as administrator
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process takeOwnProcess = Process.Start(takeOwnInfo);
                takeOwnProcess?.WaitForExit();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error Couldn't Elevate Your privileges: {ex.Message}", "Permissions Required", MessageBoxIcon.Error, true, "OK");
            }
        }

        private void GrantFullControlAndRemoveSystem(string path)
        {
            try
            {
                // Grant full control to the current user
                ProcessStartInfo grantInfo = new ProcessStartInfo("cmd.exe", $"/c icacls \"{path}\" /grant \"%USERNAME%:F\" /t")
                {
                    Verb = "runas",  // Run as administrator
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process grantProcess = Process.Start(grantInfo);
                grantProcess?.WaitForExit();

                // Remove all other permissions, including SYSTEM and TrustedInstaller
                ProcessStartInfo removeInfo = new ProcessStartInfo("cmd.exe", $"/c icacls \"{path}\" /remove:g SYSTEM TrustedInstaller /t")
                {
                    Verb = "runas",  // Run as administrator
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process removeProcess = Process.Start(removeInfo);
                removeProcess?.WaitForExit();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Error Couldn't Elevate Your privileges: {ex.Message}", "Permissions Required", MessageBoxIcon.Error, true, "OK");
            }
        }


    
       
       

      
        private void DeleteFolder(string directoryPath,string DestinationPath)
        {
            short counter = 0;
            string LinkType = chk_Directory_SymbolicLink.Checked ? "symbolic link" : "junction";


        aa:
            try
            {

                sheduledforReboot = false;
                counter++;

                try
                {
                    Directory.Delete(directoryPath, true);

                }

                catch (Exception)
                {

                    TakeOwnership(directoryPath);
                    GrantFullControlAndRemoveSystem(directoryPath);

                    // Attempt to delete the folder and its contents
                    Directory.Delete(directoryPath, true);
                }
               

               
                
            }
            catch (UnauthorizedAccessException)
            {
                // Call HandleAccessDeniedError if access is denied
                HandleAccessDeniedError(directoryPath, DestinationPath);
            }
            catch (IOException ioEx) when (ioEx.HResult == -2147024864) // HResult for "The process cannot access the file because it is being used by another process"
            {

                DialogResult result;



                ProcessKiller.condition conditon = ProcessKiller.CloseProcessUsingFolder(directoryPath,LinkType);

                if (conditon == ProcessKiller.condition.Accepted)
                {

                    goto aa;
                }

                else
                {


                    string message = "";

                    message = ProcessKiller.GetCurrentActiveAppsInFolder(directoryPath,LinkType);

                    if (message != "")
                    {
                        result = CustomMessageBox.Show($"Please close these apps: {message}\nand then click 'Done' to continue with creating the {LinkType}.\nIf you're unable to close them, a reboot will be required.",
                                                    "Folder is In Use",
                                                    MessageBoxIcon.Warning, false, "Done", "Couldn't");

                        if (result == DialogResult.Yes)
                        {
                            goto aa;
                        }
                        else
                        {
                            HandleAccessDeniedError(directoryPath, DestinationPath);
                        }


                    }

                    else
                    {
                        //No Active Apps
                        if (ProcessKiller.ActiveApps.Count == 0)
                            goto aa;

                        result = CustomMessageBox.Show($"Please close the folder or any files/subfolders inside it that are currently in use so we can proceed with creating the {LinkType}.\nOnce closed, click 'Done' to continue.\nIf you're unable to close them, a reboot will be required.",
                                                "Folder is In Use",
                                                MessageBoxIcon.Warning, false, "Done", "Couldn't");




                        if (result == DialogResult.Yes)
                        {
                            goto aa;
                        }
                        else
                        {
                            HandleAccessDeniedError(directoryPath, DestinationPath);
                        }
                    }





                }



            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"An error occurred while deleting the folder: {ex.Message}", "Error", MessageBoxIcon.Error, true, "OK");
            }
        }

        // Method to rename all files in a directory
        private void RenameFilesInDirectory(string directoryPath)
        {
            string hiddenChar = "\u200B";

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                string newFilePath = Path.Combine(directoryPath, $"{Path.GetFileNameWithoutExtension(filePath)}{hiddenChar}{Path.GetExtension(filePath)}");
                File.Move(filePath, newFilePath); // Rename the file
            }
        }

        // Method to rename all directories in a directory
        private void RenameDirectoriesInDirectory(string directoryPath)
        {
            string hiddenChar = "\u200B";
            foreach (var dirPath in Directory.GetDirectories(directoryPath))
            {
                string newDirPath = Path.Combine(Path.GetDirectoryName(dirPath), $"{Path.GetFileName(dirPath)}{hiddenChar}");
                Directory.Move(dirPath, newDirPath); // Rename the directory
            }
        }

        // Method to log actions to a text file
        private void LogAction(string message)
        {
            try
            {
                // Ensure the log file exists
                if (!File.Exists(logFilePath))
                {
                    File.WriteAllText(logFilePath, ""); // Create the file if it doesn't exist
                }

                // Append the message to the log file
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Couldn't Save Data to  log: {ex.Message}", "Error", MessageBoxIcon.Error, true, "OK");
            }
        }

        // Method to schedule a reboot
        private void ScheduleReboot()
        {
            if(sheduledforRebootList.Any() && IsRebootRequired())
            {
                if (CustomMessageBox.Show("A reboot is required. Would you like to reboot now?", "Reboot Required", MessageBoxIcon.Information, false, "Reboot Now", "Reboot Later", false) == DialogResult.Yes)
                {

                    Process.Start("shutdown", "/r /t 0"); // Reboot the system immediately
                }
            }


            
        }

        private bool CreateSymbolicLink(string source, string destination)
        {
            // Create the symbolic link using the command line
            string command;

            try
            {
                if (chk_Directory_SymbolicLink.Checked)
                    command = $"/C mklink /D \"{source}\" \"{destination}\"";
                else
                    command = $"/C mklink /J \"{source}\" \"{destination}\"";

                var process = new Process();
               
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = command;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();




                // Wait for the process to finish
                process.WaitForExit();

                if (process.ExitCode != 0)
                {

                    return false;
                }

                
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create destination directory if it doesn't exist
            Directory.CreateDirectory(destinationDir);

            // Copy files
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Copy subdirectories
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDir);
            }
        }



        private void chk_Directory_SymbolicLink_CheckedChanged(object sender, EventArgs e)
        {
            if(chk_Directory_SymbolicLink.Checked)
            {
                btnCreateSymlink.Text = "Create Symbolic Link";
                
            }
            else
            {
                btnCreateSymlink.Text = "Create Junction ";
                

            }
        }

       

        private void MainForm_Shown(object sender, EventArgs e)
        {
            CreateSymbolicForOnReboot();
        }
    }


   
}
