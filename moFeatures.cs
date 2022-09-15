using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moFeatures
    {
        #region 字段

        private List<moFeature> _Features;

        #endregion


        #region 构造函数

        public moFeatures()
        {
            _Features = new List<moFeature>();
        }


        #endregion


        #region 属性

        public Int32 Count
        {
            get { return _Features.Count(); }
        }




        #endregion


        #region 方法

        public List<moFeature> Get()
        {
            return _Features;
        }

        public moFeature GetItem(Int32 index)
        {
            return _Features[index];
        }

        public void SetGeometry(Int32 index,moGeometry geometry)
        {
            _Features[index].Geometry = geometry;
        }

        public void Add(moFeature feature)
        {
            _Features.Add(feature);
        }

        public void RemoveAt(Int32 index)
        {
            _Features.RemoveAt(index);
        }

        public void Remove(moFeature feature)
        {
            _Features.Remove(feature);
        }

        public void Clear()
        {
            _Features.Clear();
        }

        public int Search(moFeature feature)
        {
            int index = 0;
            for (index = 0; index < _Features.Count; index++)
            {
                if (feature == _Features[index])
                    return index;
            }
            return -1;
        }

        #endregion
    }
}
