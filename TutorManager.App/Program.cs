using TutorManager.App.Data;
using TutorManager.App.UI;

namespace TutorManager.App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {   
            Db.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // Only run MainForm if login returned OK
                    Application.Run(new MainForm());
                }
                else
                {
                    // Otherwise, the app just ends here
                    Application.Exit();
                }
            }
        }
    }
}