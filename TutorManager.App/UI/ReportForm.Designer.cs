using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Models;

namespace TutorManager.App.UI
{
    public partial class ReportForm : Form
    {
        // Removed cmbGrade
        ComboBox cmbLevel;
        DateTimePicker dtFrom, dtTo;
        Button btnLoad, btnExport;

        Label lblFinalTotal;
        Panel pnlHeader;
        DataGridView grid;

        StudentRepository studentRepo = new StudentRepository();
        AttendanceRepository attRepo = new AttendanceRepository();

        private void InitializeComponent()
        {
            this.Text = "Financial Reports";
            this.Size = new Size(1150, 750);
            this.BackColor = Color.FromArgb(245, 247, 250);

            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = Color.ForestGreen
            };

            // ================= TOP BAR =================
            Panel top = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White
            };

            // Maths Level Filter (Shifted to the left since Grade is gone)
            Label lblLevel = new Label() { Text = "Filter by Maths Level", Left = 20, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            cmbLevel = new ComboBox()
            {
                Left = 20,
                Top = 32,
                Width = 180,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblFrom = new Label() { Text = "From Date", Left = 220, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtFrom = new DateTimePicker()
            {
                Left = 220,
                Top = 32,
                Width = 160,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            Label lblTo = new Label() { Text = "To Date", Left = 400, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtTo = new DateTimePicker()
            {
                Left = 400,
                Top = 32,
                Width = 160,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            btnLoad = new Button()
            {
                Text = "Generate Report",
                Left = 580,
                Top = 28,
                Width = 150,
                Height = 38,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10)
            };
            btnLoad.FlatAppearance.BorderSize = 0;

            btnExport = new Button()
            {
                Text = "Export CSV",
                Left = 740,
                Top = 28,
                Width = 130,
                Height = 38,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10),
                Enabled = false
            };
            btnExport.FlatAppearance.BorderSize = 0;

            btnLoad.Click += (s, e) => LoadReport();
            btnExport.Click += (s, e) => ExportToCsv();

            top.Controls.AddRange(new Control[] { lblLevel, cmbLevel, lblFrom, dtFrom, lblTo, dtTo, btnLoad, btnExport });

            // ================= GRID =================
            grid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 35 }
            };

            // ================= BOTTOM BAR =================
            Panel bottom = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(0, 0, 30, 0)
            };

            lblFinalTotal = new Label()
            {
                Text = "Final Total: $0.00",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Dock = DockStyle.Right,
                Width = 400,
                TextAlign = ContentAlignment.MiddleRight
            };

            bottom.Controls.Add(lblFinalTotal);

            this.Controls.Add(grid);
            this.Controls.Add(bottom);
            this.Controls.Add(top);
            this.Controls.Add(pnlHeader);

            LoadFilterData();
            SetupGrid();
        }

        void SetupGrid()
        {
            grid.Columns.Clear();
            grid.Columns.Add("Date", "Date");            // Index 0
            grid.Columns.Add("Time", "Batch Time");      // Index 1
            grid.Columns.Add("Name", "Student Name");    // Index 2
            grid.Columns.Add("Grade", "Grade");          // Index 3
            grid.Columns.Add("Level", "Maths Level");    // Index 4
            grid.Columns.Add("Hours", "Hours");          // Index 5
            grid.Columns.Add("Rate", "Hourly Rate");     // Index 6
            grid.Columns.Add("Amount", "Total Earnings");// Index 7

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 136);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 45;
        }

        void LoadFilterData()
        {
            cmbLevel.Items.Clear();
            cmbLevel.Items.Add("All Levels");

            var levels = studentRepo.GetLevels();
            foreach (var lvl in levels)
            {
                cmbLevel.Items.Add(lvl);
            }
            cmbLevel.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddDays(-14);
            dtTo.Value = DateTime.Today;
        }

        void LoadReport()
        {
            grid.Rows.Clear();
            string selectedLevel = cmbLevel.Text;
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;

            // 1. Get all active students first for reference
            var allStudents = studentRepo.GetAll().Where(x => x.IsActive == 1).ToList();

            // 2. Get all present attendance within the date range
            var attendance = attRepo.GetAll()
                .Where(x => x.ClassDate >= from && x.ClassDate <= to && x.IsPresent)
                .OrderByDescending(x => x.ClassDate) // Show newest first
                .ToList();

            decimal grandTotal = 0;

            foreach (var att in attendance)
            {
                // Find the student linked to this specific attendance record
                var s = allStudents.FirstOrDefault(x => x.Id == att.StudentId);

                // Skip if student doesn't match the Level filter
                if (s == null) continue;
                if (selectedLevel != "All Levels" && s.LevelName != selectedLevel) continue;

                decimal amount = att.HoursWorked * s.HourlyRate;
                grandTotal += amount;

                grid.Rows.Add(
                att.ClassDate.ToShortDateString(), // 0: Date
                att.BatchTime ?? "N/A",            // 1: Time (This was likely missing!)
                s.Name,                            // 2: Name
                s.Grade,                           // 3: Grade
                s.LevelName,                       // 4: Level
                att.HoursWorked.ToString("N2"),    // 5: Hours
                s.HourlyRate.ToString("C2"),       // 6: Rate
                amount.ToString("C2")              // 7: Amount (Total Earnings)
        );
            }

            lblFinalTotal.Text = $"Final Total: {grandTotal.ToString("C2")}";
            btnExport.Enabled = grid.Rows.Count > 0;
        }

        void ExportToCsv()
        {
            if (grid.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV file|*.csv",
                FileName = $"Financial_Report_{cmbLevel.Text}_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        // Headers - Added Grade
                        sw.WriteLine("Date,Student,Grade,Math Level,Hours,Rate,Amount");

                        foreach (DataGridViewRow row in grid.Rows)
                        {
                            if (row.IsNewRow) continue;
                            sw.WriteLine($"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value},{row.Cells[3].Value},{row.Cells[4].Value},\"{row.Cells[5].Value}\",\"{row.Cells[6].Value}\",\"{row.Cells[7].Value}\"");
                        }

                        sw.WriteLine();
                        sw.WriteLine($",,,,,, {lblFinalTotal.Text.Replace(",", "")}");
                    }
                    MessageBox.Show("Exported Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}