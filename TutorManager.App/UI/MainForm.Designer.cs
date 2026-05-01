using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.UI;
using TutorManager.App.Utilities;

namespace TutorManager.App.UI
{
    public partial class MainForm : Form
    {
        // Controls
        Panel pnlSidebar, pnlMain, pnlLogo, pnlHeader;
        Button btnStudents, btnAttendance, btnReports,btnBackUp, btnLogout;
        Label lblTitleLogo, lblManager, lblTitle;
        PictureBox picLogo, picBackground;
        Panel pnlOverlay;
        System.Windows.Forms.Timer activityTimer;

        // Styling
        Color sidebarColor = Color.FromArgb(24, 28, 36);
        Color hoverColor = Color.FromArgb(45, 50, 65);
        Color accentTeal = Color.FromArgb(0, 180, 160);
        private Button currentBtn = null;
        private bool isLoggingOut = false;

        private void InitializeComponent(bool isAdmin)
        {
           
            string resourceFolder = Path.Combine(AppContext.BaseDirectory, "Resources");       

            // ================= 1. SIDEBAR =================
            pnlSidebar = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 180, // Slightly wider for better spacing
                BackColor = sidebarColor
            };

            // Sidebar Logo Area
            pnlLogo = new Panel() { Height = 100, Dock = DockStyle.Top, BackColor = Color.FromArgb(20, 24, 32) };

            picLogo = new PictureBox()
            {
                Size = new Size(32, 32),
                Location = new Point(15, 32),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Path.Combine(resourceFolder, "study.png")),
                BackColor = Color.Transparent
            };

            lblTitleLogo = new Label() { Text = "TUTOR", ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(52, 28), AutoSize = true };
            lblManager = new Label() { Text = "MANAGER", ForeColor = accentTeal, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(52, 46), AutoSize = true };

            pnlLogo.Controls.Add(picLogo);
            pnlLogo.Controls.Add(lblTitleLogo);
            pnlLogo.Controls.Add(lblManager);

            // Sidebar Buttons
            btnStudents = CreateBtn("Students", 120);
            btnAttendance = CreateBtn("Attendance", 175);
            btnReports = CreateBtn("Reports", 230);           

            btnStudents.Click += (s, e) => { ActivateButton(btnStudents); lblTitle.Text = "Student Management"; OpenForm(new StudentForm()); };
            btnAttendance.Click += (s, e) => { ActivateButton(btnAttendance); lblTitle.Text = "Attendance Tracking"; OpenForm(new LogHoursForm()); };
            btnReports.Click += (s, e) => { ActivateButton(btnReports); lblTitle.Text = "Performance Reports"; OpenForm(new ReportForm()); };

            btnLogout = new Button()
            {
                Text = "   Logout",
                Dock = DockStyle.Bottom,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                BackColor = sidebarColor,
                ForeColor = Color.FromArgb(255, 120, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 45, 60);
            btnLogout.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 35, 50);

            btnLogout.Click += async (s, e) =>
            {
                var result = MessageBox.Show(
                    "Are you sure you want to log out?",
                    "Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    isLoggingOut = true;
                    btnLogout.Enabled = false;
                    await PerformAutoBackupAsync();
                    LoginForm login = new LoginForm();
                    this.Hide();
                    login.ShowDialog();
                    this.Close();
                }
            };

            var img = Image.FromFile(Path.Combine(resourceFolder, "logout.png"));
            btnLogout.Image = new Bitmap(img, new Size(18, 18));

            btnLogout.Cursor = Cursors.Hand;

            pnlSidebar.Controls.Add(btnReports);
            pnlSidebar.Controls.Add(btnAttendance);
            pnlSidebar.Controls.Add(btnStudents);
            pnlSidebar.Controls.Add(pnlLogo);
            pnlSidebar.Controls.Add(btnLogout);
            btnLogout.BringToFront();

            // ================= 2. MAIN CONTAINER =================
            pnlMain = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            // ================= 3. TOP HEADER (New Design) =================
            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Visible = false // Only show when a module is open
            };

            lblTitle = new Label()
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(48, 54, 65),
                Location = new Point(25, 18),
                AutoSize = true
            };

            Panel headerBorder = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 2,
                BackColor = Color.FromArgb(230, 230, 230) // Subtle line
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(headerBorder);

            // ================= 4. WELCOME SCREEN =================
            picBackground = new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile(Path.Combine(resourceFolder, "dashboard_bg.png")),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            pnlOverlay = new Panel() { Size = new Size(600, 250), BackColor = Color.Transparent };
            Label lblWelcome = new Label() { Text = "WELCOME TO TUTOR MANAGER", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Height = 60, Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(0, 100, 90) };
            Label lblInst = new Label() { Text = "Select a module from the left sidebar to begin.", TextAlign = ContentAlignment.TopCenter, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 14), ForeColor = Color.DimGray };

            pnlOverlay.Controls.Add(lblInst);
            pnlOverlay.Controls.Add(lblWelcome);
            picBackground.Controls.Add(pnlOverlay);

            pnlMain.Controls.Add(picBackground);
            pnlMain.Controls.Add(pnlHeader); // Added to Main

            // ================= 5. FORM SETUP =================
            this.Text = "Tutor Manager";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);

            if (isAdmin)
            {               
                btnBackUp = CreateBtn("Backup", 285);               
                btnBackUp.Click += (s, e) => {
                    ActivateButton(btnBackUp);
                    lblTitle.Text = "Backup Management";
                    OpenForm(new BackupForm());
                };          
                pnlSidebar.Controls.Add(btnBackUp);
                btnBackUp.BringToFront();
            }

            this.Load += MainForm_Load;
            this.Resize += (s, e) => CenterWelcomePanel();
        }

        private void CenterWelcomePanel()
        {
            if (pnlOverlay != null)
            {
                pnlOverlay.Location = new Point((pnlMain.Width - pnlOverlay.Width) / 2, (pnlMain.Height - pnlOverlay.Height) / 2);
            }
        }

        void ActivateButton(Button btn)
        {
            if (currentBtn != null) currentBtn.BackColor = sidebarColor;
            currentBtn = btn;
            btn.BackColor = Color.FromArgb(60, 60, 120);

            picBackground.Visible = false; // Hide welcome
            pnlHeader.Visible = true;     // Show clean header
        }

        private async Task PerformAutoBackupAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var repo = new BackupRepository();
                    string dbPath = Path.Combine(AppContext.BaseDirectory, "tutor.db");
                    if (File.Exists(dbPath))
                    {
                        repo.PerformAutoBackup(dbPath);
                    }
                });
            }
            catch (Exception ex)
            {
                // Don’t crash UI — log silently
                System.Diagnostics.Debug.WriteLine("Auto-backup failed: " + ex.Message);
            }
        }

        void OpenForm(Form form)
        {
            // Remove existing forms but keep the Header
            for (int i = pnlMain.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlMain.Controls[i] is Form) pnlMain.Controls.RemoveAt(i);
            }

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(form);
            form.BringToFront();
            form.Show();
        }

        Button CreateBtn(string text, int top)
        {
            Button btn = new Button() { Text = "    " + text, Width = 180, Height = 50, Top = top, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = sidebarColor, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 11) };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Center your welcome screen
            CenterWelcomePanel();            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!isLoggingOut && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MessageBox.Show("Please use Logout button.", "Action not allowed");
                return;
            }

            base.OnFormClosing(e);
        }
    }
}