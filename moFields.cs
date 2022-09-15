using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moFields
    {
        #region 字段

        private List<moField> _Fields;
        private string _PrimaryField;   //主字段名称
        private bool _ShowAlias = false;
        
        #endregion

        #region 构造函数

        public moFields()
        {
            _Fields = new List<moField>();
        }

        #endregion
  
        #region 属性

        public Int32 Count
        {
            get { return _Fields.Count; }
        }

        public string PrimaryField
        {
            get { return _PrimaryField; }
            set { _PrimaryField = value; }
        }

        public List<moField> Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }

        public bool ShowAlias
        {
            get { return _ShowAlias; }
            set { _ShowAlias = value; }
        }


        #endregion

        #region 方法

        public moField GetItem(Int32 index)
        {
            return _Fields[index];
        }

        public moField GetItem(string name)
        {
            Int32 sindex = FindField(name);
            if (sindex >= 0)
                return _Fields[sindex];
            else
                return null; 
        }

        /// <summary>
        /// 根据名称查找索引号，无则return -1
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Int32 FindField(string name)
        {
            Int32 sFieldCount = _Fields.Count;
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                if (_Fields[i].Name.ToLower() == name.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }

        public void Append(moField field)
        {
            if (FindField(field.Name) >= 0) 
            {
                throw new Exception ( "Fields对象中不能存在重名的字段!" );    
            }
            _Fields.Add(field);
            if (FieldAppended != null)
                FieldAppended(this, field);

        }

        public void RemoveAt(Int32 index)
        {
            moField sField = _Fields[index];
            _Fields.RemoveAt(index);

            if(FieldRemoved!=null)
            {
                FieldRemoved(this, index, sField);
            }



        }

        #endregion

        #region 事件

        internal delegate void FieldAppendedHandle(object sender, moField fieldAppended);
        internal event FieldAppendedHandle FieldAppended;

        internal delegate void FieldRemovedHandle(object sender, Int32 fieldindex, moField fieldRemoved);
        internal event FieldRemovedHandle FieldRemoved;

        #endregion
    }
}
