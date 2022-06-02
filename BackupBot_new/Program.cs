using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackupBot_new
{
    class Program
    {
        static void Main(string[] args)
        {
            FileFolderNode root = new FileFolderNode(@"H:\SEW\Bank 20.01");

            //Fehlerbehandlung wenn der Benutzer Source- und Backup-Pfad nicht ordnungsgemäß angegeben hat.
            if (args.Length != 2)
            {
                Console.WriteLine("Bitte GENAU zwei Argumente angeben! Sourcepath und Backuppath");
                Console.WriteLine("Bitte beliebige Taste drücken......");
                Console.Read();
                Environment.Exit(1);
            }

            string SourcePath = args[0];
            string BackupPath = args[1];

            BackupManager myBackupManager = new BackupManager(SourcePath, BackupPath);

            //Überprüfen der übergebenen Pfade
            //Abfrage ob Backup-Ordner erstellt werden soll.
            if (myBackupManager.CheckSourcePath() == false) Environment.Exit(1);
            if (myBackupManager.CheckBackupPath() == false) Environment.Exit(1);
            if (myBackupManager.ScanFilesNow() == true) Console.WriteLine("Scan erfolgreich!");
            if (myBackupManager.BackupNow() == true) Console.WriteLine("Backup erfolgreich!");
            else Console.WriteLine("Backup fehlgeschlagen!");

            Console.ReadLine();
        }
    }
}
