using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NonaClip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private SettingDialog dialog = new SettingDialog();
        //        private const string file_path = @"C:\Users\kanno\Documents\Visual Studio 2015\Projects\NonaClip\resources\wafuu2_cat.png";
        private const string resource_path = @"resources\";
        private const string conf_file = @"nonaclip_settings.xml";
        private Point lastMousePosition;
        private bool mouseCapture;
        private TextBuffer txbuf;
        private SettingParams prms;
        private Label[] labels;

        const int MOD_ALT = 0x0001;
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int MOD_WIN = 0x0008;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_HOTKEY = 0x0312;
        const int WM_COPY = 0x0301;
        const int WM_PASTE = 0x0302;
        const int WM_CLIPBOARDUPDATE = 0x031D;
//        const int WM_DRAWCLIPBOARD = 0x0308;
        const int WM_SETTEXT = 0x000c;
        const int KEYEVENTF_KEYDOWN = 0x0; 
        const int KEYEVENTF_KEYUP = 0x2;
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int VK_SHIFT = 0x10;
        const int VK_CONTROL = 0x11;
        const int VK_MENU = 0x12;
        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr HWnd, int ID);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr HWnd, uint uCmd);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetGUIThreadInfo(uint idThread, ref GuiThreadInfo lpgui);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void RemoveClipboardFormatListener(IntPtr hwnd);

        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GuiThreadInfo
        {
            public int cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public Rect rcCaret;
        }


        private void Item2_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            // Load parameters.
            LoadParamFile();
            // Load text buffer
            LoadTextBuffer();
            // Regist Hot keys.
            HotkeyRegisterAll();
            // Show graphical frame
            string file_path = resource_path + prms.background_file;
            ShowGraphics(file_path);
            labels = new Label[] { label1, label2, label3, label4, label5, label6, label7, label8, label9 };
            foreach (Label label in  labels )
            {
                label.BackColor = System.Drawing.Color.Transparent;
            }
            Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width,0);
            RefreshLabel();
            // start Clipboard watch 
            AddClipboardFormatListener(Handle);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveClipboardFormatListener(Handle);
            notifyIcon1.Visible = false;
            HotkeyUnregisterAll();
            SaveTextBuffer();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                for (int i = 0; i < 9; i++)
                {
                    if ((int)m.WParam == prms.hotkey_ids[i])
                    {
                        SendPaste(i);
                    }
                }
                if ((int)m.WParam == prms.hotkey_show)
                {
                    if (this.Visible)
                    {
                        this.Visible = false;
                        this.TopMost = false;
                    }
                    else
                    {
                        this.Visible = true;
                        this.TopMost = prms.top_most;
                    }
                }
            }
            else if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardUpdate();
            }
        }

        private void LoadParamFile()
        {
            string file = conf_file;
            if (System.IO.File.Exists(file))
            {
                try {
                    System.Xml.Serialization.XmlSerializer xml_sr = new System.Xml.Serialization.XmlSerializer(typeof(SettingParams));
                    System.IO.StreamReader sr = new System.IO.StreamReader(file, new System.Text.UTF8Encoding(false));
                    prms = (SettingParams)xml_sr.Deserialize(sr);
                    sr.Close();
                }
                catch
                {
                    prms = new SettingParams();
                    prms.load_default_param();
                    return;
                }

            }
            else
            {
                prms = new SettingParams();
                prms.load_default_param();
            }
        }

        public void SaveParamFile()
        {
            string file = conf_file;
            try
            {
                System.Xml.Serialization.XmlSerializer xml_sr = new System.Xml.Serialization.XmlSerializer(typeof(SettingParams));
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false, new System.Text.UTF8Encoding(false));
                xml_sr.Serialize(sw, prms);
                sw.Close();
            }
            catch
            {
                return;
            }
        }

        private void LoadTextBuffer()
        {
            if(prms.text_record == false)
            {
                txbuf = new TextBuffer();
                return;
            }
            string file = prms.record_file;
            if(System.IO.File.Exists(file))
            {
                try {
                    System.Xml.Serialization.XmlSerializer xml_sr = new System.Xml.Serialization.XmlSerializer(typeof(TextBuffer));
                    System.IO.StreamReader sr = new System.IO.StreamReader(file, new System.Text.UTF8Encoding(false));
                    txbuf = (TextBuffer)xml_sr.Deserialize(sr);
                    sr.Close();
                }
                catch
                {
                    txbuf = new TextBuffer();
                }
            }
            else
            {
                txbuf = new TextBuffer();
            }
        }

        private void SaveTextBuffer()
        {
            if (prms.text_record == false)
            {
                return;
            }
            string file = prms.record_file;
            try
            {
                System.Xml.Serialization.XmlSerializer xml_sr = new System.Xml.Serialization.XmlSerializer(typeof(TextBuffer));
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false, new System.Text.UTF8Encoding(false));
                xml_sr.Serialize(sw, txbuf);
                sw.Close();
            }
            catch
            {
                return;
            }
        }

        public void ReloadGraphics()
        {
            string file_path = resource_path + prms.background_file;
            ShowGraphics(file_path);
            if (this.Visible)
            {
                this.TopMost = prms.top_most;
            }
            RefreshLabel();
        }

        public void HotkeyRegisterAll()
        {
            RegisterHotKey(Handle, prms.hotkey_show, prms.mod_type, prms.show_key);
            for(int i=0; i<9; i++)
            {
                RegisterHotKey(Handle, prms.hotkey_ids[i], prms.mod_type, prms.keyset[i]);
            }
        }

        public void HotkeyUnregisterAll()
        {
            UnregisterHotKey(Handle, prms.hotkey_show);
            for (int i = 0; i < 9; i++)
            {
                UnregisterHotKey(Handle, prms.hotkey_ids[i]);
            }
        }

        private void SendPaste(int n)
        {
            string text = txbuf.buffers[n];
            if (text != "")
            {
                RemoveClipboardFormatListener(Handle);
                Clipboard.SetText(text);
                if(prms.sendkey_type == SettingParams.TYPE_INS)
                {
                    if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0x0000)
                    {
                        SendKeys.SendWait("^+%{A 0}");
                    }
                    SendKeys.SendWait("+{INSERT}");
                }
                else if(prms.sendkey_type == SettingParams.TYPE_V)
                {
                    if ((Control.ModifierKeys & (Keys.Shift | Keys.Alt)) != 0x0000)
                    {
                        SendKeys.SendWait("^+%{A 0}");
                    }
                    SendKeys.SendWait("^v");
                }
                AddClipboardFormatListener(Handle);
            }
        }

        private void SendCopy()
        {
            if((Control.ModifierKeys & (Keys.Shift | Keys.Alt)) != 0x0000)
            {
                SendKeys.SendWait("^+%{A 0}");
            }
            SendKeys.SendWait("^{INSERT}");
            System.Threading.Thread.Sleep(50);
            SetText();
            RefreshLabel();
        }

        private void OnClipboardUpdate()
        {
            SetText();
            RefreshLabel();
        }

        private void SetText()
        {
            if (Clipboard.ContainsText())
            {
                string str = Clipboard.GetText();
                if (TextBuffer.isLegal(str))
                {
                    txbuf.push_text(str);
                }
            }
        }

        private void RefreshLabel()
        {
            for(int i=0; i<9; i++)
            {
                labels[i].Text = txbuf.get_caption(i);
            }
        }

        private IntPtr GetFocusWindow()
        {
            IntPtr hWnd = GetForegroundWindow();
            IntPtr handle;
            if (hWnd == this.Handle)
            {
                handle = this.ActiveControl.Handle;
            }
            else
            {
                uint threadId = GetWindowThreadProcessId(hWnd, IntPtr.Zero);
                GuiThreadInfo gti = new GuiThreadInfo();
                gti.cbSize = Marshal.SizeOf(gti);
                bool GUIinfo = GetGUIThreadInfo(threadId, ref gti);
                handle = gti.hwndFocus;
            }
            return handle;
        }

        private void Item1_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dialog.Visible == false)
            {
                dialog.AttachObject(this, prms);
                dialog.SetParam();
                dialog.MakeFileList(resource_path);
                dialog.ShowDialog();
            }
        }

        private void ShowGraphics(string path)
        {
            //フォームの境界線をなくす
            FormBorderStyle = FormBorderStyle.None;
            if (System.IO.File.Exists(path))
            {
                try
                {
                    //フォームのサイズ変更
                    SizeChange(path);
                    //背景画像を指定する
                    Bitmap img = new Bitmap(path);
                    img.MakeTransparent();
                    this.BackgroundImage = img;
                    //背景を透明に
                    this.TransparencyKey = this.BackColor;
                }
                catch
                {
                    ;
                }
            }
        }

        //ウィンドウの大きさを画像の大きさに変更
        private void SizeChange(string path)
        {
            //元画像の縦横サイズを調べる
            System.Drawing.Bitmap bmpSrc = new System.Drawing.Bitmap(@path);
            int width = bmpSrc.Width;
            int height = bmpSrc.Height;
            //ウィンドウのサイズを変更
            this.Size = new Size(width, height);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //マウスの位置を所得
            this.lastMousePosition = Control.MousePosition;
            this.mouseCapture = true;
        }

        private void Form1_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (this.mouseCapture == true && this.Capture == false)
            {
                this.mouseCapture = false;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.mouseCapture = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseCapture == false)
            {
                return;
            }
            // 最新のマウスの位置を取得
            Point mp = Control.MousePosition;
            // 差分を取得
            int offsetX = mp.X - this.lastMousePosition.X;
            int offsetY = mp.Y - this.lastMousePosition.Y;
            // コントロールを移動
            this.Location = new Point(this.Left + offsetX, this.Top + offsetY);
            this.lastMousePosition = mp;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            int i;
            for (i = 0; i < 9; i++)
            {
                if (lb.TabIndex == labels[i].TabIndex)
                    break;
            }
            txbuf.lock_stat[i] = !txbuf.lock_stat[i];
            labels[i].Text = txbuf.get_caption(i);
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            lb.ForeColor = Color.SkyBlue;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            Label lb = (Label)sender;
            lb.ForeColor = SystemColors.HighlightText;
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Label lb = (Label)sender;
            int i;
            for (i = 0; i < 9; i++)
            {
                if (lb.TabIndex == labels[i].TabIndex)
                    break;
            }
            Clipboard.SetText(txbuf.buffers[i]);
            MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                txbuf.set_text("", i);
            }
            RefreshLabel();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (prms.invisible_first)
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;
                this.TopMost = prms.top_most;
            }
        }
    }
}
