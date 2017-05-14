using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lb01
{
    class ReadFile
    {
        public List<string> readFile()
        {
            List<string> lines = new List<string>();
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            f.RestoreDirectory = true;

            if (f.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader r = new StreamReader(f.OpenFile()))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines;
        }

        private void handleLine (string line)
        {

        }
    }
}
