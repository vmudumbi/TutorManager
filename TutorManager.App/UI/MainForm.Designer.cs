using System;
using System.Drawing;
using System.Windows.Forms;
using TutorManager.App.UI;
using TutorManager.App.Utilities;

namespace TutorManager.App
{
    public partial class MainForm : Form
    {
        Panel pnlSidebar, pnlHeader, pnlMain, pnlLogo;
        Button btnStudents, btnAttendance, btnReports;
        Label lblTitle, lblLogoSub;
        Panel pnlLogin;
        TextBox txtUser, txtPass;

        Color sidebarColor = Color.FromArgb(24, 28, 36);
        Color headerColor = Color.FromArgb(236, 240, 243); // soft light gray
        Color hoverColor = Color.FromArgb(45, 50, 65);
        Color selectedColor = Color.FromArgb(70, 130, 180);

        private Button currentBtn = null;

        private void InitializeComponent()
        {
            string projectRoot =
             Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));

            string resourceFolder = Path.Combine(projectRoot, "Resources");
            // ================= HEADER =================
            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.ForestGreen
            };

            lblTitle = new Label()
            {
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,       // White text looks very "nice" on green
                BackColor = Color.Transparent, // This removes the white box behind the text
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(20, 15)   // Adjust to center vertically
            };

            Panel headerBorder = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(230, 230, 230)
            };

                      
            pnlHeader.Controls.Add(headerBorder);

            // ================= SIDEBAR =================
            pnlSidebar = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = sidebarColor
            };

            // ===== TOP LOGO PANEL (STUDY STYLE) =====
            pnlLogo = new Panel()
            {
                Height = 80, // Slightly shorter for a sleeker look
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 28, 38) // A deeper dark than the sidebar
            };

            lblLogoSub = new Label()
            {
                Text = "Manager",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10),
                Location = new Point(70, 45),
                AutoSize = true
            };


            PictureBox picLogo = new PictureBox()
            {
                Size = new Size(32, 32), // Smaller is often classier
                Location = new Point(20, 24),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Path.Combine(resourceFolder, "study.png")),
                BackColor = Color.Transparent
            };

            Label lblTitleLogo = new Label()
            {
                Text = "TUTOR", // Split the name for better styling
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(60, 20),
                AutoSize = true
            };

            Label lblManager = new Label()
            {
                Text = "MANAGER",
                ForeColor = Color.FromArgb(0, 180, 160), // Use your Teal Green for part of the text
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(60, 38), // Stacked slightly below
                AutoSize = true
            };

            //pnlLogo.Controls.Add(lblIcon);
            pnlLogo.Controls.Add(picLogo);
            pnlLogo.Controls.Add(lblTitleLogo);
            pnlLogo.Controls.Add(lblManager);
            pnlLogo.Controls.Add(lblLogoSub);

            // ================= BUTTONS =================
            btnStudents = CreateBtn("Students", 120);
            btnAttendance = CreateBtn("Attendance", 180);
            btnReports = CreateBtn("Reports", 240);

            this.Load += MainForm_Load;

            btnStudents.Click += (s, e) =>
            {
                ActivateButton(btnStudents);
                lblTitle.Text = "Students";
                OpenForm(new StudentForm());
            };

            btnAttendance.Click += (s, e) =>
            {
                ActivateButton(btnAttendance);
                lblTitle.Text = "Attendance";
                OpenForm(new LogHoursForm());
            };

            btnReports.Click += (s, e) =>
            {
                ActivateButton(btnReports);
                lblTitle.Text = "Reports";
                OpenForm(new ReportForm());
            };

            pnlSidebar.Controls.Add(pnlLogo);
            pnlSidebar.Controls.Add(btnStudents);
            pnlSidebar.Controls.Add(btnAttendance);
            pnlSidebar.Controls.Add(btnReports);

            // ================= MAIN =================
            pnlMain = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            // ================= FORM =================
            this.Text = "Tutor Manager";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            ShowLoader();
            await Task.Delay(1200);
            HideLoader();
        }

        // ================= BUTTON STYLE =================
        Button CreateBtn(string text, int top)
        {
            Button btn = new Button()
            {
                Text = "   " + text,
                Width = 220,
                Height = 50,
                Top = top,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = sidebarColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11)
            };

            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) =>
            {
                if (btn != currentBtn)
                    btn.BackColor = hoverColor;
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn != currentBtn)
                    btn.BackColor = sidebarColor;
            };

            return btn;
        }

        // ================= ACTIVE BUTTON =================
        void ActivateButton(Button btn)
        {
            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is Button b)
                    b.BackColor = Color.Transparent;
            }

            btn.BackColor = Color.FromArgb(60, 60, 120);
        }

        // ================= LOAD FORM =================
        void OpenForm(Form form)
        {
            pnlMain.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            pnlMain.Controls.Add(form);
            form.Show();
        }

        Panel loadingPanel;        
        Spinner spinner;

        void ShowLoader()
        {
            loadingPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(180, 255, 255, 255)
            };

            spinner = new Spinner();

            spinner.Location = new Point(
                (this.Width / 2) - 30,
                (this.Height / 2) - 30
            );

            loadingPanel.Controls.Add(spinner);
            this.Controls.Add(loadingPanel);
            loadingPanel.BringToFront();
        }

        void HideLoader()
        {
            this.Controls.Remove(loadingPanel);
        }
    }
}