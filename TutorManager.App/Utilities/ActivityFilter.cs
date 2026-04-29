using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorManager.App.Utilities
{
    public class ActivityFilter : IMessageFilter
    {
        private readonly Action _onActivity;

        // Windows Message Constants
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_MOUSEWHEEL = 0x020A;

        public ActivityFilter(Action onActivity)
        {
            _onActivity = onActivity;
        }

        public bool PreFilterMessage(ref Message m)
        {
            // Reset timer if mouse moves, clicks, scrolls, or key is pressed
            if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_LBUTTONDOWN ||
                m.Msg == WM_KEYDOWN || m.Msg == WM_MOUSEWHEEL)
            {
                _onActivity();
            }
            return false; // Let the message continue to the controls
        }
    }
}
