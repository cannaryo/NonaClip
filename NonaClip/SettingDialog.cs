using System;
using System.Collections;
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
    public partial class SettingDialog : Form
    {
        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);
        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr HWnd, int ID);

        const string KEYSET_D = "1-9";
        const string KEYSET_N = "NumPad1-NumPad9";

        private Form1 parent_form;
        private SettingParams prms;
        private Keys key_code1 = Keys.C;
        private Keys key_code2 = Keys.D0;
        private Hashtable mod_table = new Hashtable();

        public SettingDialog()
        {
            InitializeComponent();
            InitializeTable();
        }

        private void InitializeTable()
        {
            mod_table[SettingParams.MOD_CONTROL] = "Ctrl";
            mod_table[SettingParams.MOD_CONTROL | SettingParams.MOD_SHIFT] = "Ctrl+Shift";
            mod_table[SettingParams.MOD_CONTROL | SettingParams.MOD_ALT] = "Ctrl+Alt";
            mod_table[SettingParams.MOD_ALT | SettingParams.MOD_SHIFT] = "Alt+Shift";
        }

        public void attach_object(Form1 fm, SettingParams pm)
        {
            parent_form = fm;
            prms = pm;
        }

        public void set_params()
        {
            key_code1 = (Keys)prms.copy_key;
            key_code2 = (Keys)prms.show_key;
            textBox1.Text = key_code1.ToString();
            textBox2.Text = key_code2.ToString();
            comboBox1.Text = (string)mod_table[prms.mod_type];
            if(prms.keyset[0] == prms.keyset_n[0])
            {
                comboBox2.Text = KEYSET_N;
            }
            else
            {
                comboBox2.Text = KEYSET_D;
            }
            checkBox1.Checked = prms.top_most;
            checkBox2.Checked = prms.text_record;
            checkBox3.Checked = prms.invisible_first;
        }

        public void make_file_list(string path)
        {
            string[] files = System.IO.Directory.GetFiles(path, "*.png");
            for(int i=0; i < files.Length; i++)
            {
                files[i] = System.IO.Path.GetFileName(files[i]);
            }
            listBox1.Items.Clear();
            listBox1.Items.AddRange(files);
            listBox1.Text = prms.background_file;
        }

        private bool check_conflicts()
        {
            if (key_code1 == Keys.Insert || key_code2 == Keys.Insert)
            {
                MessageBox.Show("Insert Key is not permitted");
                return false;                 
            }
            if (comboBox2.Text == KEYSET_N)
            {
                foreach(int k in prms.keyset_n)
                {
                    if((int)key_code1 == k || (int)key_code2 == k)
                    {
                        MessageBox.Show("Hotkeys conflict");
                        return false;
                    }
                }
            }
            else
            {
                foreach (int k in prms.keyset_d)
                {
                    if ((int)key_code1 == k || (int)key_code2 == k)
                    {
                        MessageBox.Show("Hotkeys conflict");
                        return false;
                    }
                }
            }
            if(key_code1 == key_code2)
            {
                MessageBox.Show("Hotkeys conflict");
                return false;
            }
            return true;
        }

        private void accept_change()
        {
            prms.copy_key = (int)key_code1;
            prms.show_key = (int)key_code2;
            foreach(int i in mod_table.Keys)
            {
                if(comboBox1.Text == (string)mod_table[i])
                {
                    prms.mod_type = i;
                    break;
                }
            }

            if (comboBox2.Text == KEYSET_N)
            {
                prms.keyset = prms.keyset_n;
            }
            else
            {
                prms.keyset = prms.keyset_d;
            }
            prms.top_most = checkBox1.Checked;
            prms.text_record = checkBox2.Checked;
            prms.invisible_first = checkBox3.Checked;
            prms.background_file = listBox1.Text;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            if (check_conflicts())
            {
                accept_change();
                parent_form.save_param_file();
                parent_form.hotkey_unregist_all();
                parent_form.hotkey_regist_all();
                this.Close();
                parent_form.reload_graphics();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox var = (TextBox)sender;
            key_code1 = e.KeyCode;
            var.Text = e.KeyCode.ToString();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            TextBox var = (TextBox)sender;
            var.Text = "=Press any key=";
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            TextBox var = (TextBox)sender;
            if (var.Text == "=Press any key=")
                var.Text = key_code1.ToString();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox var = (TextBox)sender;
            key_code2 = e.KeyCode;
            var.Text = e.KeyCode.ToString();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            TextBox var = (TextBox)sender;
            if (var.Text == "=Press any key=")
                var.Text = key_code2.ToString();
        }

        private void button_default_Click(object sender, EventArgs e)
        {
            prms.load_default_param();
            parent_form.save_param_file();
            parent_form.hotkey_unregist_all();
            parent_form.hotkey_regist_all();
            this.Close();
            parent_form.reload_graphics();
        }
    }
}
