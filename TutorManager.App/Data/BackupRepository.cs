using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using TutorManager.App.Models;

namespace TutorManager.App.Data
{
    public class BackupRepository
    {
        private string backupFolder = ConfigurationManager.AppSettings["BackupPath"] ?? "Backups";
        private int maxFiles = 10;

        public BackupRepository()
        {
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
        }

        public void PerformAutoBackup(string sourceDbPath)
        {
            try
            {
                // Simply check if the source database exists
                if (!File.Exists(sourceDbPath)) return;

                // We removed the 'DateTime.Today' check.
                // Now it runs every time the method is called.
                CreateBackup(sourceDbPath);
            }
            catch (Exception ex)
            {
                // Log error to debug console if needed
                System.Diagnostics.Debug.WriteLine("Auto-backup failed: " + ex.Message);
            }
        }

        public string CreateBackup(string sourceDbPath)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string fileName = $"tutor_{timestamp}.db";
            string destPath = Path.Combine(backupFolder, fileName);

            File.Copy(sourceDbPath, destPath, true);
            DeleteOldestBackups();
            return destPath;
        }

        // FIX FOR ERROR: GetBackupHistory
        public List<BackupFile> GetBackupHistory()
        {
            var list = new List<BackupFile>();
            var files = new DirectoryInfo(backupFolder).GetFiles("*.db")
                            .OrderByDescending(f => f.CreationTime);

            foreach (var file in files)
            {
                list.Add(new BackupFile
                {
                    FileName = file.Name,
                    FullPath = file.FullName,
                    CreatedAt = file.CreationTime,
                    SizeFormatted = (file.Length / 1024.0).ToString("F2") + " KB"
                });
            }
            return list;
        }
        
        public void RestoreBackup(string backupFilePath, string currentDbPath)
        {
            if (!File.Exists(backupFilePath)) return;

            // Optional: Create a safety backup of the current DB before overwriting
            string safetyPath = currentDbPath + ".temp";
            File.Copy(currentDbPath, safetyPath, true);

            File.Copy(backupFilePath, currentDbPath, true);
        }

        private void DeleteOldestBackups()
        {
            var files = new DirectoryInfo(backupFolder).GetFiles("*.db")
                            .OrderByDescending(f => f.CreationTime).ToList();

            while (files.Count > maxFiles)
            {
                files.Last().Delete();
                files.RemoveAt(files.Count - 1);
            }
        }
    }
}