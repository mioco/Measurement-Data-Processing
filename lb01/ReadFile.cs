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

        OpenFileDialog f = new OpenFileDialog();

        #region 读取文件
        public OpenFileDialog readFile()
        {
            f.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            f.RestoreDirectory = true;
            return f;
        }
        #endregion

        #region 获取数据内容
        public List<string> getList(List<T>, OpenFileDialog f)
        {
            List<string> lines = new List<string>();
            
            string path = f.FileName;
            using (StreamReader r = new StreamReader(f.OpenFile()))
            {
                lines = r.ReadLine().Split(' ').ToList<string>();
                lines.RemoveAll(String.IsNullOrEmpty);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        //数据处理
        private void handleData()
        {
            ControlPoints.Add(new LPointClass
            {
                PID = TempString[0],
                H = double.Parse(TempString[1]),
                IsControlP = true,
                IsH0 = true
            });
        }
        #endregion

        #region 获取数据路径
        public string getPath(OpenFileDialog f)
        {
            if (f.ShowDialog() == DialogResult.OK)
            {
                return f.FileName;
            }
            return "error";
        }
        #endregion
    }
}
