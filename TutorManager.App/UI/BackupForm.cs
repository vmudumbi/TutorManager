using System;
using System.IO;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;

namespace TutorManager.App.UI
{
    public partial class BackupForm : Form
    {
        private BackupRepository _repo = new BackupRepository();
        private string _dbPath = Path.Combine(AppContext.BaseDirectory, "tutor.db");

        public BackupForm()
        {
            InitializeComponent();
            this.Load += (s, e) => LoadHistory();

            btnTakeBackup.Click += (s, e) => {
                _repo.CreateBackup(_dbPath);
                LoadHistory();
                MessageBox.Show("Manual backup created.");
            };

            btnRestore.Click += BtnRestore_Click;
            btnBrowseRestore.Click += BtnBrowseRestore_Click;
        }

        private void LoadHistory()
        {
            dgvHistory.DataSource = null;
            dgvHistory.DataSource = _repo.GetBackupHistory();
            if (dgvHistory.Columns["FullPath"] != null) dgvHistory.Columns["FullPath"].Visible = false;
        }

        // Strategy A: Restore from the selected row in Grid
        private void BtnRestore_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count == 0) return;
            var selected = (BackupFile)dgvHistory.SelectedRows[0].DataBoundItem;
            ExecuteRestore(selected.FullPath, selected.FileName);
        }

        // Strategy B: Restore from a file manually picked from a Folder
        private void BtnBrowseRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQLite Database (*.db)|*.db";
                ofd.Title = "Select a Backup File to Restore";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ExecuteRestore(ofd.FileName, Path.GetFileName(ofd.FileName));
                }
            }
        }

        private void ExecuteRestore(string filePath, string name)
        {
            var result = MessageBox.Show(
                $"RESTORE WARNING:\n\nYou are about to overwrite your current data with: {name}\n\n" +
                "The application will restart immediately. Proceed?",
                "Critical Recovery", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 1. IMPORTANT: Kill any active SQLite connections
                    Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

                    // 2. Perform file swap
                    _repo.RestoreBackup(filePath, _dbPath);

                    // 3. Restart
                    Application.Restart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Restore Failed: " + ex.Message);
                }
            }
        }
    }
}