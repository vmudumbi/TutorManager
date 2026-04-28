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
        DataGridView grid;
        TextBox txtName, txtEmail, txtPhone;
        NumericUpDown numRate;
        Button btnSave, btnNew;
        TextBox txtSchool, txtDescription;
        ComboBox cmbGrade;
        CheckBox chkActive;

        Student selectedStudent = null;
        private StudentRepository studentrepo = new StudentRepository();

        public void InitializeComponent()
        {
            //this.Text = "Students";
            //this.Size = new Size(900, 600);
            //this.MinimumSize = new Size(900, 600);
            //this.MaximumSize = new Size(900, 600);
            //this.BackColor = Color.White;

            //Panel top = new Panel()
            //{
            //    Dock = DockStyle.Top,
            //    Height = 80,
            //    BackColor = Color.FromArgb(0, 120, 110)
            //};
           
            // ================= GRID =================
            grid = new DataGridView()
            {
                Dock = DockStyle.Left,
                Width = 500,
                BackgroundColor = Color.White,
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            grid.DataBindingComplete += (s, e) =>
            {
                if (grid.Columns["Id"] != null)
                    grid.Columns["Id"].Visible = false;
            };

            grid.CellClick += Grid_CellClick;

            // ================= RIGHT PANEL =================
            Panel right = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            Label title = new Label()
            {
                Text = "Add / Update Student",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Left = 20,
                Top = 10
            };

            // ================= INPUTS =================
            txtName = CreateBox("Name", 60);
            txtSchool = CreateBox("School Name", 110);

            cmbGrade = new ComboBox()
            {
                Left = 20,
                Top = 160,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbGrade.Items.AddRange(new object[]
            {
                "Grade 8","Grade 9","Grade 10","Grade 11","Grade 12"
            });

            // ================= RATE =================
            Panel ratePanel = new Panel()
            {
                Left = 20,
                Top = 210,
                Width = 250,
                Height = 35
            };

            numRate = new NumericUpDown()
            {
                Width = 100,
                Value = 20,
                Minimum = 1,
                Maximum = 500
            };

            Label lblRate = new Label()
            {
                Text = "per/hr",
                Left = 110,
                Top = 5,
                AutoSize = true,
                ForeColor = Color.Gray
            };

            ratePanel.Controls.Add(numRate);
            ratePanel.Controls.Add(lblRate);

            txtEmail = CreateBox("Email", 260);
            txtPhone = CreateBox("Phone", 310);
            txtPhone.TextChanged += TxtPhone_TextChanged;
            txtPhone.MaxLength = 14;

            txtDescription = new TextBox()
            {
                Left = 20,
                Top = 360,
                Width = 300,
                Height = 80,
                Multiline = true,
                PlaceholderText = "Description"
            };

            chkActive = new CheckBox()
            {
                Text = "Active",
                Left = 20,
                Top = 460,
                Checked = true,
                AutoSize = true
            };

            // ================= BUTTONS =================
            btnSave = new Button()
            {
                Text = "Add",
                Left = 20,
                Top = 500,
                Width = 120,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnNew = new Button()
            {
                Text = "New",
                Left = 160,
                Top = 500,
                Width = 120
            };

            btnSave.Click += BtnSave_Click;
            btnNew.Click += (s, e) => ResetForm();

            // ================= ADD CONTROLS =================
            right.Controls.Add(title);
            right.Controls.Add(txtName);
            right.Controls.Add(txtSchool);
            right.Controls.Add(cmbGrade);
            right.Controls.Add(ratePanel);
            right.Controls.Add(txtEmail);
            right.Controls.Add(txtPhone);
            right.Controls.Add(txtDescription);
            right.Controls.Add(chkActive);
            right.Controls.Add(btnSave);
            right.Controls.Add(btnNew);

            this.Controls.Add(right);
            this.Controls.Add(grid);
            //this.Controls.Add(top);

            LoadGrid();
        }

        // ================= LOAD =================
        private void LoadGrid()
        {
            var data = studentrepo.GetAll();

            // FIX: convert bool/int safely if needed
            grid.DataSource = null;
            grid.DataSource = data;

            if (grid.Columns["IsActive"] != null)
                grid.Columns["IsActive"].Visible = false;
        }

        // ================= SAVE / UPDATE =================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name required");
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbGrade.Text))
            {
                MessageBox.Show("Grade required");
                return;
            }

            if (selectedStudent == null)
            {
                studentrepo.Add(new Student
                {
                    Name = txtName.Text,
                    SchoolName = txtSchool.Text,
                    Grade = cmbGrade.Text,
                    HourlyRate = numRate.Value,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    Description = txtDescription.Text,
                    IsActive = chkActive.Checked ? 1 : 0
                });
            }
            else
            {
                selectedStudent.SchoolName = txtSchool.Text;
                selectedStudent.Phone = txtPhone.Text;
                selectedStudent.Grade = cmbGrade.Text;
                selectedStudent.HourlyRate = numRate.Value;
                selectedStudent.Description = txtDescription.Text;
                selectedStudent.IsActive = chkActive.Checked ? 1 : 0;

                studentrepo.Update(selectedStudent);
            }

            ResetForm();
            LoadGrid();
        }

        // ================= SELECT =================
        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            selectedStudent = (Student)grid.Rows[e.RowIndex].DataBoundItem;

            txtName.Text = selectedStudent.Name;
            txtSchool.Text = selectedStudent.SchoolName;
            cmbGrade.Text = selectedStudent.Grade;
            txtEmail.Text = selectedStudent.Email;
            txtPhone.Text = selectedStudent.Phone;
            numRate.Value = selectedStudent.HourlyRate;
            txtDescription.Text = selectedStudent.Description;
            chkActive.Checked = selectedStudent.IsActive == 1;

            btnSave.Text = "Update";
        }

        // ================= RESET =================
        private void ResetForm()
        {
            selectedStudent = null;

            txtName.Text = "";
            txtSchool.Text = "";
            cmbGrade.SelectedIndex = -1;
            txtEmail.Text = "";
            txtPhone.Text = "";
            numRate.Value = 20;
            txtDescription.Text = "";
            chkActive.Checked = true;

            btnSave.Text = "Add";
        }

        // ================= PHONE FORMAT =================
        private bool _formatting = false;

        private void TxtPhone_TextChanged(object sender, EventArgs e)
        {
            if (_formatting) return;
            _formatting = true;

            string digits = new string(txtPhone.Text.Where(char.IsDigit).ToArray());

            if (digits.Length > 10)
                digits = digits.Substring(0, 10);

            string f = "";
            if (digits.Length > 0) f = "(" + digits.Substring(0, Math.Min(3, digits.Length));
            if (digits.Length >= 3) f += ")-" + digits.Substring(3, Math.Min(3, digits.Length - 3));
            if (digits.Length >= 6) f += "-" + digits.Substring(6);

            txtPhone.Text = f;
            txtPhone.SelectionStart = txtPhone.Text.Length;

            _formatting = false;
        }

        private TextBox CreateBox(string placeholder, int top)
        {
            return new TextBox()
            {
                PlaceholderText = placeholder,
                Left = 20,
                Top = top,
                Width = 300
            };
        }
    }
}