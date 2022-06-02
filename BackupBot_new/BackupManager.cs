using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace BackupBot_new
{
    class BackupManager
    {
        string sourcePath;
        string backupPath;
        FileFolderNode RootNode;

        public BackupManager(string sourcePath, string backupPath)
        {
            this.sourcePath = sourcePath;
            this.backupPath = backupPath;
        }

        public bool CheckSourcePath()
        {
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("Quellpfad existiert nicht!");
                Console.WriteLine("Bitte beliebige Taste drücken......");
                Console.Read();
                return false;
            }
            return true;
        }
        public bool CheckBackupPath()
        {
            if (!Directory.Exists(backupPath))
            {
                Console.WriteLine("Backuppfad existiert nicht!");
                Console.WriteLine("Zum Erstellen 'e' drücken! Andere Tastet beendet die Applikation!");
                if (Console.ReadKey().KeyChar == 'e')
                {
                    Directory.CreateDirectory(backupPath);
                    Console.WriteLine();
                    Console.WriteLine("Backupverzeichnis '{0}' wurde erstellt!", backupPath);
                    return true;
                }
                else return false;
            }
            return true;
        }
        string GenerateTimestampBackupFolder(DateTime timestamp)
        {
            /*string Foldername = "backup_" + timestamp.Year.ToString("0000")     + "_"
                                          + timestamp.Month.ToString("00")      + "_"
                                          + timestamp.Day.ToString("00")        + "__"
                                          + timestamp.Hour.ToString("00")       + "_"
                                          + timestamp.Minute.ToString("00")     + "_"
                                          + timestamp.Second.ToString("00");*/

            string Foldername = "backup_" + timestamp.ToString("yyyy_MM_dd__HH_mm_ss");

            string fullbackupPath = Path.Combine(backupPath, Foldername);

            Directory.CreateDirectory(fullbackupPath);

            return fullbackupPath;
        }

        void DeepCopyFolder(string localsourcePath, string localDestinationPath)
        {
            DirectoryInfo source = new DirectoryInfo(localsourcePath);

            foreach (FileInfo f in source.GetFiles())
            {
                //f.LastWriteTimeUtc
                File.Copy(f.FullName, Path.Combine(localDestinationPath, f.Name));
            }

            foreach (DirectoryInfo d in source.GetDirectories())
            {
                Directory.CreateDirectory(Path.Combine(localDestinationPath, d.Name));
                DeepCopyFolder(d.FullName, Path.Combine(localDestinationPath, d.Name));
            }
        }

        public void ScanChangedFiles()
        {
            RootNode = new FileFolderNode(sourcePath);
        }
        public bool ScanFilesNow()
        {
            ScanChangedFiles();

            return true;
        }

        public bool BackupNow()
        {
            string actualBackupFolder = GenerateTimestampBackupFolder(DateTime.Now);

            DeepCopyFolder(sourcePath, actualBackupFolder);
            ScanChangedFiles();

            RootNode.SaveToFile(@"H:\Syt\Backup 3.0\Zielordner\ABC.xml");

            FileFolderNode parsedTree = new FileFolderNode(XmlReader.Create(@"H:\Syt\Backup 3.0\Zielordner\ABC.xml"));
            Console.Write("");
            return true;
        }
    }
}
