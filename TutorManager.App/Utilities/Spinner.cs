using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TutorManager.App.Utilities
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class Spinner : Control
    {
        private Timer timer;
        private int angle = 0;

        public Spinner()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(60, 60);

            timer = new Timer();
            timer.Interval = 20; // smooth rotation
            timer.Tick += (s, e) =>
            {
                angle += 6;
                if (angle >= 360) angle = 0;
                this.Invalidate();
            };

            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int lineCount = 12;
            int radius = 20;

            for (int i = 0; i < lineCount; i++)
            {
                int alpha = (int)(255.0 * (i + 1) / lineCount);
                using (Pen pen = new Pen(Color.FromArgb(alpha, 70, 130, 180), 3))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    double angleOffset = (Math.PI * 2 * i / lineCount) + (angle * Math.PI / 180);

                    int x1 = this.Width / 2 + (int)(radius * Math.Cos(angleOffset));
                    int y1 = this.Height / 2 + (int)(radius * Math.Sin(angleOffset));

                    int x2 = this.Width / 2 + (int)((radius - 10) * Math.Cos(angleOffset));
                    int y2 = this.Height / 2 + (int)((radius - 10) * Math.Sin(angleOffset));

                    e.Graphics.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }
    }
}
