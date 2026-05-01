using System;
using System.Windows.Forms;
using TutorManager.App.Data;
using TutorManager.App.Utility;

namespace TutorManager.App.UI
{
    public partial class LoginForm : Form
    {
        private UserRepository _userRepo = new UserRepository();
        public bool IsAuthenticated { get; private set; } = false;

        public LoginForm()
        {
            InitializeComponent();

            btnLogin.Click += BtnLogin_Click;
            lnkRegister.LinkClicked += LnkRegister_LinkClicked;
        }


        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            // Call our new Progress Utility
            bool loginSuccessful = await UIStyleHelper.ExecuteWithProgress("Connecting to Tutor Manager...", async () =>
            {
                // This runs your database check on a background thread
                return await Task.Run(() => _userRepo.Validate(user, pass));
            });

            if (loginSuccessful)
            {
                var isAdmin = user == "admin" ? true : false;

                MainForm main = new MainForm(isAdmin);
                this.Hide();
                main.ShowDialog();
                this.Close();

            }
            else
            {
                // The progress bar already showed "Failed!", 
                // but you can add a small shake effect or a shake-label here if you like.
                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // We will create RegisterForm next
            using (var regForm = new RegisterForm())
            {
                if (regForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Registration successful! You can now login.");
                }
            }
        }
    }
}