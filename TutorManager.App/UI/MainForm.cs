using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TutorManager.App
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.MinimumSize = new Size(1100, 700);
            this.MaximumSize = new Size(1100, 700);

            string projectRoot =
             Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));

            string resourceFolder = Path.Combine(projectRoot, "Resources");

            // ================= NICE BACKGROUND IMAGE =================
            PictureBox picBackground = new PictureBox()
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile(Path.Combine(resourceFolder, "dashboard_bg.png")),
                SizeMode = PictureBoxSizeMode.CenterImage, // Or Zoom if it's a full pattern
                BackColor = Color.FromArgb(245, 247, 250)
            };

            // ================= WELCOME OVERLAY =================
            // We put a transparent panel over the image to hold the text
            Panel pnlOverlay = new Panel()
            {
                Size = new Size(400, 200),
                BackColor = Color.FromArgb(180, 255, 255, 255), // Semi-transparent white
                BorderStyle = BorderStyle.None
            };

            Label lblWelcome = new Label()
            {
                Text = "WELCOME TO TUTOR MANAGER",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 110), // Your Teal Green
                BackColor = Color.Transparent
            };

            Label lblInstruction = new Label()
            {
                Text = "Select a module from the left sidebar\nto manage your students and reports.",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.DimGray,
                BackColor = Color.Transparent
            };

            pnlOverlay.Controls.Add(lblInstruction);
            pnlOverlay.Controls.Add(lblWelcome);

            // Center the overlay on top of the background image
            picBackground.Controls.Add(pnlOverlay);
            pnlOverlay.Location = new Point((pnlMain.Width - pnlOverlay.Width) / 2, 200);

            pnlMain.Controls.Add(picBackground);
        }
    }
}
