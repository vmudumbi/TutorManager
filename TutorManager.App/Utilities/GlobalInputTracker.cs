using System;
using System.Windows.Forms;

namespace TutorManager.App.Utilities
{
    public class GlobalInputTracker : IMessageFilter
    {
        public event Action UserActivity;

        public bool PreFilterMessage(ref Message m)
        {
            // Mouse move, click, keyboard
            if (m.Msg == 0x0200 || // Mouse move
                m.Msg == 0x0201 || // Mouse click
                m.Msg == 0x0100)   // Key down
            {
                UserActivity?.Invoke();
            }

            return false;
        }
    }
}