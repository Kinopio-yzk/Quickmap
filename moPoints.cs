using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moPoints:moShape
    {
        #region 字段

        private List<moPoint> _Points;
        private double _MinX = double.MaxValue, _MaxX = double.MinValue;
        private double _MinY = double.MaxValue, _MaxY = double.MinValue;

        #endregion

        #region 构造函数

        public moPoints()
        {
            _Points = new List<moPoint>();
        }

        public moPoints(moPoint[] points)
        {
            _Points = new List<moPoint>();
            _Points.AddRange(points);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取点的数量
        /// </summary>
        public Int32 Count
        {
            get { return _Points.Count; }
        }

        /// <summary>
        /// 获取最小X坐标
        /// </summary>
        public double MinX
        {
            get { return _MinX; }
        }

        /// <summary>
        /// 获取最大X坐标
        /// </summary>
        public double MaxX
        {
            get { return _MaxX; }
        }

        /// <summary>
        /// 获取最小Y坐标
        /// </summary>
        public double MinY
        {
            get { return _MinY; }
        }

        /// <summary>
        /// 获取最大Y坐标
        /// </summary>
        public double MaxY
        {
            get { return _MaxY; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="index">点的索引</param>
        /// <returns></returns>
        public moPoint GetItem(Int32 index)
        {
            return _Points[index];
        }


        public void SetItem(Int32 index, moPoint value)
        {
            _Points[index] = value;
        }

        /// <summary>
        /// 将点添加到末尾
        /// </summary>
        /// <param name="point">点</param>
        public void Add(moPoint point)
        {
            _Points.Add(point);
        }

        /// <summary>
        /// 将指定数组中的元素添加到末尾
        /// </summary>
        /// <param name="points">点的数组</param>
        public void AddRange(moPoint[] points)
        {
            _Points.AddRange(points);
        }
        /// <summary>
        /// 将指定元素插入到索引处
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        public void Insert(Int32 index,moPoint point)
        {
            _Points.Insert(index, point);
        }

        public void InsertRange(Int32 index,moPoint[] points)
        {
            _Points.InsertRange(index, points);
        }

        public void RemoveAt(Int32 index)
        {
            _Points.RemoveAt(index);
        }

        public moPoint[] ToArray()
        {
            return _Points.ToArray();
        }

        public void Clear()
        {
            _Points.Clear();
        }

        public moRectangle GetEnvelope()
        {
            moRectangle sRect = new moRectangle(_MinX, _MaxX, _MinY, _MaxY);
            return sRect;
        }

        public void UpdateExtent()
        {
            CalExtent();
        }

        public moPoints Clone()
        {
            moPoints sPoints = new moPoints();
            Int32 sPointCount = _Points.Count;
            for (Int32 i = 0; i <= sPointCount - 1; i++)
            {
                moPoint sPoint = new moPoint(_Points[i].X, _Points[i].Y);
                sPoints.Add(sPoint);
            }
            sPoints._MinX = _MinX;
            sPoints._MaxX = _MaxX;
            sPoints._MinY = _MinY;
            sPoints._MaxY = _MaxY;
            return sPoints;
        }
        #endregion

        #region 私有函数

        private void CalExtent()
        {
            double sMinX = double.MaxValue;
            double sMaxX = double.MinValue;
            double sMinY = double.MaxValue;
            double sMaxY = double.MinValue;
            Int32 sPointCount = _Points.Count;
            for (Int32 i = 0; i <= sPointCount - 1; i++)
            {
                if (_Points[i].X < sMinX)
                    sMinX = _Points[i].X;
                if (_Points[i].X > sMaxX)
                    sMaxX = _Points[i].X;
                if (_Points[i].Y < sMinY)
                    sMinY = _Points[i].Y;
                if (_Points[i].Y > sMaxY)
                    sMaxY = _Points[i].Y;
            }
            _MinX = sMinX;
            _MaxX = sMaxX;
            _MinY = sMinY;
            _MaxY = sMaxY;
        }

        #endregion
    }
}
