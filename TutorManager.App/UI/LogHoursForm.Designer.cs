using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;

namespace TutorManager.App.UI
{
    public partial class LogHoursForm : Form
    {
        // Changed from cmbGrade to cmbLevel
        ComboBox cmbLevel;
        DateTimePicker dtDate;
        DataGridView grid;
        Button btnMarkAll, btnSave;
        Panel pnlHeader;

        StudentRepository studentRepo = new StudentRepository();
        AttendanceRepository attRepo = new AttendanceRepository();

        List<Student> students = new();
        private void InitializeComponent()
        {
            this.Text = "Log Attendance & Hours";
            this.Size = new Size(1000, 650);
            this.BackColor = Color.FromArgb(245, 245, 245);

            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = Color.ForestGreen
            };

            Panel top = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.White
            };

            // Level Filter Label
            Label lblFilter = new Label() { Text = "Maths Level:", Left = 20, Top = 5, AutoSize = true, Font = new Font("Segoe UI", 8) };

            cmbLevel = new ComboBox()
            {
                Left = 20,
                Top = 23,
                Width = 160,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLevel.SelectedIndexChanged += (s, e) => LoadStudents();

            dtDate = new DateTimePicker()
            {
                Left = 200,
                Top = 23,
                Width = 180,
                Font = new Font("Segoe UI", 10)
            };
            dtDate.ValueChanged += (s, e) => LoadStudents();

            btnMarkAll = new Button()
            {
                Text = "Mark All Present",
                Left = 400,
                Top = 20,
                Width = 130,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9)
            };
            btnMarkAll.Click += BtnMarkAll_Click;

            btnSave = new Button()
            {
                Text = "Save Attendance",
                Left = 540,
                Top = 20,
                Width = 130,
                Height = 35,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9),
                Enabled = false
            };
            btnSave.Click += BtnSave_Click;

            top.Controls.AddRange(new Control[] { lblFilter, cmbLevel, dtDate, btnMarkAll, btnSave });

            grid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EditMode = DataGridViewEditMode.EditOnEnter,
                RowTemplate = { Height = 35 }
            };

            // Selection Logic for Save Button
            grid.CellValueChanged += (s, e) => { btnSave.Enabled = true; };
            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            this.Controls.Add(grid);
            this.Controls.Add(top);
            this.Controls.Add(pnlHeader);

            SetupGrid();
            LoadLevels(); // Changed from LoadGrades
        }

        private void SetupGrid()
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 136);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            grid.ColumnHeadersHeight = 40;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Student Name", ReadOnly = true, FillWeight = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grade", HeaderText = "Grade", ReadOnly = true, FillWeight = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MathsLevel", HeaderText = "Level", ReadOnly = true, FillWeight = 100 });

            DataGridViewComboBoxColumn time = new DataGridViewComboBoxColumn();
            time.Name = "BatchTime";
            time.HeaderText = "Batch Time";
            time.Items.AddRange("6 AM", "7 AM", "8 AM", "9 AM", "10 AM", "11 AM", "12 PM", "1 PM", "2 PM", "3 PM", "4 PM", "5 PM", "6 PM", "7 PM", "8 PM");
            grid.Columns.Add(time);

            DataGridViewComboBoxColumn hours = new DataGridViewComboBoxColumn();
            hours.Name = "Hours";
            hours.HeaderText = "Hours";
            hours.Items.AddRange("1", "1.5", "2", "2.5", "3");
            grid.Columns.Add(hours);

            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Present", HeaderText = "Present", FillWeight = 60 });
        }

        private void LoadLevels()
        {
            cmbLevel.Items.Clear();
            var levels = studentRepo.GetLevels(); // Assuming this returns List<string> or List<MathsLevel>
            foreach (var lvl in levels)
            {
                cmbLevel.Items.Add(lvl);
            }

            if (cmbLevel.Items.Count > 0) cmbLevel.SelectedIndex = 0;
        }

        private void LoadStudents()
        {
            if (cmbLevel.SelectedItem == null) return;

            btnSave.Enabled = false;
            string selectedLevel = cmbLevel.Text;
            DateTime selectedDate = dtDate.Value.Date;

            // 1. Filter students by LevelName instead of Grade
            students = studentRepo.GetAll().FindAll(x => x.LevelName == selectedLevel && x.IsActive == 1);

            // 2. Get existing attendance for this level and date
            // Note: If your GetByDate uses grade, you might need to update that Repo method to filter by Level or Student IDs
            var attendanceList = attRepo.GetAll().Where(x => x.ClassDate == selectedDate).ToList();

            grid.Rows.Clear();

            foreach (var s in students)
            {
                var studentAtt = attendanceList.FirstOrDefault(x => x.StudentId == s.Id);

                string safeTime = studentAtt != null ? (studentAtt.BatchTime ?? "").Trim() : "4 PM"; // Default to a common time
                string safeHours = studentAtt != null ? studentAtt.HoursWorked.ToString() : "1.5";
                bool present = studentAtt != null && studentAtt.IsPresent;

                // Validating dropdown selections
                if (!((DataGridViewComboBoxColumn)grid.Columns["BatchTime"]).Items.Contains(safeTime)) safeTime = "4 PM";
                if (!((DataGridViewComboBoxColumn)grid.Columns["Hours"]).Items.Contains(safeHours)) safeHours = "1.5";

                grid.Rows.Add(s.Id, s.Name, s.Grade, s.LevelName, safeTime, safeHours, present);
            }
        }

        private void BtnMarkAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                row.Cells["Present"].Value = true;
            }
            btnSave.Enabled = true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                int studentId = Convert.ToInt32(row.Cells["Id"].Value);
                string batchTime = row.Cells["BatchTime"].Value?.ToString() ?? "";
                decimal.TryParse(row.Cells["Hours"].Value?.ToString(), out decimal hoursWorked);
                bool present = row.Cells["Present"].Value != null && Convert.ToBoolean(row.Cells["Present"].Value);

                attRepo.SaveAttendance(new Attendance
                {
                    StudentId = studentId,
                    ClassDate = dtDate.Value.Date,
                    BatchTime = batchTime,
                    IsPresent = present,
                    HoursWorked = present ? (hoursWorked == 0 ? 1 : hoursWorked) : 0
                });
            }

            MessageBox.Show("Attendance Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnSave.Enabled = false;
        }
    }
}