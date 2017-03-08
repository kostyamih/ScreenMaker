using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class ScreenSaver : Form
    {

        private int _mousex, _mousey;
        private bool _flag;
        private Graphics _graphics;

        public ScreenSaver()
        {
            InitializeComponent();
            ShowIcon = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            TransparencyKey = BackColor;
            pictureBox1.BackColor = BackColor;

            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            var bmp = SaveScreen(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            pictureBox1.Image = bmp;

            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.Cursor = Cursors.Cross;

            pictureBox1.Focus();

            _graphics = pictureBox1.CreateGraphics();
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_flag)
            {

                _graphics.Clear(BackColor);
                int hight = e.Y - _mousey;
                int width = e.X - _mousex;
                int x = _mousex;
                int y = _mousey;
                if (width < 0) { width *= -1; x = e.X;}
                if (hight < 0) { hight *= -1; y = e.Y;}

                _graphics.DrawRectangle(new Pen(Brushes.Red), x, y, width, hight);
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _flag = false;
            ScreenMaker main = Owner as ScreenMaker;
            _graphics.Clear(BackColor);
            main?.SetScreen(SaveScreen(_mousex, _mousey, e.X - _mousex, e.Y - _mousey));
            Dispose();
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _mousex = e.X;
            _mousey = e.Y;
            _flag = true;
        }

        private Bitmap SaveScreen(int x, int y, int width, int hight)
        {
            if (width < 0) width *= -1;
            if (hight < 0) hight *= -1;          
            Graphics graph = null;
            var bmp = new Bitmap(width, hight);
            graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(x, y, 0, 0, bmp.Size);
            return bmp;
        }

    }
}
