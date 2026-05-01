using System;
using System.Configuration;
using System.Windows.Forms;
using TutorManager.App.Data;

namespace TutorManager.App.UI
{
    public partial class RegisterForm : Form
    {
        private UserRepository _userRepo = new UserRepository();

        public RegisterForm()
        {
            InitializeComponent();

            btnRegister.Click += BtnRegister_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;
            string secret = txtSecret.Text;

            var adminregkey = ConfigurationManager.AppSettings["ADMIN_REGISTRATION_KEY"];

            // 1. Basic Validation
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Check Secret Key
            if (secret != adminregkey)
            {
                MessageBox.Show("Invalid Admin Secret Key. Registration denied.", "Security", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // 3. Attempt to Save to Database
            if (_userRepo.Add(user, pass,"staff"))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Could not create account. The username might already exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}