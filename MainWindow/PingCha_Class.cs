using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP
{
    public class PingCha_Class
    {
        public List<PointClass> ControlPoints = new List<PointClass>();//控制点集合
        public List<StationClass> StationInfos = new List<StationClass>();//测站观测数据
        public List<PointClass> StartPoints = new List<PointClass>();//当前点数据包括控制点
        public List<PointClass> UnknowPoints = new List<PointClass>();//未知点集合
        public List<PointClass> AjustPoints = new List<PointClass>();//平差后的值
        public int DistNum;//观测边的数量
        //属性
        public Matrix Ba { get; set; }//角度误差方程系数矩阵
        public Matrix Bd { get; set; }//距离误差方程系数
        public Matrix B { get; set; }//总误差方程式系数
        public Matrix Pa { get; set; }//角度观测值权
        public Matrix Pd { get; set; }//距离观测值权
        public Matrix P { get; set; }//总的观测值权
        public Matrix La { get; set; }//角度误差方程常数项
        public Matrix Ld { get; set; }//距离误差方程常数项
        public Matrix L { get; set; }//总的常数项
        public Matrix Q { get; set; }//总的协因数阵
        public Matrix x { get; set; }//待定点改正数
        public Matrix V { get; set; }//待定点改正数
        public Matrix Va { get; set; }//角度改正数
        public Matrix Vd { get; set; }//距离改正数
        //方法
        public List<PointClass> InputCtrData(string FileName)
        {
            //返回值通过共有字段ControlPoints返回
            FileStream fs1 = new FileStream(FileName, FileMode.Open);
            //定义并实例化一个FileStream，并以FileMode.Open打开
            StreamReader MyReader = new StreamReader(fs1, Encoding.ASCII);
            //定义并实例化一个StreamReader，并以ASCII格式读取
            //这两个类在System.IO命名空间中，在定义前加入该命名空间
            while (MyReader.EndOfStream != true)
            {
                List<string> TempString = MyReader.ReadLine().Split(' ').ToList<string>();
                //空格作为分隔符号
                TempString.RemoveAll(Tool_Class.IsSpace);//移除所有空格的元素
                ControlPoints.Add(new PointClass//读取的数据添加到ControlPoints点集合中
                {
                    PID = TempString[0],//点名
                    X = double.Parse(TempString[1]),//X坐标
                    Y = double.Parse(TempString[2]),//Y坐标
                    IsControlP = true//是否是控制点
                });
            }
            MyReader.Close();
            return ControlPoints;
        }
        //读入观测数据
        public List<StationClass> InputObsData(string FileName)
        //第一个参数为文件路径及名称，通过StationInfos返回函数值
        {
            FileStream fs = new FileStream(FileName, FileMode.Open);//定义流文件类
            StreamReader Reader = new StreamReader(fs, Encoding.ASCII);//定义读取文件类
            //文件是否到最后
            while (Reader.EndOfStream != true)//通过循环读取文件信息
            {
                List<string> TempString = Reader.ReadLine().Split(' ').ToList<string>();
                //以空格作为分隔符号
                TempString.RemoveAll(Tool_Class.IsSpace);//删除所有的空格元素
                StationInfos.Add(new StationClass//添加测站元素
                {
                    SP = new PointClass { PID = TempString[0] },//测站点名
                    BP = new PointClass { PID = TempString[1] },//后视点名
                    FP = new PointClass { PID = TempString[2] },//前视点名
                    LAngle = Tool_Class.Deg_DMSToRad((double.Parse(TempString[3])) / 10000),
                    //将度分秒转成弧度付给左角
                    DisB = double.Parse(TempString[4]),//后视距离
                    DisF = double.Parse(TempString[5])//前视距离
                });
            }
            Reader.Close();//流文件关闭
            PerfectStationInfos();//完善测站信息，添加控制点、方位角及距离
            return StationInfos;
        }
        //完善测站信息
        private void PerfectStationInfos()
        {
            //添加控制点信息
            foreach (StationClass TC in StationInfos)
            {
                int index = Tool_Class.Belong(ControlPoints, TC.SP.PID);
                if (index > -1)
                {
                    TC.SP.X = ControlPoints[index].X;
                    TC.SP.Y = ControlPoints[index].Y;
                    TC.SP.IsControlP = true;
                }
                index = Tool_Class.Belong(ControlPoints, TC.FP.PID);
                if (index > -1)
                {
                    TC.FP.X = ControlPoints[index].X;
                    TC.FP.Y = ControlPoints[index].Y;
                    TC.FP.IsControlP = true;
                }
                index = Tool_Class.Belong(ControlPoints, TC.BP.PID);
                if (index > -1)
                {
                    TC.BP.X = ControlPoints[index].X;
                    TC.BP.Y = ControlPoints[index].Y;
                    TC.BP.IsControlP = true;
                }
            }
            //将当前点信息添加到测站信息
            foreach (StationClass TC in StationInfos)
            {
                int SP_index, FP_index, BP_index;
                SP_index = Tool_Class.Belong(StartPoints, TC.SP.PID);
                if (SP_index > -1)
                {
                    TC.SP = StartPoints[SP_index];
                }
                FP_index = Tool_Class.Belong(StartPoints, TC.FP.PID);
                if (SP_index > -1)
                {
                    TC.FP = StartPoints[FP_index];
                }
                BP_index = Tool_Class.Belong(StartPoints, TC.BP.PID);
                if (BP_index > -1)
                {
                    TC.BP = StartPoints[BP_index];
                }
            }
            //添加已知两点的方位角
            foreach (StationClass TC in StationInfos)
            {
                if (TC.SP.IsControlP && TC.BP.IsControlP)
                {
                    TC.BAzimuth = Tool_Class.IIPointFW(TC.SP, TC.BP);
                    TC.DisB = Tool_Class.distIIP(TC.SP, TC.BP);
                }
                if (TC.SP.IsControlP && TC.FP.IsControlP)
                {
                    TC.FAzimuth = Tool_Class.IIPointFW(TC.SP, TC.FP);
                    TC.DisF = Tool_Class.distIIP(TC.SP, TC.FP);
                }
            }
        }
        //计算水平网用到的点
        public List<PointClass> Calc_StartPoints()
        {
            foreach (StationClass ST in StationInfos)//提取用到的点
            {
                int CP_Index = Tool_Class.Belong(StartPoints, ST.BP.PID);
                //计算后视点在当前点集中
                if (CP_Index == -1)
                {
                    StartPoints.Add(ST.BP); //不在执行添加
                }
                CP_Index = Tool_Class.Belong(StartPoints, ST.SP.PID);
                if (CP_Index == -1)
                {
                    StartPoints.Add(ST.SP);//不在执行添加
                }
                CP_Index = Tool_Class.Belong(StartPoints, ST.FP.PID);
                if (CP_Index == -1)
                {
                    StartPoints.Add(ST.FP);//不在执行添加
                }
            }
            //加入控制点信息
            foreach (PointClass P in StartPoints)
            {
                int cp_index = Tool_Class.Belong(ControlPoints, P.PID);
                //计算当前点是否在控制点集合中
                if (cp_index > -1)//在加入控制点信息
                {
                    P.X = ControlPoints[cp_index].X;
                    P.Y = ControlPoints[cp_index].Y;
                    P.IsControlP = true;
                }
            }
            return StartPoints;
        }
        //计算未知点
        public List<PointClass> Calc_UnknowPoints()
        {
            foreach (PointClass p in StartPoints)
            {
                if (!p.IsControlP)
                {
                    UnknowPoints.Add(p);
                }
            }
            return UnknowPoints;
        }
        public string ViewStationInfos()
        {
            string temp = null;
            string temps = "测站 后视 前视 左角 后视距 后视方位角前视距前视方位角\n";
            foreach (StationClass CP1 in StationInfos)
            {
                temp = string.Format("{0,4}", CP1.SP.PID.ToString()) + string.Format("{0,5}",
                CP1.BP.PID.ToString()) + string.Format("{0,6}", CP1.FP.PID.ToString()) +
                string.Format("{0,15}", Tool_Class.RadToDeg_DMS(CP1.LAngle)) + string.Format("{0,11}",
                CP1.DisB.ToString("f3")) + string.Format("{0,15}",
                Tool_Class.RadToDeg_DMS(CP1.BAzimuth))
                + string.Format("{0,11}", CP1.DisF.ToString("f3")) +
                string.Format("{0,15}", Tool_Class.RadToDeg_DMS(CP1.FAzimuth))
                + '\n';
                temps = temps + temp;
            }
            return temps;
        }
        public string ViewlistPoints(List<PointClass> listpoints)
        {
            string temp = null;
            string temps = "点名： 横坐标X（m） 纵坐标Y（m） 控制点\n";
            foreach (PointClass CP1 in listpoints)
            {
                temp = string.Format("{0,-6}", CP1.PID.ToString()) + ':' +
                string.Format("{0,12}", CP1.X.ToString("f4")) + "m, " + string.Format("{0,12}",
                CP1.Y.ToString("f4")) + "m; " + string.Format("{0,8}", CP1.IsControlP.ToString()) +
                '\n';
                temps = temps + temp;
            }
            return temps;
        }
        private void updataUnknowPoints(List<PointClass> unknowPoints, List<PointClass>
StartPoints)
        {
            foreach (PointClass p in unknowPoints)
            {
                int index = Tool_Class.Belong(StartPoints, p.PID);
                if (index > -1)
                {
                    p.X = StartPoints[index].X;
                    p.Y = StartPoints[index].Y;
                    StartPoints[index].IsControlP = false;
                    p.IsControlP = false;
                }
            }
        }
        //初始值计算
        public void X0Y0Calculate()
        {
            int unkPnumber = UnknowPoints.Count;
            int kk = 0;
            while (kk < unkPnumber)
            {
                foreach (StationClass SC in StationInfos)
                {
                    int SP_Index = Tool_Class.Belong(StartPoints, SC.SP.PID);
                    if (StartPoints[SP_Index].IsControlP)//测站有点
                    {
                        SC.SP.X = StartPoints[SP_Index].X;
                        SC.SP.Y = StartPoints[SP_Index].Y;
                        SC.SP.IsControlP = true;
                        int BP_Index = Tool_Class.Belong(StartPoints, SC.BP.PID);
                        if (StartPoints[BP_Index].IsControlP)//后视有点
                        {
                            SC.BP.X = StartPoints[BP_Index].X;
                            SC.BP.Y = StartPoints[BP_Index].Y;
                            SC.BP.IsControlP = true;
                            int FP_Index = Tool_Class.Belong(StartPoints, SC.FP.PID);
                            if (StartPoints[FP_Index].IsControlP == false)//前视无点
                            {
                                double BAzimuth = Tool_Class.IIPointFW(SC.SP, SC.BP);
                                double FAzimuth = BAzimuth + SC.LAngle;
                                FAzimuth = Tool_Class.AjustAngle(FAzimuth);//前视方位角
                                SC.FP = Tool_Class.AzimuthDistToP(SC.SP, FAzimuth,
                                SC.DisF);
                                SC.FP.PID = StartPoints[FP_Index].PID;
                                SC.FP.IsControlP = true;
                                StartPoints[FP_Index].X = SC.FP.X;
                                StartPoints[FP_Index].Y = SC.FP.Y;
                                StartPoints[FP_Index].IsControlP = true;
                                kk++;
                            }
                        }
                        else//后视无点
                        {
                            int FP_Index = Tool_Class.Belong(StartPoints, SC.FP.PID);
                            if (StartPoints[FP_Index].IsControlP)//前视有点
                            {
                                SC.FP.X = StartPoints[FP_Index].X;
                                SC.FP.Y = StartPoints[FP_Index].Y;
                                SC.FP.IsControlP = true;
                                double FAzimuth = Tool_Class.IIPointFW(SC.SP, SC.FP);
                                double BAzimuth = FAzimuth - SC.LAngle;
                                BAzimuth = Tool_Class.AjustAngle(BAzimuth);
                                SC.BP = Tool_Class.AzimuthDistToP(SC.SP, BAzimuth,
                                SC.DisB);
                                SC.BP.PID = StartPoints[BP_Index].PID;
                                SC.BP.IsControlP = true;
                                StartPoints[BP_Index].X = SC.BP.X;
                                StartPoints[BP_Index].Y = SC.BP.Y;
                                StartPoints[BP_Index].IsControlP = true;
                                kk++;
                            }
                        }
                    }
                }
            }
            //更新未知点
            PerfectStationInfos();//补充测站信息
            updataUnknowPoints(UnknowPoints, StartPoints);//更新未知点
        }
        //提取边信息
        public void ExtractLineDist(List<LineClass> Distances)
        {
            List<LineClass> Lines = new List<LineClass>();
            foreach (StationClass SC in StationInfos)
            {
                Lines.Add(new LineClass
                {
                    SP = new PointClass
                    {
                        PID = SC.SP.PID,
                        X = SC.SP.X,
                        Y = SC.SP.Y
                    },
                    EP = new PointClass
                    {
                        PID = SC.BP.PID,
                        X = SC.BP.X,
                        Y = SC.BP.Y
                    },
                    Distance = SC.DisB
                });
                Lines.Add(new LineClass
                {
                    SP = new PointClass
                    {
                        PID = SC.SP.PID,
                        X = SC.SP.X,
                        Y = SC.SP.Y
                    },
                    EP = new PointClass
                    {
                        PID = SC.FP.PID,
                        X = SC.FP.X,
                        Y = SC.FP.Y
                    },
                    Distance = SC.DisF
                });
            }
            foreach (LineClass L in Lines)
            {
                bool IsExist = false;
                string FLName = L.SP.PID + L.EP.PID;
                foreach (LineClass LT in Distances)
                {
                    string CLName1 = LT.SP.PID + LT.EP.PID;
                    string CLName2 = LT.EP.PID + LT.SP.PID;
                    if (string.Equals(FLName, CLName1) || string.Equals(FLName,
                    CLName2))
                    {
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    int SPIndex = Tool_Class.Belong(ControlPoints, L.SP.PID);
                    int EPIndex = Tool_Class.Belong(ControlPoints, L.EP.PID);
                    bool SPISContr = false, EPISContr = false;
                    if (SPIndex > -1)
                    {
                        SPISContr = true;
                    }
                    if (EPIndex > -1)
                    {
                        EPISContr = true;
                    }
                    if (SPISContr && EPISContr)//非控制点
                    {
                        IsExist = false;
                    }
                    else
                    {
                        Distances.Add(L);
                        IsExist = false;
                    }
                }
            }
            DistNum = Distances.Count;//得到测边数
        }
        public void AngleErrorEquations()
        {
            int m_Pnumber = UnknowPoints.Count();
            int numAngle = StationInfos.Count();
            PointClass FP, SP, BP;
            Ba = new Matrix(numAngle, m_Pnumber * 2);//系数
            La = new Matrix(numAngle, 1);//常数项
            Va = new Matrix(numAngle, 1);//改正数
            for (int Index = 0; Index < numAngle; Index++)
            {
                FP = StationInfos[Index].FP;
                BP = StationInfos[Index].BP;
                SP = StationInfos[Index].SP;
                int j, k, h;
                double axj, ayj, bxk, byk, cxh, cyh;
                double rou = 180 * 60 * 60 / Math.PI;
                double dertY0_jk, dertY0_jh;
                double dertX0_jk, dertX0_jh;
                double S0_jk, S0_jh;
                double Li;
                j = Tool_Class.Belong(UnknowPoints, SP.PID);
                k = Tool_Class.Belong(UnknowPoints, FP.PID);
                h = Tool_Class.Belong(UnknowPoints, BP.PID);
                S0_jh = StationInfos[Index].DisB;
                S0_jk = StationInfos[Index].DisF;
                dertY0_jh = BP.Y - SP.Y;
                dertX0_jh = BP.X - SP.X;
                dertY0_jk = FP.Y - SP.Y;
                dertX0_jk = FP.X - SP.X;
                axj = rou * (dertY0_jk / S0_jk / S0_jk - dertY0_jh / S0_jh / S0_jh);
                ayj = -rou * (dertX0_jk / S0_jk / S0_jk - dertX0_jh / S0_jh / S0_jh);
                bxk = -rou * dertY0_jk / S0_jk / S0_jk;
                byk = rou * dertX0_jk / S0_jk / S0_jk;
                cxh = rou * dertY0_jh / S0_jh / S0_jh;
                cyh = -rou * dertX0_jh / S0_jh / S0_jh;
                Li = StationInfos[Index].LAngle - (StationInfos[Index].FAzimuth -
                StationInfos[Index].BAzimuth);
                //非控制点放入到系数矩阵
                if (j > -1)
                {
                    Ba.SetNum(Index, 2 * j, axj);
                    Ba.SetNum(Index, 2 * j + 1, ayj);
                }
                if (k > -1)
                {
                    Ba.SetNum(Index, 2 * k, bxk);
                    Ba.SetNum(Index, 2 * k + 1, byk);
                }
                if (h > -1)
                {
                    Ba.SetNum(Index, 2 * h, cxh);
                    Ba.SetNum(Index, 2 * h + 1, cyh);
                }
                La.SetNum(Index, 0, Li);
            }
        }


        public void DistErrorEquations()
        {
            int m_Pnumber = UnknowPoints.Count();
            int numAngle = StationInfos.Count();
            List<LineClass> Distances = new List<LineClass>();//观测距离
                                                              //列边长误差方程
            ExtractLineDist(Distances);
            int DistNum = Distances.Count;
            Bd = new Matrix(DistNum, m_Pnumber * 2);
            Ld = new Matrix(DistNum, 1);
            Vd = new Matrix(DistNum, 1);
            for (int D_index = 0; D_index < Distances.Count - 1; D_index++)
            {
                double dxj, dyj, dxk, dyk;
                int j = Tool_Class.Belong(UnknowPoints, Distances[D_index].SP.PID);
                int k = Tool_Class.Belong(UnknowPoints, Distances[D_index].EP.PID);
                double FWjk = Tool_Class.IIPointFW(Distances[D_index].SP,
                Distances[D_index].EP);
                dxj = -Math.Cos(FWjk);
                dyj = -Math.Sin(FWjk);
                dxk = -dxj;
                dyk = -dyj;
                if (j > -1)
                {
                    Bd.SetNum(D_index, 2 * j, dxj);
                    Bd.SetNum(D_index, 2 * j + 1, dyj);
                }
                if (k > -1)
                {
                    Bd.SetNum(D_index, 2 * k, dxk);
                    Bd.SetNum(D_index, 2 * k + 1, dyk);
                }
                double ld = Distances[D_index].Distance -
                Tool_Class.distIIP(Distances[D_index].SP, Distances[D_index].EP);
                Ld.SetNum(D_index, 0, ld);
            }
        }
        public void AllErrorEquations()
        {
            //总的误差方程计算
            int m_Pnumber = UnknowPoints.Count();
            int numAngle = StationInfos.Count();
            B = new Matrix(DistNum + numAngle, m_Pnumber * 2);
            L = new Matrix(DistNum + numAngle, 1);
            int bi, bj;
            for (bi = 0; bi < numAngle; bi++)
            {
                for (bj = 0; bj < m_Pnumber * 2; bj++)
                {
                    B.SetNum(bi, bj, Ba.getNum(bi, bj));
                }
                L.SetNum(bi, 0, La.getNum(bi, 0));
            }
            for (bi = numAngle; bi < DistNum + numAngle; bi++)
            {
                for (bj = 0; bj < m_Pnumber * 2; bj++)
                {
                    double tt = Bd.getNum(bi - numAngle, bj);
                    B.SetNum(bi, bj, tt);
                }
                L.SetNum(bi, 0, Ld.getNum(bi - numAngle, 0));
            }
        }
        private void PMatrix()
        {
            int bi;
            int m_Pnumber = UnknowPoints.Count();
            int numAngle = StationInfos.Count();
            P = new Matrix(DistNum + numAngle, DistNum + numAngle);
            //设夹角的中误差为1“，距离的中误差为0.005m
            //角度权=1，距离全4*10000（秒/米）平方
            for (bi = 0; bi < numAngle; bi++)
            {
                P.SetNum(bi, bi, 1);
            }
            for (bi = numAngle; bi < DistNum + numAngle; bi++)
            {
                P.SetNum(bi, bi, 40000);
            }
            L = new Matrix(DistNum + numAngle, 1);
            for (bi = 0; bi < numAngle; bi++)
            {
                L.SetNum(bi, 0, La.getNum(bi, 0));
            }
            for (bi = numAngle; bi < DistNum + numAngle; bi++)
            {
                L.SetNum(bi, 0, Ld.getNum(bi - numAngle, 0));
            }
        }
        public void LevelAdjust()
        {
            AngleErrorEquations();//列角度误差方程
            DistErrorEquations();//列边长误差方程
            AllErrorEquations();//合并误差方程
            PMatrix();//权阵
            //法方程
            Q = Matrix.Inverse(Matrix.Mutiply(Matrix.Mutiply(B.Transpose(), P), B));
            x = Matrix.Mutiply(Q, Matrix.Mutiply(Matrix.Mutiply(B.Transpose(), P), L));
            V = Matrix.subtraction(Matrix.Mutiply(B, x), L);
            for (int index = 0; index < UnknowPoints.Count; index++)
            {
                AjustPoints.Add(new PointClass
                {
                    PID = UnknowPoints[index].PID,
                    X = UnknowPoints[index].X + x.getNum(index * 2, 0),
                    Y = UnknowPoints[index].Y + x.getNum(index * 2 + 1, 0)
                });
            }
        }
    }
}
