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
    /// LB02.xaml 的交互逻辑
    /// </summary>
    public partial class LB02 : Window
    {
        public LB02()
        {
            InitializeComponent();
        }
        
        PingCha_Class CPlaneNetAdjust1 = new PingCha_Class();
        List<PointClass> ControlPoints = new List<PointClass>();//控制点集合
        List<StationClass> StationInfos = new List<StationClass>();//测站观测数据
        List<PointClass> StartPoints = new List<PointClass>();//控制点集合
        // 控制点文件
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile();
            ControlPoints = CPlaneNetAdjust1.InputCtrData(readFile.getPath());
            this.dataGridView3.DataContext = null;
            this.dataGridView3.DataContext = ControlPoints;
        }
        // 观测数据
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
        // 测边数据
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }
        // 退出
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {

        }
        // 提取所有点
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {

        }
        // 提取未知点
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {

        }
        // 初始值计算
        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {

        }
        // 角误差方程
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {

        }
        // 边误差方程
        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {

        }
        // 合并误差方程
        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {

        }
        // 平差计算
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {

        }
        // 一步解算
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {

        }
    }
}
