using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDP
{
    class ReadFile
    {

        private OpenFileDialog f;
        private string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;

        // 构造函数初始化设置
        public ReadFile()
        {
            f = new OpenFileDialog();
            f.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            f.RestoreDirectory = true;
        }
        
        #region 获取数据内容
        public List<LPointClass> getPoints()
        {
            List<string> TempString = new List<string>();
            List<LPointClass> container = new List<LPointClass>();

            using (StreamReader r = new StreamReader(f.OpenFile()))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    // 处理每行数据，去空
                    TempString = handleLine(line);

                    // 创建构造函数类
                    container.Add(new LPointClass
                    {
                        PID = TempString[0],
                        H = double.Parse(TempString[1]),
                        IsControlP = true,
                        IsH0 = true
                    });
                }
            }

            return container;
        }

        public List<LineClass> getLines()
        {
            List<string> TempString = new List<string>();
            List<LineClass> container = new List<LineClass>();

            using (StreamReader r = new StreamReader(f.OpenFile()))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    // 处理每行数据，去空
                    TempString = handleLine(line);
                    
                    container.Add(new LineClass
                    {
                        SP = new LPointClass { PID = TempString[0] },
                        EP = new LPointClass { PID = TempString[1] },
                        dH = double.Parse(TempString[2]),
                        Distance = double.Parse(TempString[3]),
                    });
                }
            }

            return container;
        }

        private List<string> handleLine(string line)
        {
            List<string> TempString = new List<string>();
            TempString = line.Split(' ').ToList<string>();
            TempString.RemoveAll(String.IsNullOrEmpty);
            return TempString;
        }

        #endregion

        #region 获取数据路径
        public string getPath()
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
