using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;

namespace TutorManager.App.UI
{
    public partial class StudentForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtEmail, txtPhone, txtSchool, txtDescription;
        private NumericUpDown numRate;
        private Button btnSave, btnNew;
        private ComboBox cmbGrade;
        private CheckBox chkActive;

        private Student selectedStudent = null;
        private StudentRepository studentrepo = new StudentRepository();

        // Professional Theme Colors
        private Color accentTeal = Color.FromArgb(0, 180, 160);
        private Color bodyBg = Color.FromArgb(240, 243, 247);
        private Color gridHeaderBg = Color.FromArgb(232, 234, 237);
        private Color borderGray = Color.DarkGray; 
      

        public void InitializeComponent()
        {
            // Panels
            Panel pnlLeft = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(15) };
            Panel pnlRight = new Panel() { Dock = DockStyle.Right, Width = 380, BackColor = Color.White, Padding = new Padding(25) };
            Panel pnlDivider = new Panel() { Dock = DockStyle.Right, Width = 1, BackColor = Color.FromArgb(224, 224, 224) };

            // ================= GRID =================
            grid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(235, 235, 235),
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 40 }
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = gridHeaderBg;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            grid.ColumnHeadersHeight = 45;

            grid.DataBindingComplete += (s, e) => {
                if (grid.Columns["Id"] != null) grid.Columns["Id"].Visible = false;
                if (grid.Columns["IsActive"] != null) grid.Columns["IsActive"].Visible = false;
                if (grid.Columns["Description"] != null) grid.Columns["Description"].Visible = false;
                foreach (DataGridViewColumn col in grid.Columns) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            };

            grid.CellClick += Grid_CellClick;
            pnlLeft.Controls.Add(grid);

            // ================= FORM =================
            Label lblFormTitle = new Label() { Text = "Student Details", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = accentTeal, Dock = DockStyle.Top, Height = 25 };

            // Create Inputs
            txtName = CreateStyledBox("Full Name", 70);
            txtSchool = CreateStyledBox("School Name", 130);

            cmbGrade = new ComboBox() { Left = 25, Top = 195, Width = 330, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };
            cmbGrade.Items.AddRange(new object[] { "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12" });

            Label lblRate = new Label() { Text = "Hourly Rate ($)", Left = 25, Top = 235, AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = Color.DimGray };
            numRate = new NumericUpDown() { Left = 25, Top = 255, Width = 100, Value = 20, Font = new Font("Segoe UI", 11) };

            txtEmail = CreateStyledBox("Email Address", 305);
            txtPhone = CreateStyledBox("Phone (000)-000-0000", 365);
            txtPhone.TextChanged += TxtPhone_TextChanged;

            
            txtDescription = new TextBox()
            {
                Left = 25,
                Top = 425,
                Width = 330,
                Height = 85,
                Multiline = true,
                PlaceholderText = "Notes / Description",
                Font = new Font("Segoe UI", 10.5f),
                BackColor = Color.White,
                ScrollBars = ScrollBars.Vertical
            };

            chkActive = new CheckBox() { Text = "Active Student", Left = 25, Top = 600, Checked = true, AutoSize = true, Font = new Font("Segoe UI", 10) };

            btnSave = new Button() { Text = "Save Student", Left = 25, Top = 550, Width = 160, Height = 45, BackColor = accentTeal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10) };
            btnSave.FlatAppearance.BorderSize = 0;

            btnNew = new Button() { Text = "Clear Form", Left = 195, Top = 550, Width = 140, Height = 45, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btnNew.FlatAppearance.BorderColor = Color.Black;

            btnSave.Click += BtnSave_Click;
            btnNew.Click += (s, e) => ResetForm();

            pnlRight.Controls.AddRange(new Control[] { lblFormTitle, txtName, txtSchool, cmbGrade, lblRate, numRate, txtEmail, txtPhone, txtDescription, chkActive, btnSave, btnNew });

            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlDivider);
            this.Controls.Add(pnlRight);
            LoadGrid();
        }

        private TextBox CreateStyledBox(string placeholder, int top)
        {
            return new TextBox()
            {
                PlaceholderText = placeholder,
                Left = 25,
                Top = top,
                Width = 330,
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5) 
            };
        }

        private void LoadGrid() { grid.DataSource = null; grid.DataSource = studentrepo.GetAll(); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name required"); return; }
            if (selectedStudent == null)
            {
                studentrepo.Add(new Student { Name = txtName.Text, SchoolName = txtSchool.Text, Grade = cmbGrade.Text, HourlyRate = numRate.Value, Email = txtEmail.Text, Phone = txtPhone.Text, Description = txtDescription.Text, IsActive = chkActive.Checked ? 1 : 0 });
            }
            else
            {
                selectedStudent.Name = txtName.Text; selectedStudent.SchoolName = txtSchool.Text; selectedStudent.Grade = cmbGrade.Text; selectedStudent.HourlyRate = numRate.Value; selectedStudent.Email = txtEmail.Text; selectedStudent.Phone = txtPhone.Text; selectedStudent.Description = txtDescription.Text; selectedStudent.IsActive = chkActive.Checked ? 1 : 0;
                studentrepo.Update(selectedStudent);
            }
            ResetForm(); LoadGrid();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            selectedStudent = (Student)grid.Rows[e.RowIndex].DataBoundItem;
            txtName.Text = selectedStudent.Name; txtSchool.Text = selectedStudent.SchoolName; cmbGrade.Text = selectedStudent.Grade; txtEmail.Text = selectedStudent.Email; txtPhone.Text = selectedStudent.Phone; numRate.Value = selectedStudent.HourlyRate; txtDescription.Text = selectedStudent.Description; chkActive.Checked = selectedStudent.IsActive == 1;
            btnSave.Text = "Update Student";
        }

        private void ResetForm()
        {
            selectedStudent = null; txtName.Clear(); txtSchool.Clear(); txtEmail.Clear(); txtPhone.Clear(); txtDescription.Clear(); cmbGrade.SelectedIndex = -1; numRate.Value = 20; chkActive.Checked = true; btnSave.Text = "Save Student";
        }

        private bool _formatting = false;
        private void TxtPhone_TextChanged(object sender, EventArgs e)
        {
            if (_formatting) return; _formatting = true;
            string digits = new string(txtPhone.Text.Where(char.IsDigit).ToArray());
            if (digits.Length > 10) digits = digits.Substring(0, 10);
            string f = "";
            if (digits.Length > 0) f = "(" + digits.Substring(0, Math.Min(3, digits.Length));
            if (digits.Length >= 3) f += ")-" + digits.Substring(3, Math.Min(3, digits.Length - 3));
            if (digits.Length >= 6) f += "-" + digits.Substring(6);
            txtPhone.Text = f; txtPhone.SelectionStart = txtPhone.Text.Length;
            _formatting = false;
        }
    }
}