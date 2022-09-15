using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    /// <summary>
    /// 部分集合类
    /// </summary>
    public class moParts
    {
        #region 字段

        private List<moPoints> _Parts;

        #endregion

        #region 构造函数

        public moParts()
        {
            _Parts = new List<moPoints>();
        }

        public  moParts(moPoints[] parts)
        {
            _Parts = new List<moPoints>();
            _Parts.AddRange(parts);
        }

        #endregion

        #region 属性

        public Int32 Count
        {
            get { return _Parts.Count; }
        }

        #endregion

        #region 方法

        public moPoints GetItem(Int32 index)
        {
            return _Parts[index];
        }
        
        public void SetItem(Int32 index,moPoints part)
        {
            _Parts[index] = part;
        }

        public void Add(moPoints part)
        {
            _Parts.Add(part);
        }

        public void AddRange(moPoints [] parts)
        {
            _Parts.AddRange(parts);
        }

        public moParts Clone()
        {
            moParts sParts = new moParts();
            Int32 sPartCount = _Parts.Count;
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                moPoints sPart = _Parts[i].Clone();
                sParts.Add(sPart);
            }
            return sParts;
        }
        #endregion
    }
}
