using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP
{
    public class CoordtoTrans
    {
        public void InputData(string FileName, List<PointClass> Points)
        {
            FileStream fs1 = new FileStream(FileName, FileMode.Open);
            StreamReader Reader = new StreamReader(fs1, Encoding.ASCII);

            while (Reader.EndOfStream != true)
            {
                List<string> TempString = Reader.ReadLine().Split(' ').ToList<string>();
                TempString.RemoveAll(Tool_Class.IsSpace);
                Points.Add(new PointClass
                {
                    PID = TempString[0],
                    X = double.Parse(TempString[1]),
                    Y = double.Parse(TempString[2]),
                });

            }
            Reader.Close();
        }

        public List<PointClass> IICoordTans(List<PointClass> SourcePoints, List<PointClass> TargetPoints, List<PointClass> IIDimTarPoints)
        {
            int Index = 0;
            List<PointClass> CommonP = new List<PointClass>();
            //计算公共点，并将公共点存到COMMONp
            foreach (PointClass TP in SourcePoints)
            {
                Index = Tool_Class.Belong(TargetPoints, TP.PID);//获取原坐标点在目标坐标系中的索引值
                if (Index != -1)
                {
                    CommonP.Add(new PointClass
                    {
                        PID = TP.PID,
                        X = TP.X,
                        Y = TP.Y,
                        IsCommonP = true
                    });
                }
            }

            //确定旋转电与参照点：计算最远距离点，从而确定了旋转点（第0个元素）与参照点（第一个元素）点号
            string[] sArray = new string[2];//记录旋转点与参照点名
            PointClass SRotateP = new PointClass();//源坐标旋转点
            PointClass SRefP = new PointClass();//源坐标参照点
            PointClass TRotateP = new PointClass();//目标坐标旋转点
            PointClass TRefP = new PointClass();//目标坐标参照点
            sArray = Tool_Class.DistMaxListP(CommonP);
            Index = Tool_Class.Belong(CommonP, sArray[0]);
            SRotateP = CommonP[Index];
            Index = Tool_Class.Belong(CommonP, sArray[1]);
            SRefP = CommonP[Index];
            //////////////////////////////////////////////
            double sfw = Tool_Class.IIPointFW(SRotateP, SRefP);//确定源坐标两个点（旋转点和参照点）方位角
            double sd = Tool_Class.distIIP(SRotateP, SRefP);//确定源坐标两个点（旋转点和参照点）距离
            ////////////获取目标系的旋转点与参照点
            Index = Tool_Class.Belong(TargetPoints, sArray[0]);
            TRotateP = TargetPoints[Index];
            Index = Tool_Class.Belong(TargetPoints, sArray[1]);
            TRefP = TargetPoints[Index];
            ////////////////
            double Tfw = Tool_Class.IIPointFW(TRotateP, TRefP);//确定源坐标两个点（旋转点和参照点）方位角
            double Td = Tool_Class.distIIP(TRotateP, TRefP);//确定源坐标两个点（旋转点和参照点）距离
            double detfw = Tfw - sfw;
            double k = Td / sd - 1;
            //定义基线向量的矩阵
            Matrix sBase = new Matrix(2, SourcePoints.Count);
            Matrix tBase = new Matrix(2, SourcePoints.Count);
            //计算源坐标系中各点相对于旋转点的基线向量，并将其扩大到（k+1）后存储到sBase矩阵中
            for (Index = 0; Index < SourcePoints.Count; Index++)
            {
                double Bx = (SourcePoints[Index].X - SRotateP.X) * (1 + k);
                double By = (SourcePoints[Index].Y - SRotateP.Y) * (1 + k);
                sBase.SetNum(0, Index, Bx);
                sBase.SetNum(1, Index, By);
            }
            //计算旋转矩阵
            Matrix R = new Matrix(2, 2);
            R.SetNum(0, 0, Math.Cos(detfw));
            R.SetNum(0, 1, -Math.Sin(detfw));
            R.SetNum(1, 0, 1 * Math.Sin(detfw));
            R.SetNum(1, 1, Math.Cos(detfw));
            //对sBase中的基线向量进行旋转放到tBase中
            tBase = Matrix.Mutiply(R, sBase);
            //平移后，最终结果放到IIDimTraPoints
            for (Index = 0; Index < SourcePoints.Count; Index++)
            {
                IIDimTarPoints.Add(new PointClass
                {
                    PID = SourcePoints[Index].PID,
                    X = TRotateP.X + tBase.getNum(0, Index),
                    Y = TRotateP.Y + tBase.getNum(1, Index)
                });
            }
            return CommonP;
        }

        public string ViewPoints(List<PointClass> Points)//注意这个返回值自己加的
        {
            string temps = null;
            string temp = null;
            foreach (PointClass TP in Points)
            {
                temp = TP.PID.ToString() + ':' + "X=" + TP.X.ToString() + ':' + "Y=" + TP.Y.ToString() + ";";
                temps = temps + temp + '\n';
            }
            return temps;
        }
    }
}
