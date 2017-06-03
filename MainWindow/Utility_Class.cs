using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDP
{
    #region 实验一
    //点要素类重新定义
    public class PointClass
    {
        public string PID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }
        //是否是控制点
        public bool IsControlP { get; set; }
        //是否有初始高程
        public bool IsH0 { get; set; }
        //是否是公共点
        public bool IsCommonP { get; set; }
    }
    //边要素类
    public class LineClass
    {

        public string LID { get; set; }             //观测边号
        public PointClass SP { get; set; }        //起点
        public PointClass EP { get; set; }        //终点
        public double dH { get; set; }             //观测高差
        public double Distance { get; set; }       //观测距离
    }
    #endregion

    #region 实验二

    // 测站信息类
    public class StationClass
    {
        public PointClass SP { get; set; } //测站点
        public PointClass BP { get; set; } //后视
        public PointClass FP { get; set; } //前视
        public double DisB { get; set; } //后视距离
        public double DisF { get; set; } //后视距离
        public double RAngle { get; set; } //右角
        public double LAngle { get; set; } //左角
        public double BAzimuth { get; set; } //后视方位角
        public double FAzimuth { get; set; } //前视方位角
        public double SdetX { get; set; }//测站纵坐标的增量
        public double SdetY { get; set; }//测站横坐标增量
    }
    
    #endregion
}

