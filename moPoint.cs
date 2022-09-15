using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    /// <summary>
    /// 点
    /// </summary>
    public class moPoint:moGeometry
    {
        #region 字段

        private double _X, _Y; //x、y坐标

        #endregion

        #region 构造函数

        public moPoint(double x,double y)
        {
            _X = x;
            _Y = y;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置X坐标
        /// </summary>
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }

        /// <summary>
        /// 获取或设置Y坐标
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        #endregion

        #region 方法

        public moPoint Clone()
        {
            moPoint sPoint = new moPoint(_X, _Y);
            return sPoint;
        }

        #endregion

    }
}
