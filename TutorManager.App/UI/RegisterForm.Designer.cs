using System;
using System.Drawing;
using System.Windows.Forms;

namespace TutorManager.App.UI
{
    public partial class RegisterForm : Form
    {
        private Label lblTitle, lblUser, lblPass, lblSecret, lblSecretHint;
        private TextBox txtUser, txtPass, txtSecret;
        private Button btnRegister, btnCancel;

     

        private void InitializeComponent()
        {
            this.Text = "Create New Account";
            this.Size = new Size(380, 440);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int left = 30;
            int width = 300;
            int top = 20;
            int gap = 10;

            // ===== Title =====
            lblTitle = new Label()
            {
                Text = "Register Account",
                Left = left,
                Top = top,
                Width = width,
                Height = 35,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ===== Username =====
            top += 50;
            lblUser = new Label()
            {
                Text = "Username",
                Left = left,
                Top = top,
                Width = width
            };

            top += 20 + gap;
            txtUser = new TextBox()
            {
                Left = left,
                Top = top,
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            // ===== Password =====
            top += 40;
            lblPass = new Label()
            {
                Text = "Password",
                Left = left,
                Top = top,
                Width = width
            };

            top += 20 + gap;
            txtPass = new TextBox()
            {
                Left = left,
                Top = top,
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // ===== Secret Key =====
            top += 40;
            lblSecret = new Label()
            {
                Text = "Admin Secret Key",
                Left = left,
                Top = top,
                Width = width,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 60, 60)
            };

            top += 20;
            lblSecretHint = new Label()
            {
                Text = "Required to allow account creation",
                Left = left,
                Top = top,
                Width = width,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            top += 20 + gap;
            txtSecret = new TextBox()
            {
                Left = left,
                Top = top,
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // ===== Register Button =====
            top += 50;
            btnRegister = new Button()
            {
                Text = "CREATE ACCOUNT",
                Left = left,
                Top = top,
                Width = width,
                Height = 42,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10)
            };
            btnRegister.FlatAppearance.BorderSize = 0;

            // ===== Cancel Button =====
            top += 50;
            btnCancel = new Button()
            {
                Text = "Cancel",
                Left = left,
                Top = top,
                Width = width,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnCancel.Click += (s, e) => this.Close();

            // ===== Add Controls =====
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblUser, txtUser,
                lblPass, txtPass,
                lblSecret, lblSecretHint, txtSecret,
                btnRegister,
                btnCancel
            });
        }
    }
}