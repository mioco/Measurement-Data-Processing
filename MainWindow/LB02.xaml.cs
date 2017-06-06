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
            ReadFile readFile = new ReadFile("读入控制点文件");
            ControlPoints = CPlaneNetAdjust1.InputCtrData(readFile.getPath());
            Tool_Class.addData<PointClass>(dataGridView3, ControlPoints);
        }
        // 观测数据
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ReadFile readFile = new ReadFile("读入测站信息");
            StationInfos = CPlaneNetAdjust1.InputObsData(readFile.getPath());
            Tool_Class.addData<StationClass>(dataGridView1, StationInfos);
            for (int i = 0; i < dataGridView1.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)dataGridView1.ItemContainerGenerator.ContainerFromIndex(i);
                Console.WriteLine(row);
                
            }
        }

        // 退出
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // 提取所有点
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Tool_Class.addData<PointClass>(dataGrideView2, CPlaneNetAdjust1.Calc_StartPoints());
        }
        // 提取未知点
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Tool_Class.addData<PointClass>(dataGridView5, CPlaneNetAdjust1.Calc_UnknowPoints());
        }
        // 初始值计算
        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.X0Y0Calculate();
            Tool_Class.addData<PointClass>(dataGridView4, CPlaneNetAdjust1.UnknowPoints);
        }
        // 角误差方程
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.AngleErrorEquations();
            int row = CPlaneNetAdjust1.Ba.Detail.GetLength(0);
            int col = CPlaneNetAdjust1.Ba.Detail.GetLength(1);
            for (int i = 0; i < row; i++)
            {
                for (int k = 0; k < col; k++)
                {
                    listBox1.Items.Add(CPlaneNetAdjust1.Ba.Detail[i, k].ToString());
                }
            }
            int Lrow = CPlaneNetAdjust1.La.Detail.GetLength(0);
            int Lcol = CPlaneNetAdjust1.La.Detail.GetLength(1);
            for (int i = 0; i < Lrow; i++)
            {
                for (int k = 0; k < Lcol; k++)
                {
                    listBox2.Items.Add(CPlaneNetAdjust1.La.Detail[i, k].ToString());
                }
            }
        }
        // 边误差方程
        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.DistErrorEquations();
            int row = CPlaneNetAdjust1.Bd.Detail.GetLength(0);
            int col = CPlaneNetAdjust1.Bd.Detail.GetLength(1);
            for (int i = 0; i < row; i++)
            {
                for (int k = 0; k < col; k++)
                {
                    listBox3.Items.Add(CPlaneNetAdjust1.Bd.Detail[i, k].ToString());
                }
            }
            int Lrow = CPlaneNetAdjust1.Ld.Detail.GetLength(0);
            int Lcol = CPlaneNetAdjust1.Ld.Detail.GetLength(1);
            for (int i = 0; i < Lrow; i++)
            {
                for (int k = 0; k < Lcol; k++)
                {
                    listBox4.Items.Add(CPlaneNetAdjust1.Ld.Detail[i, k].ToString());
                }
            }
            foreach (PointClass item in CPlaneNetAdjust1.UnknowPoints)
            {
                listBox5.Items.Add("x" + item.PID);
                listBox5.Items.Add("y" + item.PID);
            }
        }
        // 合并误差方程
        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.AllErrorEquations();
            MessageBox.Show("合并误差方程列结束");
        }
        // 平差计算
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.LevelAdjust();
            foreach (PointClass item in CPlaneNetAdjust1.UnknowPoints)
            {
                listBox6.Items.Add("x" + item.PID + "=" + item.X);
                listBox6.Items.Add("y" + item.PID + "=" + item.Y);
            }
        }
        // 一步解算
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            CPlaneNetAdjust1.Calc_StartPoints();
            CPlaneNetAdjust1.Calc_UnknowPoints();
            CPlaneNetAdjust1.X0Y0Calculate();
            CPlaneNetAdjust1.LevelAdjust();
            string ViewAjustPoints = CPlaneNetAdjust1.ViewlistPoints(CPlaneNetAdjust1.AjustPoints);
            MessageBox.Show(ViewAjustPoints, "所有点信息");
        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {

        }
    }
}
