using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;
using TutorManager.App.Utility;

namespace TutorManager.App.UI
{
    public partial class StudentForm : Form
    {
        private DataGridView grid;
        private TextBox txtName, txtEmail, txtPhone, txtSchool, txtDescription;
        private NumericUpDown numRate;
        private Button btnSave, btnNew;
        private ComboBox cmbGrade, cmbMaths;
        private CheckBox chkActive;

        private Student selectedStudent = null;
        private StudentRepository studentrepo = new StudentRepository();

        private Color accentTeal = Color.FromArgb(0, 180, 160);

        public void InitializeComponent()
        {
            this.Text = "Student Management";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            Panel rightPanel = new Panel { Dock = DockStyle.Right, Width = 420, Padding = new Padding(20), BackColor = Color.White };            

            // GRID
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            grid.CellClick += Grid_CellClick;
            leftPanel.Controls.Add(grid);

            // FORM LAYOUT (KEY FIX)
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 20,
                AutoScroll = true
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // helper method
            void Add(Control c)
            {
                c.Dock = DockStyle.Top;
                layout.Controls.Add(c);
            }

            Add(CreateLabel("Student Details", true));

            txtName = CreateTextBox("Full Name");
            Add(txtName);

            txtSchool = CreateTextBox("School Name");
            Add(txtSchool);

            cmbGrade = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGrade.Items.AddRange(new object[] { "-- Select Grade --", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12" });
            cmbGrade.SelectedIndex = 0;
            Add(cmbGrade);
            
            cmbMaths = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            Add(cmbMaths);
            LoadMathsLevels();

            Add(CreateLabel("Hourly Rate ($)"));
            numRate = new NumericUpDown { Dock = DockStyle.Top, Value = 20 };
            Add(numRate);           

            txtEmail = CreateTextBox("Email");
            Add(txtEmail);

            txtPhone = CreateTextBox("Phone");
            txtPhone.TextChanged += TxtPhone_TextChanged;
            Add(txtPhone);

            txtDescription = new TextBox
            {
                Multiline = true,
                Height = 80,
                Dock = DockStyle.Top,
                PlaceholderText = "Description"
            };
            Add(txtDescription);

            chkActive = new CheckBox { Text = "Active Student", Dock = DockStyle.Top, Checked = true };
            Add(chkActive);

            btnSave = new Button
            {
                Text = "Save",
                Height = 40,
                BackColor = accentTeal,
                ForeColor = Color.White,
                Dock = DockStyle.Top
            };

            btnNew = new Button
            {
                Text = "Clear",
                Height = 40,
                Dock = DockStyle.Top
            };

            btnSave.Click += BtnSave_Click;
            btnNew.Click += (s, e) => ResetForm();

            Add(btnSave);
            Add(btnNew);

            rightPanel.Controls.Add(layout);

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);

            grid.DataBindingComplete += Grid_DataBindingComplete;

            LoadGrid();
        }

        private void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = false;

            if (grid.Columns["Name"] != null)
            {
                grid.Columns["Name"].Visible = true;
                grid.Columns["Name"].HeaderText = "Student Name";
            }

            if (grid.Columns["LevelName"] != null)
            {
                grid.Columns["LevelName"].Visible = true;
                grid.Columns["LevelName"].HeaderText = "Maths Level";
            }

            if (grid.Columns["Email"] != null)
                grid.Columns["Email"].Visible = true;

            if (grid.Columns["Phone"] != null)
            {
                grid.Columns["Phone"].Visible = true;
                grid.Columns["Phone"].HeaderText = "Phone";
            }

            if (grid.Columns["HourlyRate"] != null)
            {
                grid.Columns["HourlyRate"].Visible = true;
                grid.Columns["HourlyRate"].HeaderText = "Rate ($)";
                grid.Columns["HourlyRate"].DefaultCellStyle.Format = "C";
            }

            // Auto size ONLY visible columns
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Visible)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private Label CreateLabel(string text, bool isTitle = false)
        {
            return new Label
            {
                Text = text,
                Font = isTitle ? new Font("Segoe UI", 14, FontStyle.Bold) : new Font("Segoe UI", 9),
                ForeColor = isTitle ? accentTeal : Color.Black,
                Height = isTitle ? 40 : 20,
                Dock = DockStyle.Top
            };
        }

        private TextBox CreateTextBox(string placeholder)
        {
            return new TextBox
            {
                PlaceholderText = placeholder,
                Dock = DockStyle.Top
            };
        }

        private void LoadGrid()
        {
            grid.DataSource = null;
            grid.DataSource = studentrepo.GetAll();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // NAME
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Student Name is required.", txtName); return;
            }

            // GRADE
            if (cmbGrade.SelectedIndex <= 0)
            {
                ShowError("Please select a Grade level.", cmbGrade); return;
            }

            // MATH LEVEL
            if (cmbMaths.SelectedValue == null || (int)cmbMaths.SelectedValue == -1)
            {
                ShowError("Please select a valid Maths Level.", cmbMaths); return;
            }

            // RATE
            if (numRate.Value <= 0)
            {
                ShowError("Hourly Rate must be greater than 0.", numRate); return;
            }

            // EMAIL
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                ShowError("Invalid email format.", txtEmail); return;
            }

            // PHONE
            string digits = new string(txtPhone.Text.Where(char.IsDigit).ToArray());
            if (digits.Length != 10)
            {
                ShowError("Phone must be exactly 10 digits.", txtPhone); return;
            }

            // DATA
            var student = selectedStudent ?? new Student();

            student.Name = txtName.Text;
            student.SchoolName = txtSchool.Text;
            student.Grade = cmbGrade.Text;
            student.LevelId = (int)cmbMaths.SelectedValue;
            student.HourlyRate = numRate.Value;
            student.Email = txtEmail.Text;
            student.Phone = txtPhone.Text;
            student.Description = txtDescription.Text;
            student.IsActive = chkActive.Checked ? 1 : 0;

            // SAVE
            await Task.Run(() =>
            {
                if (selectedStudent == null)
                    studentrepo.Add(student);
                else
                    studentrepo.Update(student);
            });

            ResetForm();
            LoadGrid();
        }

        private void ShowError(string msg, Control ctrl)
        {
            MessageBox.Show(msg);
            ctrl.Focus();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            selectedStudent = (Student)grid.Rows[e.RowIndex].DataBoundItem;

            txtName.Text = selectedStudent.Name;
            txtSchool.Text = selectedStudent.SchoolName;
            cmbGrade.Text = selectedStudent.Grade;
            cmbMaths.SelectedValue = selectedStudent.LevelId;
            txtEmail.Text = selectedStudent.Email;
            txtPhone.Text = selectedStudent.Phone;
            numRate.Value = selectedStudent.HourlyRate;
            txtDescription.Text = selectedStudent.Description;
            chkActive.Checked = selectedStudent.IsActive == 1;
        }

        private void ResetForm()
        {
            selectedStudent = null;
            txtName.Clear();
            txtSchool.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtDescription.Clear();
            cmbGrade.SelectedIndex = 0;
            cmbMaths.SelectedIndex = 0;
            numRate.Value = 20;
            chkActive.Checked = true;
        }

        private void LoadMathsLevels()
        {
            var levels = studentrepo.GetLevels();
            levels.Insert(0, new MathsLevel { Id = -1, LevelName = "-- Select Level --" });

            cmbMaths.DataSource = levels;
            cmbMaths.DisplayMember = "LevelName";
            cmbMaths.ValueMember = "Id";
        }

        private bool formatting = false;

        private void TxtPhone_TextChanged(object sender, EventArgs e)
        {
            if (formatting) return;
            formatting = true;

            string digits = new string(txtPhone.Text.Where(char.IsDigit).ToArray());

            if (digits.Length > 10)
                digits = digits.Substring(0, 10);

            string f = "";
            if (digits.Length > 0) f = "(" + digits.Substring(0, Math.Min(3, digits.Length));
            if (digits.Length >= 3) f += ")-" + digits.Substring(3, Math.Min(3, digits.Length - 3));
            if (digits.Length >= 6) f += "-" + digits.Substring(6);

            txtPhone.Text = f;
            txtPhone.SelectionStart = txtPhone.Text.Length;

            formatting = false;
        }
    }
}