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
        ComboBox cmbGrade;
        DateTimePicker dtFrom, dtTo;
        Button btnLoad, btnExport;

        Label lblFinalTotal;
        Panel pnlHeader;
        DataGridView grid;

        StudentRepository studentRepo = new StudentRepository();
        AttendanceRepository attRepo = new AttendanceRepository();

        private void InitializeComponent()
        {
            this.Text = "Reports";
            this.Size = new Size(1100, 700);
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

            Label lblGrade = new Label() { Text = "Select Grade", Left = 20, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            cmbGrade = new ComboBox()
            {
                Left = 20,
                Top = 32,
                Width = 140,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblFrom = new Label() { Text = "From Date", Left = 180, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtFrom = new DateTimePicker()
            {
                Left = 180,
                Top = 32,
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            Label lblTo = new Label() { Text = "To Date", Left = 400, Top = 12, AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtTo = new DateTimePicker()
            {
                Left = 400,
                Top = 32,
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            btnLoad = new Button()
            {
                Text = "Generate Report",
                Left = 620,
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
                Left = 780,
                Top = 28,
                Width = 130,
                Height = 38,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10),
                Enabled = false // Disabled by default until data is loaded
            };
            btnExport.FlatAppearance.BorderSize = 0;

            btnLoad.Click += (s, e) => LoadReport();
            btnExport.Click += (s, e) => ExportToCsv();

            top.Controls.AddRange(new Control[] { lblGrade, cmbGrade, lblFrom, dtFrom, lblTo, dtTo, btnLoad, btnExport });

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

            LoadGrades();
            SetupGrid();
        }

        void SetupGrid()
        {
            grid.Columns.Clear();
            grid.Columns.Add("Name", "Student Name");
            grid.Columns.Add("Sessions", "Sessions Attended");
            grid.Columns.Add("Hours", "Total Hours");
            grid.Columns.Add("Rate", "Hourly Rate");
            grid.Columns.Add("Amount", "Total Earnings");

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 150, 136);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 45;
        }

        void LoadGrades()
        {
            cmbGrade.Items.Clear();
            cmbGrade.Items.AddRange(new object[] { "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12" });
            cmbGrade.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddDays(-14);
            dtTo.Value = DateTime.Today;
        }

        void LoadReport()
        {
            grid.Rows.Clear();
            string grade = cmbGrade.Text;
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;

            var students = studentRepo.GetAll()
                .Where(x => x.Grade == grade && x.IsActive == 1)
                .ToList();

            var attendance = attRepo.GetAll()
                .Where(x => x.ClassDate >= from && x.ClassDate <= to && x.IsPresent)
                .ToList();

            decimal grandTotal = 0;

            foreach (var s in students)
            {
                var studentAtt = attendance.Where(x => x.StudentId == s.Id).ToList();

                decimal hours = studentAtt.Sum(x => x.HoursWorked);
                decimal rate = s.HourlyRate;
                decimal amount = hours * rate;

                grandTotal += amount;

                grid.Rows.Add(
                    s.Name,
                    studentAtt.Count,
                    hours.ToString("N2"),
                    rate.ToString("C2"),
                    amount.ToString("C2")
                );
            }

            lblFinalTotal.Text = $"Final Total: {grandTotal.ToString("C2")}";

            // Enable Export button only if there is data
            btnExport.Enabled = grid.Rows.Count > 0;
        }

        void ExportToCsv()
        {
            if (grid.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV file|*.csv",
                FileName = $"Report_{cmbGrade.Text}_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        // Headers
                        sw.WriteLine("Student,Sessions,Hours,Rate,Amount");

                        // Data rows
                        foreach (DataGridViewRow row in grid.Rows)
                        {
                            if (row.IsNewRow) continue;
                            sw.WriteLine($"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value},\"{row.Cells[3].Value}\",\"{row.Cells[4].Value}\"");
                        }

                        sw.WriteLine();
                        sw.WriteLine($",,,,{lblFinalTotal.Text.Replace(",", "")}");
                    }
                    MessageBox.Show("Exported Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}