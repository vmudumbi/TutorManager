using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;

namespace TutorManager.App.UI
{
    public partial class LogHoursForm : Form
    {
        ComboBox cmbGrade;
        DateTimePicker dtDate;
        DataGridView grid;
        Button btnMarkAll, btnSave;

        StudentRepository studentRepo = new StudentRepository();
        AttendanceRepository attRepo = new AttendanceRepository();

        List<Student> students = new();

        // ================= UI =================
        private void InitializeComponent()
        {
            this.Text = "Attendance";
            this.Size = new Size(950, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);

            // TOP BAR
            Panel top = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White
            };

            cmbGrade = new ComboBox()
            {
                Left = 20,
                Top = 20,
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGrade.SelectedIndexChanged += (s, e) => LoadStudents();

            dtDate = new DateTimePicker()
            {
                Left = 180,
                Top = 20,
                Width = 200   // bigger width (your request)
            };

            btnMarkAll = new Button()
            {
                Text = "Mark All",
                Left = 400,
                Top = 18,
                Width = 120,
                Height = 32,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnMarkAll.Click += BtnMarkAll_Click;

            dtDate.ValueChanged += (s, e) => LoadStudents();

            btnSave = new Button()
            {
                Text = "Save",
                Left = 540,
                Top = 18,
                Width = 120,
                Height = 32,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            top.Controls.Add(cmbGrade);
            top.Controls.Add(dtDate);
            top.Controls.Add(btnMarkAll);
            top.Controls.Add(btnSave);

            // GRID
            grid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // HEADER STYLE
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;

            this.Controls.Add(grid);
            this.Controls.Add(top);

            SetupGrid();
            LoadGrades();
        }

        // ================= GRID =================
        private void SetupGrid()
        {
            grid.Columns.Clear();

            // ================= HEADER STYLE FIRST =================
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 136);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // ================= COLUMNS =================

            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.Name = "Id";
            idCol.HeaderText = "Id";
            idCol.Visible = false;
            grid.Columns.Add(idCol);

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "Name";
            nameCol.HeaderText = "Student Name";
            nameCol.ReadOnly = true;
            grid.Columns.Add(nameCol);

            DataGridViewTextBoxColumn gradeCol = new DataGridViewTextBoxColumn();
            gradeCol.Name = "Grade";
            gradeCol.HeaderText = "Grade";
            gradeCol.ReadOnly = true;
            grid.Columns.Add(gradeCol);

            // ================= TIME =================
            DataGridViewComboBoxColumn time = new DataGridViewComboBoxColumn();
            time.Name = "BatchTime";
            time.HeaderText = "Batch Time";
            time.Items.AddRange("6 AM", "7 AM", "8 AM", "9 AM", "10 AM",
                                "11 AM", "12 PM", "1 PM", "2 PM", "3 PM",
                                "4 PM", "5 PM", "6 PM", "7 PM", "8 PM");
            grid.Columns.Add(time);

            // ================= HOURS =================
            DataGridViewComboBoxColumn hours = new DataGridViewComboBoxColumn();
            hours.Name = "Hours";
            hours.HeaderText = "Hours";
            hours.Items.AddRange("1", "1.5", "2", "2.5", "3");
            grid.Columns.Add(hours);

            // ================= PRESENT =================
            DataGridViewCheckBoxColumn present = new DataGridViewCheckBoxColumn();
            present.Name = "Present";
            present.HeaderText = "Present";
            grid.Columns.Add(present);
        }

        // ================= LOAD =================
        private void LoadGrades()
        {
            cmbGrade.Items.Clear();

            cmbGrade.Items.AddRange(new object[]
            {
                "Grade 8","Grade 9","Grade 10","Grade 11","Grade 12"
            });

            cmbGrade.SelectedIndex = 0;
        }


        private void LoadStudents()
        {
            string grade = cmbGrade.Text;
            DateTime selectedDate = dtDate.Value.Date;

            students = studentRepo.GetAll()
                .FindAll(x => x.Grade == grade && x.IsActive == 1);

            var attendanceList = attRepo.GetByDate(grade, selectedDate);

            grid.Rows.Clear();

            foreach (var s in students)
            {
                var studentAtt = attendanceList
                    .Where(x => x.StudentId == s.Id)
                    .ToList();

                string safeTime = studentAtt.Any()
    ? (studentAtt[0].BatchTime ?? "").Trim()
    : "";

                string safeHours = studentAtt.Any()
                    ? studentAtt[0].HoursWorked.ToString()
                    : "1";

                bool present = studentAtt.Any() && studentAtt[0].IsPresent;

                // validate ComboBox values
                var timeCol = (DataGridViewComboBoxColumn)grid.Columns[3];
                if (!timeCol.Items.Contains(safeTime))
                    safeTime = "";

                var hoursCol = (DataGridViewComboBoxColumn)grid.Columns[4];
                if (!hoursCol.Items.Contains(safeHours))
                    safeHours = "1";

                grid.Rows.Add(
                    s.Id,
                    s.Name,
                    s.Grade,
                    safeTime,
                    safeHours,
                    present
                );    
            }
        }

        // ================= MARK ALL =================
        private void BtnMarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                row.Cells[4].Value = true;
            }
        }

        // ================= SAVE =================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                int studentId = Convert.ToInt32(row.Cells[0].Value);

                string batchTime = row.Cells[3].Value?.ToString() ?? "";

                decimal hoursWorked = 0;
                decimal.TryParse(row.Cells[4].Value?.ToString(), out hoursWorked);

                bool present = row.Cells[5].Value != null &&
                               Convert.ToBoolean(row.Cells[5].Value);

                if (present && hoursWorked == 0)
                    hoursWorked = 1;

                attRepo.SaveAttendance(new Attendance
                {
                    StudentId = studentId,
                    ClassDate = dtDate.Value.Date,
                    BatchTime = batchTime,
                    IsPresent = present,
                    HoursWorked = hoursWorked
                });
            }

            MessageBox.Show("Attendance Saved");
        }
    }
}