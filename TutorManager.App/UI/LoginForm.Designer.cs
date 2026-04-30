namespace TutorManager.App.UI
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.LinkLabel lnkRegister;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Tutor Manager - Login";
            this.Size = new System.Drawing.Size(380, 340);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            lblTitle = new System.Windows.Forms.Label()
            {   
                Left = 30,
                Top = 20,
                Width = 300,
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold)
            };

            lblUser = new System.Windows.Forms.Label() { Text = "Username", Left = 30, Top = 70, AutoSize = true };
            txtUser = new System.Windows.Forms.TextBox()
            {
                Left = 30,
                Top = 90,
                Width = 300,
                Font = new System.Drawing.Font("Segoe UI", 11),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            };

            lblPass = new System.Windows.Forms.Label() { Text = "Password", Left = 30, Top = 130, AutoSize = true };
            txtPass = new System.Windows.Forms.TextBox()
            {
                Left = 30,
                Top = 150,
                Width = 300,
                Font = new System.Drawing.Font("Segoe UI", 11),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            btnLogin = new System.Windows.Forms.Button()
            {
                Text = "LOGIN",
                Left = 30,
                Top = 200,
                Width = 300,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI Semibold", 10)
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            lnkRegister = new System.Windows.Forms.LinkLabel()
            {
                Text = "New user? Create an account",
                Left = 30,
                Top = 255,
                Width = 300,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                LinkColor = System.Drawing.Color.Gray
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUser);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lnkRegister);

            this.AcceptButton = btnLogin;
        }
    }
}