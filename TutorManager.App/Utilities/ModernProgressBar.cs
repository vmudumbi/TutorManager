using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TutorManager.App.Utility
{
    public class ModernProgressBar : ProgressBar
    {
        public ModernProgressBar()
        {
            // Enable custom painting and double buffering to prevent flickering
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = this.ClientRectangle;
            Rectangle innerRect = new Rectangle(0, 0, rect.Width, rect.Height);

            // 1. Draw the Background (Track)
            using (GraphicsPath trackPath = GetRoundedRect(innerRect, innerRect.Height / 2))
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(235, 237, 240)))
            {
                g.FillPath(trackBrush, trackPath);
            }

            // 2. Draw the Progress Fill (Pill)
            double scale = (double)this.Value / this.Maximum;
            int fillWidth = (int)(innerRect.Width * scale);

            if (fillWidth > 5) // Only draw if there is progress
            {
                Rectangle fillRect = new Rectangle(0, 0, fillWidth, innerRect.Height);
                using (GraphicsPath fillPath = GetRoundedRect(fillRect, fillRect.Height / 2))
                {
                    // Modern Gradient: Cyan to Royal Blue
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        innerRect,
                        Color.FromArgb(0, 255, 255),
                        Color.FromArgb(0, 114, 255),
                        0.0f))
                    {
                        g.FillPath(brush, fillPath);
                    }
                }
            }
        }

        // Helper to create rounded corners
        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}