using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MDP
{
    public static class Tool_Class
    {
        public static int Belong(List<PointClass> Points, string s)//或取点（S）在点击合中的索引值
        {
            foreach (PointClass TPoint in Points)
            {
                if (TPoint.PID == s)
                    return Points.IndexOf(TPoint);
            }
            return -1;
        }
        public static string[] DistMaxListP(List<PointClass> Points)
        {
            string[] sArray = new string[2];
            int i, j, np;
            double dis_Temp, dis_Max = 0;
            np = Points.Count;
            for (i = 0; i < np - 1; i++)
            {
                for (j = i + 1; j < np; j++)
                {
                    dis_Temp = distIIP(Points[i], Points[j]);
                    if (dis_Temp > dis_Max)
                    {
                        sArray[0] = Points[i].PID;//记录点名
                        sArray[1] = Points[j].PID;
                        dis_Max = dis_Temp;
                    }
                }
            }
            return sArray;
        }
        public static double distIIP(PointClass P1, PointClass P2)
        {
            double dist = 0;
            dist = Math.Sqrt((P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P1.Y -
            P2.Y));
            dist = Math.Abs(dist);
            return dist;
        }
        //计算方位角
        public static double IIPointFW(PointClass P1, PointClass P2)//计算第一点指向第二点的方位角，单位为弧度
        {
            //先求出AB的象限角：
            //θ=arctan((Y2-Y1)/(X2-X1))
            double detx, dety, xita, FW = -1000;
            detx = P2.X - P1.X;
            dety = P2.Y - P1.Y;
            if (detx == 0)
            {
                if (dety > 0)
                {
                    FW = Math.PI / 2;
                    return FW;
                }
                else
                {
                    FW = 3 * Math.PI / 2;
                    return FW;
                }
            }
            else
            {
                xita = Math.Atan(dety / detx);
                xita = Math.Abs(xita);
            }
            if (detx > 0 && dety >= 0)
            {
                FW = xita;
            }
            if (detx < 0 && dety >= 0)
            {
                FW = Math.PI - xita;
            }
            if (detx < 0 && dety <= 0)
            {
                FW = Math.PI + xita;
            }
            if (detx > 0 && dety <= 0)
            {
                FW = 2 * Math.PI - xita;
            }
            if (FW > 2 * Math.PI)
            {
                FW = xita - 2 * Math.PI;
            }
            if (FW < 0)
            {
                FW = xita + 2 * Math.PI;
            }
            return FW;
        }
        public static double AjustAngle(double angle)
        {
            if (angle > 2 * Math.PI)
            {
                angle = angle - 2 * Math.PI;
            }
            if (angle < 0)
            {
                angle = angle + 2 * Math.PI;
            }
            return angle;
        }
        //定义弧度转度的方法
        public static double RadToDeg(double Radian)//弧度（Radian)转角度（degree）
        {
            double Deg;
            Deg = Radian / Math.PI / 2 * 360;
            return Deg;
        }
        //定义弧度制转角度制 度分秒方法
        public static string RadToDeg_DMS(double Radian)
        {
            int d, m;
            double s;
            string DMS = null;
            double hudu;
            hudu = Radian / Math.PI / 2 * 360;
            if (hudu > 0)
            {
                d = (int)Math.Floor(hudu);
                m = (int)Math.Floor((hudu - d) * 60);
                s = ((((hudu - d) * 60) - m) * 60);
                s = Math.Round(s, 1);
                DMS = d.ToString() + "°" + m.ToString("d2") + "'" + s.ToString("f1") +
                "''";
            }
            else
            {
                hudu = -hudu;
                d = (int)hudu;
                m = (int)((hudu - d) * 60);
                s = ((((hudu - d) * 60) - m) * 60);
                DMS = "-" + d.ToString() + "°" + m.ToString("d2") + "'" +
                s.ToString("f1") + "''";
            }
            return DMS;
        }
        //度分秒转换弧度
        public static double Deg_DMSToRad(double Deg)
        {
            double du = Math.Floor(Deg);
            double fen = Math.Floor((Deg - du) * 100);
            double miao = Deg * 10000 - du * 10000 - fen * 100;
            double Rad = (du + fen / 60.0 + miao / 3600.0) / 180.0 * Math.PI;
            return Rad;
        }
        public static bool IsSpace(string s)
        {
            if (s == "")
                return true;
            else return false;
        }
        public static PointClass AzimuthDistToP(PointClass SP, double Azimuth, double
        Distance)
        {
            PointClass tagetP = new PointClass();
            tagetP.X = SP.X + Distance * Math.Cos(Azimuth);
            tagetP.Y = SP.Y + Distance * Math.Sin(Azimuth);
            return tagetP;
        }

        // 添加数据
        public static void addData<T>(DataGrid dataGrid, List<T> list)
        {
            dataGrid.ItemsSource = null;
            // dataGrid.AutoGenerateColumns = false;
            dataGrid.ItemsSource = list;
        }
    }
}
