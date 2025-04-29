using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace WindowsFormsApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            GetStartupPrograms();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GetStartupPrograms();
        }

        private void GetStartupPrograms()
        {
            HashSet<string> startupPrograms = new HashSet<string>();

            // Kayıt Defterinden başlangıç programlarını al
            GetStartupProgramsFromRegistry(startupPrograms, Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            GetStartupProgramsFromRegistry(startupPrograms, Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

            // Başlangıç klasöründen başlangıç programlarını al
            GetStartupProgramsFromFolder(startupPrograms, Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            GetStartupProgramsFromFolder(startupPrograms, Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));



            // ListBox'a başlangıç programlarını ekle
            listBox1.Items.Clear();
            foreach (string program in startupPrograms)
            {
                listBox1.Items.Add(program);
            }
        }

        private void GetStartupProgramsFromRegistry(HashSet<string> startupPrograms, RegistryKey rootKey, string subKey)
        {
            try
            {
                using (RegistryKey key = rootKey.OpenSubKey(subKey))
                {
                    if (key != null)
                    {
                        foreach (string appName in key.GetValueNames())
                        {
                            string command = key.GetValue(appName).ToString();
                            startupPrograms.Add(appName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Başlangıç programları alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetStartupProgramsFromFolder(HashSet<string> startupPrograms, string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    // Sistemle ilgili olanları filtrele
                    if (!IsSystemProgram(fileName, filePath, string.Empty))
                    {
                        startupPrograms.Add(fileName);
                    }
                }
            }
        }

        private void GetTasksFromFolder(TaskFolder folder, HashSet<string> startupPrograms, string description)
        {
            foreach (Task task in folder.Tasks)
            {
                if (task.Definition.Triggers.Any(trigger => trigger.TriggerType == TaskTriggerType.Boot ||
                                                            trigger.TriggerType == TaskTriggerType.Logon))
                {
                    description = task.Definition.RegistrationInfo != null ? task.Definition.RegistrationInfo.Description : null;
                    // Sistemle ilgili olanları filtrele
                    if (!IsSystemProgram(task.Name, task.Definition.Actions.ToString(), description))
                    {
                        startupPrograms.Add(task.Name);
                    }
                }
            }

            foreach (TaskFolder subFolder in folder.SubFolders)
            {
                GetTasksFromFolder(subFolder, startupPrograms, description);
            }
        }

        private bool IsSystemProgram(string programName, string command, string description)
        {
            // Sistemle ilgili programları filtrelemek için kurallar
            string[] systemPrograms = new string[]
            {
                "SecurityHealthSystray",
                "OneDrive",
                "Windows Security",
                "MicrosoftEdgeAutoLaunch",
                "Program",
                "ShellExperienceHost",
                "StartMenuExperienceHost",
                "OneDriveSetup",
                "WindowsDefender",
                "Adobe",
                "GoogleUpdateTaskMachineUA",
                "GoogleUpdateTaskMachineCore",
                "Opera Browser Assistant",
                "Skype",
                "Windows Mail",
                "YourPhone"
            };

            // Sistem programlarının adını, komutlarını ve açıklamalarını kontrol et
            foreach (var systemProgram in systemPrograms)
            {
                if (programName.IndexOf(systemProgram, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    command.IndexOf(systemProgram, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (description != null && description.IndexOf(systemProgram, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }

            return false;
        }

        





    }
}
