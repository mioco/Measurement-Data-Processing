using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDP
{

    /// <summary>
    /// 水准网平差类
    /// </summary>
    public class AdjustLevel
    {

        public Matrix B { get; set; }//误差方程系数矩阵
        public Matrix P { get; set; }//观测值权阵
        public Matrix l { get; set; }//误差方成常数项
        public Matrix Q { get; set; }//协因数阵
        public Matrix x { get; set; }//待定点改正数
        public Matrix V { get; set; }//观测值改正数
        public Matrix Temp_M { get; set; }//临时的属性
        public double derta;//单位权中误差
        public int n;//观测数
        public int t;//必要观测数
        public void LevelAdjust(List<LineClass> CurrentSegments, List<PointClass> CurrentPoints, List<PointClass> ControlPoints)
        {

            n = CurrentSegments.Count;//计算观测数
            t = CurrentPoints.Count - ControlPoints.Count;//计算必要观测数，ControlPoints是水准网中的控制点，应去除不相关控制点。
            double temp = 0;
            B = new Matrix(n, t);//初始化
            P = new Matrix(n, n);
            l = new Matrix(n, 1);

            //再当前点中提取待定点，去除控制点
            List<PointClass> TPoints = new List<PointClass>();
            foreach (PointClass Tp in CurrentPoints)
            {
                if (Tp.IsControlP == false)
                    TPoints.Add(Tp);
            }
            //k相当于观测边号
            for (int k = 0; k < n; k++)
            {
                //i，j为待定点参数的序号
                int i = Belong(TPoints, CurrentSegments[k].SP.PID);
                int j = Belong(TPoints, CurrentSegments[k].EP.PID);
                temp = 2.0 / CurrentSegments[k].Distance;//权值
                P.SetNum(k, k, temp);
                //终点观测高程-终点近似高程
                temp = CurrentSegments[k].SP.H + CurrentSegments[k].dH - CurrentSegments[k].EP.H;
                //改正数以mm为单位
                temp = temp * 1000;
                l.SetNum(k, 0, temp);//常数项设值
                if (CurrentSegments[k].SP.IsControlP == false)
                    B.SetNum(k, i, -1);//观测边起点系数为-1
                if (CurrentSegments[k].EP.IsControlP == false)
                    B.SetNum(k, j, 1);//观测边终点系数为+1
            }

            Q = Matrix.Inverse(Matrix.Mutiply(Matrix.Mutiply(B.Transpose(), P), B));//Q=BTPB_1
            x = Matrix.Mutiply(Q, Matrix.Mutiply(Matrix.Mutiply(B.Transpose(), P), l));//x=BTPB_1*BTPL
            V = Matrix.subtraction(Matrix.Mutiply(B, x), l);//V=BX-L
            //计算待定点平差值
            int kk = 0;
            foreach (PointClass TP in CurrentPoints)
            {

                if (TP.IsControlP == false)
                {
                    TP.H = TP.H + x.getNum(kk, 0) / 1000;
                    kk++;
                }
            }
            //计算精度
            Temp_M = Matrix.Mutiply(Matrix.Mutiply(V.Transpose(), P), V);
            derta = Math.Sqrt(Temp_M.getNum(0, 0) / (n - t));
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
    }
}
