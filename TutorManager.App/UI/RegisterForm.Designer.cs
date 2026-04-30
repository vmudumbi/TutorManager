namespace TutorManager.App.UI
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle, lblUser, lblPass, lblSecret;
        private System.Windows.Forms.TextBox txtUser, txtPass, txtSecret;
        private System.Windows.Forms.Button btnRegister, btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Create New Account";
            this.Size = new System.Drawing.Size(380, 420);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            lblTitle = new System.Windows.Forms.Label()
            {
                Text = "Register Account",
                Left = 30,
                Top = 20,
                Width = 300,
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold)
            };

            // Username
            lblUser = new System.Windows.Forms.Label() { Text = "New Username", Left = 30, Top = 75, AutoSize = true };
            txtUser = new System.Windows.Forms.TextBox() { Left = 30, Top = 95, Width = 300, Font = new System.Drawing.Font("Segoe UI", 11), BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };

            // Password
            lblPass = new System.Windows.Forms.Label() { Text = "New Password", Left = 30, Top = 140, AutoSize = true };
            txtPass = new System.Windows.Forms.TextBox() { Left = 30, Top = 160, Width = 300, Font = new System.Drawing.Font("Segoe UI", 11), BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            // Secret Key (Security layer)
            lblSecret = new System.Windows.Forms.Label() { Text = "Admin Secret Key (To allow registration)", Left = 30, Top = 205, AutoSize = true, ForeColor = System.Drawing.Color.DarkRed };
            txtSecret = new System.Windows.Forms.TextBox() { Left = 30, Top = 225, Width = 300, Font = new System.Drawing.Font("Segoe UI", 11), BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            btnRegister = new System.Windows.Forms.Button()
            {
                Text = "CREATE ACCOUNT",
                Left = 30,
                Top = 285,
                Width = 300,
                Height = 40,
                BackColor = System.Drawing.Color.SeaGreen,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI Semibold", 10)
            };
            btnRegister.FlatAppearance.BorderSize = 0;

            btnCancel = new System.Windows.Forms.Button()
            {
                Text = "Cancel",
                Left = 30,
                Top = 330,
                Width = 300,
                Height = 30,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                ForeColor = System.Drawing.Color.Gray
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new System.Windows.Forms.Control[] { lblTitle, lblUser, txtUser, lblPass, txtPass, lblSecret, txtSecret, btnRegister, btnCancel });
        }
    }
}