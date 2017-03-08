using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

namespace Demo
{
    public partial class ScreenMaker : Form
    {
        private IKeyboardMouseEvents m_GlobalHook;
        private ScreenSaver _screenSaver;

        public ScreenMaker()
        {
            InitializeComponent();
            //TransparencyKey = BackColor;
            ShowIcon = false;
            ShowInTaskbar = false;

            WindowState = FormWindowState.Minimized;

            notifyIcon1.DoubleClick += NotifyIcon1_Click;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "ScreenHelper is started";
            notifyIcon1.BalloonTipTitle = "ScreenHelper";
            notifyIcon1.ShowBalloonTip(5);

            Subscribe();
            Closing += Form1_Closing;
        }

        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            //WindowState = FormWindowState.Normal;
        }

        public void SetScreen(Bitmap bmp)
        {
            UploadImage(bmp);
        }

        private async void UploadImage(Bitmap bmp)
        {
            try
            {
                var client = new ImgurClient("06906c0abc249ee", "6ca80496554f4ef717a37f0651202b35ba9681ec");
                var endpoint = new ImageEndpoint(client);
                IImage image;
                using (var fs = new MemoryStream())
                {
                    bmp.Save(fs,ImageFormat.Bmp);
                    fs.Position = 0;
                    image = await endpoint.UploadImageStreamAsync(fs);
                }
                Clipboard.SetText(image.Link);
                //Debug.Write("Image uploaded. Image Url: " + image.Link);
            }
            catch (ImgurException imgurEx)
            {
                notifyIcon1.BalloonTipText = @"An error occurred uploading an image to Imgur.";
                notifyIcon1.ShowBalloonTip(3);
                //Debug.Write(imgurEx.Message);
            }
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += M_GlobalHook_KeyDown;
        }

        private void M_GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.PrintScreen) 
            {
                _screenSaver = new ScreenSaver();
                _screenSaver.Owner = this;
                _screenSaver.ShowDialog();
                _screenSaver.Activate();
            }
            if (e.KeyCode==Keys.Escape) _screenSaver?.Dispose();
        }

       public void Unsubscribe()
        {
            m_GlobalHook.KeyDown -= M_GlobalHook_KeyDown;
            m_GlobalHook.Dispose();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
