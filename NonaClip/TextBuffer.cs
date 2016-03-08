using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonaClip
{
    public class TextBuffer
    {
        const int max_caption = 40;
        const int max_length = 1000000;
        public string[] buffers { get; set; }
        public string[] captions;
        public bool[] lock_stat { get; set; }
    
        public TextBuffer()
        {
            load();
        }

        private void load()
        {
            buffers = Enumerable.Repeat<string>("", 9).ToArray();
            captions = Enumerable.Repeat<string>("", 9).ToArray();
            lock_stat = Enumerable.Repeat<bool>(false, 9).ToArray();
        }

        private bool load_text(string file)
        {
            return true;
        }

        public static string get_short_str(string str)
        {
            //            System.Globalization.StringInfo si = new System.Globalization.StringInfo(str);
            if (str == "")
                return "";
            string ret = str.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("\b", "");
            int cc = ret.Length;
            if (ret.Length > max_caption)
            {
                ret = ret.Substring(0, max_caption) + " ...";
            }
            string exp = "(" + cc + "文字)";
            return ret + " " + exp;
        }

        public string get_caption(int num)
        {
            string ls = (num+1).ToString() + ")";
            if(lock_stat[num] == true)
            {
                ls = ls + "* ";
            }
            else
            {
                ls = ls + "  ";
            }
            return ls + captions[num];
        }

        public void set_text(string str, int num)
        {
            captions[num] = get_short_str(str);
            buffers[num] = str;
        }

        public static bool isLegal(string str)
        {
            if (str.Length == 0 || str.Length > max_length)
            {
                return false;
            }
            return true;
        }

        public void push_text(string str)
        {
            int i = find_next(0);
            string buf = str;
            while(i != -1)
            {
                buf = buffers[i];
                set_text(str, i);
                str = buf;
                i = find_next(i+1);
            }
        }

        private int find_next(int n)
        {
            for (int i = n; i < 9; i++)
            {
                if (lock_stat[i] == true)
                    continue;
                else
                    return i;
            }
            return -1;
        }
    }
}
