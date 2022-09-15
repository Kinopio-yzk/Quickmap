using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moAttributes
    {
        #region 字段

        private List<Object> _Attributes;

        #endregion

        #region 构造函数

        public  moAttributes()
        {
            _Attributes = new List<object>();
        }

        #endregion

        #region 属性

        public Int32 Count
        {
            get { return _Attributes.Count(); }
        }

        #endregion

        #region 方法



        public object GetItem(Int32 index)
        {
            return _Attributes[index];
        }

        public void SetItem(Int32 index,Object value)
        {
            _Attributes[index] = value;
        }

        public object[] ToArray()
        {
            return _Attributes.ToArray();
        }

        public void FromArray(object[] value)
        {
            _Attributes.Clear();
            _Attributes.AddRange(value);
        }

        public void Append(object value)
        {
            _Attributes.Add(value);
        }

        public void RemoveAt(Int32 index)
        {
            _Attributes.RemoveAt(index);
        }

        public moAttributes Clone()
        {
            moAttributes sAttributes = new moAttributes();
            sAttributes._Attributes.AddRange(_Attributes);
            return sAttributes;
        }


        #endregion

        #region



        #endregion

    }
}
