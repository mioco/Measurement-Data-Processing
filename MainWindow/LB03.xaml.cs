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
    /// LB03.xaml 的交互逻辑
    /// </summary>
    public partial class LB03 : Window
    {
        public LB03()
        {
            InitializeComponent();
        }

        CoordtoTrans MyCoordtoTrans = new CoordtoTrans();
        List<PointClass> SourcePoints = new List<PointClass>();
        List<PointClass> TargetPoints = new List<PointClass>();
        List<PointClass> CommonP = new List<PointClass>();
        List<PointClass> IIDimTraPoints = new List<PointClass>();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReadFile f = new ReadFile("原坐标文件");
            MyCoordtoTrans.InputData(f.getPath(), SourcePoints);
            Tool_Class.addData<PointClass>(dataGrid1, SourcePoints);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ReadFile f = new ReadFile("目标坐标文件");
            MyCoordtoTrans.InputData(f.getPath(), TargetPoints);
            Tool_Class.addData<PointClass>(dataGrid2, TargetPoints);
        }

        // 退出
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        // 二维坐标转换
        private void _2DCoordinateTransformation(object sender, RoutedEventArgs e)
        {
            CommonP = MyCoordtoTrans.IICoordTans(SourcePoints, TargetPoints, IIDimTraPoints);
            MyCoordtoTrans.IICoordTans(SourcePoints, TargetPoints, IIDimTraPoints);
            Tool_Class.addData<PointClass>(dataGrid3, CommonP);
            Tool_Class.addData<PointClass>(dataGrid4, IIDimTraPoints);
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
