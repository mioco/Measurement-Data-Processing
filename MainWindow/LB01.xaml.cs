using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDP
{
    /// <summary>
    /// LB01.xaml 的交互逻辑
    /// </summary>
    public partial class LB01 : Window
    {
        public LB01()
        {
            InitializeComponent();
        }

        public static List<PointClass> ControlPoints = new List<PointClass>();
        public static List<PointClass> CurrentPoints = new List<PointClass>();
        public static List<LineClass> CurrentSegments = new List<LineClass>();
        public double derta; // 单位全中误差

        #region 文件读取
        private void readData(object sender, RoutedEventArgs e)
        {
            Window1 readFileWindow = new Window1();
            readFileWindow.Owner = this;
            readFileWindow.ShowDialog();
            addData(readFileWindow.pointContent, pointFeature);
            addData(readFileWindow.edgeContent, edgeFeature);
            foreach (PointClass CP in CurrentPoints)
            {
                string temp = CP.PID.ToString() + ':' + "  " + CP.H.ToString() + "m;" + '\n';
                currentPoint.Items.Add(temp);
            }

            // 显示原始高程数值
            foreach (LineClass TLine in CurrentSegments)
            {
                //添加当前边起始点

                int Index_P = Belong(CurrentPoints, TLine.SP.PID);
                int Index = 0;
                if (Index_P == -1)
                {
                    //不在当前点集合中
                    Index = Belong(ControlPoints, TLine.SP.PID);
                    if (Index == -1)
                    {
                        //不是控制点
                        CurrentPoints.Add(new PointClass
                        {
                            PID = TLine.SP.PID,
                            //**********************************************************************************************************
                            //修改处：将非控制点初始高程显示为0，而不是10000
                            H = 0,
                            IsControlP = false,
                            IsH0 = false
                        });
                        TLine.SP.H = 0;
                        TLine.SP.IsControlP = false;
                        TLine.SP.IsH0 = false;
                    }
                    else
                    {
                        //是控制点
                        CurrentPoints.Add(ControlPoints[Index]);
                        TLine.SP = ControlPoints[Index];
                    }
                }
                //添加当前边尾点
                Index_P = Belong(CurrentPoints, TLine.EP.PID);
                if (Index_P == -1)
                {
                    //不在当前点集合中
                    Index = Belong(ControlPoints, TLine.EP.PID);
                    if (Index == -1)
                    {
                        //不是控制点
                        CurrentPoints.Add(new PointClass
                        {
                            PID = TLine.EP.PID,
                            H = 0,
                            IsControlP = false,
                            IsH0 = false
                        });
                        TLine.EP.H = 0;
                        TLine.EP.IsControlP = false;
                        TLine.EP.IsH0 = false;
                    }
                    else
                    {
                        //是控制点
                        CurrentPoints.Add(ControlPoints[Index]);
                        TLine.EP = ControlPoints[Index];
                    }
                }


            }

        }

        //遍历插入
        private void addData(ListBox original, ListBox current)
        {
            foreach (object li in original.Items)
            {
                current.Items.Add(li);
            }
        }
        #endregion

        #region 高程计算
        private void elevation(object sender, RoutedEventArgs e)
        {
            CaculateH(CurrentPoints, CurrentSegments);
            ControlPoints.RemoveRange(0, ControlPoints.Count);
            //净化控制点（去除非水准网中的控制点）
            foreach (PointClass P in CurrentPoints)
            {
                if (P.IsControlP == true)
                {
                    ControlPoints.Add(P);
                }
            }
        }

        // 高程
        public void CaculateH(List<PointClass> CurrentPoints, List<LineClass> CurrentSegments)
        {
            //临时变量
            string temp, temps;
            int Index = 0;
            //当前点有高程的个数
            int CP = 0;
            //当前点没有高程的个数
            int DP = 0;
            do
            {
                foreach (LineClass Tline in CurrentSegments)
                {   //获取最新的点信息
                    Index = Belong(CurrentPoints, Tline.SP.PID);
                    Tline.SP.H = CurrentPoints[Index].H;
                    Tline.SP.IsControlP = CurrentPoints[Index].IsControlP;
                    Tline.SP.IsH0 = CurrentPoints[Index].IsH0;

                    Index = Belong(CurrentPoints, Tline.EP.PID);
                    Tline.EP.H = CurrentPoints[Index].H;
                    Tline.EP.IsControlP = CurrentPoints[Index].IsControlP;
                    Tline.EP.IsH0 = CurrentPoints[Index].IsH0;
                    if (Tline.SP.IsH0 == true && Tline.EP.IsH0 == false)
                    {
                        Tline.EP.H = Tline.SP.H + Tline.dH;
                        Tline.EP.IsH0 = true;
                        Index = Belong(CurrentPoints, Tline.EP.PID);
                        CurrentPoints[Index].H = Tline.EP.H;
                        CurrentPoints[Index].IsH0 = true;
                    }
                    if (Tline.SP.IsH0 == false && Tline.EP.IsH0 == true)
                    {
                        Tline.SP.H = Tline.EP.H - Tline.dH;
                        Tline.SP.IsH0 = true;
                        Index = Belong(CurrentPoints, Tline.SP.PID);
                        CurrentPoints[Index].H = Tline.SP.H;
                        CurrentPoints[Index].IsH0 = true;
                    }
                }
                CP = 0;
                foreach (PointClass P in CurrentPoints)
                {
                    if (P.IsH0 == true)
                        CP++;

                }
                DP = CurrentPoints.Count() - CP;

            } while (DP > 0);

            //显示高程近似值
            temps = null;
            foreach (PointClass CP1 in CurrentPoints)
            {

                temp = CP1.PID.ToString() + ":  " + CP1.H.ToString() + "m, " + CP1.IsControlP.ToString() + "; " + CP1.IsH0.ToString() + ';' + '\n';
                temps = temps + temp;
            }
            height.Items.Add(temps);
        }

        private static int Belong(List<PointClass> CurrentPoints, string s)
        {
            foreach (PointClass TPoint in CurrentPoints)
            {
                if (TPoint.PID == s)
                    return CurrentPoints.IndexOf(TPoint);
            }
            return -1;
        }
        #endregion

        #region 水准网平差
        private void levellingNetwork(object sender, RoutedEventArgs e)
        {
            //定义两个临时变量用于显示平差结果
            string temp = null;
            string temps = null;
            double c = 0;
            double d = 0;


            //实例化一个平差类
            AdjustLevel MyAdjust = new AdjustLevel();
            //调用该类的平差方法
            MyAdjust.LevelAdjust(CurrentSegments, CurrentPoints, ControlPoints);
            //调用该类的字段（字段值在类外部不能修改）
            derta = MyAdjust.derta;
            //显示平差结果
            temps = null;
            int k = 0;
            foreach (PointClass CP1 in CurrentPoints)
            {
                if (CP1.IsControlP == false)
                {
                    string h = null;    //使平差结果保留到小数点后四位
                    c = Convert.ToDouble(CP1.H.ToString());
                    d = Math.Round(c, 4);
                    h = Convert.ToString(d);
                    temp = CP1.PID.ToString() + ":       " + h + "m," + '\n';
                    temps = temps + temp;
                    k++;

                }
            }

            adjustLevel.Items.Add(temps);
            MessageBox.Show("完成水准网平差");
        }
        #endregion

        #region 精度计算
        private void precition(object sender, RoutedEventArgs e)
        {
            double c, d;
            string h = null;
            c = Convert.ToDouble(derta.ToString());
            d = Math.Round(c, 6);
            h = Convert.ToString(d);

            accuracy.Items.Add("单位权中误差为：" + '\n' + "m=+—" + h + "mm");
        }
        #endregion

        #region 保存数据
        private void saveData(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 退出
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void draw(object sender, RoutedEventArgs e)
        {
            Bitmap bmap = new Bitmap(305, 359);
            Graphics gph = Graphics.FromImage(bmap);
            CurrentPoints[0].X = 300;
            CurrentPoints[0].Y = 300;
            CurrentPoints[1].X = 150;
            CurrentPoints[1].Y = 300;
            CurrentPoints[2].X = 100;
            CurrentPoints[2].Y = 350;
            CurrentPoints[3].X = 200;
            CurrentPoints[3].Y = 300;

            foreach (PointClass CP in CurrentPoints)
            {
                gph.DrawEllipse(Pens.Black, Convert.ToInt16(CP.X), Convert.ToInt16(CP.Y), 1, 1);
                gph.FillEllipse(new SolidBrush(System.Drawing.Color.Black), Convert.ToInt16(CP.X) - 1f, Convert.ToInt16(CP.Y) - 1f, 1, 1);

            }
            float x1, x2, y1, y2;
            for (int i = 0; i < 5; i++)
            {
                int a = Convert.ToInt16(CurrentSegments[i].SP.PID) - 1;
                int b = Convert.ToInt16(CurrentSegments[i].EP.PID) - 1;
                x1 = (float)CurrentPoints[a].X;
                y1 = (float)CurrentPoints[a].Y;
                x2 = (float)CurrentPoints[b].X;
                y2 = (float)CurrentPoints[b].Y;
                gph.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Black, 2), x1, y1, x2, y2);
            }
            graphBox.Source = loadBitmap(bmap);
        }

        // 将Bitmap转换成BitmapSource
        [System.Runtime.InteropServices.DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
    }
}
