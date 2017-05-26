using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lb01
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        #region 变量定义
        public static List<LPointClass> ControlPoints = new List<LPointClass>();
        public static List<LPointClass> CurrentPoints = new List<LPointClass>();
        public static List<LineClass> CurrentSegments = new List<LineClass>();
        public double derta;//单位全中误差

        string[] al; //定义一个字符串数组
        #endregion

        public Window1()
        {
            InitializeComponent();
        }

        //读取点文件
        private void readPoint(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile();
            pointPath.Text = readFile.getPath(readFile.readFile());
            addData(readFile.getList(readFile.readFile()), pointContent);
        }

        //读取边文件
        private void readEdge(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile();
            edgePath.Text = readFile.getPath(readFile.readFile());
            addData(readFile.getList(readFile.readFile()), edgeContent);
        }

        //数据填充
        private void addData(List<string> data, ListBox container)
        {
            data.ForEach(delegate (string d)
            {
                container.Items.Add(d);
            });
        }
    }
}
