using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TutorManager.App.Utility
{
    public static class UIStyleHelper
    {

        public static async Task<bool> ExecuteWithProgress(string message, Func<Task<bool>> work)
        {
            using (var loadingForm = new Form())
            {
                loadingForm.FormBorderStyle = FormBorderStyle.None;
                loadingForm.StartPosition = FormStartPosition.CenterScreen;
                loadingForm.Size = new Size(400, 150);
                loadingForm.BackColor = Color.White;

                // Add the Custom Progress Bar
                var pBar = new ModernProgressBar
                {
                    Location = new Point(50, 80),
                    Size = new Size(300, 12),
                    Maximum = 100,
                    Value = 0
                };

                var lbl = new Label
                {
                    Text = message,
                    Location = new Point(0, 40),
                    Size = new Size(400, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 11, FontStyle.Regular)
                };

                loadingForm.Controls.Add(pBar);
                loadingForm.Controls.Add(lbl);
                loadingForm.Show();

                // Simulate smooth initial progress
                for (int i = 0; i <= 30; i += 5)
                {
                    pBar.Value = i;
                    await Task.Delay(100);
                }

                // Execute actual work
                bool isSuccess = await work();

                // Complete the bar
                for (int i = pBar.Value; i <= 100; i += 10)
                {
                    pBar.Value = i;
                    await Task.Delay(10);
                }

                // Update UI for result
                lbl.Text = isSuccess ? "Success!" : "Failed!";
                lbl.ForeColor = isSuccess ? Color.ForestGreen : Color.Firebrick;

                await Task.Delay(1000);
                loadingForm.Close();
                return isSuccess;
            }
        }
    }
}