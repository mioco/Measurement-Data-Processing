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

namespace MDP
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        #region 变量定义

        #endregion

        public Window1()
        {
            InitializeComponent();
        }

        // 读取点文件
        private void readPoint(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile("读取点文件");
            pointPath.Text = readFile.getPath();
            
            readFile.getPoints().ForEach(delegate (PointClass P)
            {
                LB01.CurrentPoints.Add(P);
                string temp = P.PID.ToString() + ", " + P.H.ToString() + "(m), " + P.IsControlP.ToString() + ": " + P.IsH0.ToString() + ';' + '\n';
                pointContent.Items.Add(temp);
            });

        }

        // 读取边文件
        private void readEdge(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile("读取边文件");
            edgePath.Text = readFile.getPath();
            int ii = 0;

            readFile.getLines().ForEach(delegate (LineClass L)
            {
                LB01.CurrentSegments.Add(L);
                string temp = ii.ToString() + ":  " + L.SP.PID.ToString() + ",  " + L.EP.PID.ToString() + ",  " + L.dH.ToString() + "m,  " + L.Distance.ToString() + "km;" + '\n';
                edgeContent.Items.Add(temp);
                ii++;
            });
        }

        private void confirm(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
