using System;
using System.Drawing;
using System.Windows.Forms;

namespace TutorManager.App.Services
{
    public static class LoginService
    {
        // This variable stays true as long as the app is running
        private static bool _isLoggedIn = false;

        public static bool Authenticate()
        {
            // If already logged in, don't show the box again!
            if (_isLoggedIn) return true;

            using (Form loginForm = new Form())
            {
                loginForm.Text = "Security Check";
                loginForm.Size = new Size(350, 240);
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                loginForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                loginForm.MaximizeBox = false;
                loginForm.MinimizeBox = false;
                loginForm.BackColor = Color.White;

                Label lblMsg = new Label()
                {
                    Text = "Administrative access required.",
                    Left = 20,
                    Top = 15,
                    Width = 300,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(45, 45, 48)
                };

                Label lblUser = new Label() { Text = "Username", Left = 20, Top = 55, AutoSize = true, Font = new Font("Segoe UI", 9) };
                TextBox txtUser = new TextBox() { Left = 20, Top = 75, Width = 290, Height = 30, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };

                Label lblPass = new Label() { Text = "Password", Left = 20, Top = 110, AutoSize = true, Font = new Font("Segoe UI", 9) };
                TextBox txtPass = new TextBox() { Left = 20, Top = 130, Width = 290, Height = 30, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };

                Button btnLogin = new Button()
                {
                    Text = "Login",
                    Left = 210,
                    Top = 165,
                    Width = 100,
                    Height = 35,
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10),
                    DialogResult = DialogResult.OK
                };
                btnLogin.FlatAppearance.BorderSize = 0;

                loginForm.Controls.AddRange(new Control[] { lblMsg, lblUser, txtUser, lblPass, txtPass, btnLogin });
                loginForm.AcceptButton = btnLogin;

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    if (txtUser.Text == "venkat" && txtPass.Text == "venkatreport")
                    {
                        _isLoggedIn = true;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                }
                return false;
            }
        }

        // Optional: Call this if you want a "Logout" button somewhere
        public static void Logout() => _isLoggedIn = false;
    }
}