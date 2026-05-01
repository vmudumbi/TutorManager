namespace TutorManager.App.UI
{
    partial class BackupForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.Button btnTakeBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnBrowseRestore; // New Button
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom; // New Panel for buttons

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.btnTakeBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnBrowseRestore = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // pnlTop
            this.pnlTop.Controls.Add(this.lblStatus);
            this.pnlTop.Controls.Add(this.btnTakeBackup);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 60;
            this.pnlTop.BackColor = System.Drawing.Color.WhiteSmoke;

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(20, 20);
            this.lblStatus.Text = "Recent Backups (Auto-Saved Daily)";
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.AutoSize = true;

            // btnTakeBackup
            this.btnTakeBackup.Anchor = AnchorStyles.Right;
            this.btnTakeBackup.Location = new System.Drawing.Point(620, 15);
            this.btnTakeBackup.Size = new System.Drawing.Size(150, 30);
            this.btnTakeBackup.Text = "Backup Now";
            this.btnTakeBackup.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnTakeBackup.ForeColor = System.Drawing.Color.White;
            this.btnTakeBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // dgvHistory
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.BorderStyle = BorderStyle.None;

            // pnlBottom (Container for Restore Buttons)
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Height = 60;
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // btnRestore (Restore from Grid)
            this.btnRestore.Location = new System.Drawing.Point(405, 10);
            this.btnRestore.Size = new System.Drawing.Size(370, 40);
            this.btnRestore.Text = "Restore Selected from History";
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.btnRestore.ForeColor = System.Drawing.Color.White;
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // btnBrowseRestore (Manual File Selection)
            this.btnBrowseRestore.Location = new System.Drawing.Point(25, 10);
            this.btnBrowseRestore.Size = new System.Drawing.Size(370, 40);
            this.btnBrowseRestore.Text = "Restore from External File...";
            this.btnBrowseRestore.BackColor = System.Drawing.Color.DimGray;
            this.btnBrowseRestore.ForeColor = System.Drawing.Color.White;
            this.btnBrowseRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            // BackupForm
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.dgvHistory);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Text = "Database Management - Admin Only";
            this.pnlBottom.Controls.Add(this.btnRestore);
            this.pnlBottom.Controls.Add(this.btnBrowseRestore);

            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}