using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NonaClip
{
    public class SettingParams
    {
        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;
        public const int MOD_WIN = 0x0008;
        public const int TYPE_INS = 0;
        public const int TYPE_V = 1;
        public int[] keyset_d = new int[] { (int)Keys.D1, (int)Keys.D2, (int)Keys.D3, (int)Keys.D4, (int)Keys.D5,
                (int)Keys.D6,(int)Keys.D7,(int)Keys.D8,(int)Keys.D9,};
        public int[] keyset_n = new int[] { (int)Keys.NumPad1, (int)Keys.NumPad2, (int)Keys.NumPad3, (int)Keys.NumPad4, (int)Keys.NumPad5, 
            (int)Keys.NumPad6, (int)Keys.NumPad7, (int)Keys.NumPad8, (int)Keys.NumPad9 };

        public int[] hotkey_ids = new int[] { 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009 };
        public int hotkey_show = 0x0011;

        public int[] keyset;
        public int show_key;
        public int mod_type;
        public bool top_most;
        public bool invisible_first;
        public bool text_record;
        public string record_file;
        public string background_file;
        public int sendkey_type;

        public void load_default_param()
        {
            keyset = keyset_d;
            mod_type = MOD_CONTROL;
            top_most = true;
            invisible_first = false;
            text_record = true;
            record_file = "record.xml";
            background_file = "default.png";
            show_key = (int)Keys.D0;
            sendkey_type = TYPE_INS;
        }
    }
}
