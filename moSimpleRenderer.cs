using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public class moSimpleRenderer : moRenderer
    {
        #region 字段

        private moSymbol _Symbol;
        
        #endregion 
        
        #region 构造函数

        public moSimpleRenderer()
        {}
        #endregion
        

        #region 属性

        public override moRendererTypeConstant RendererType
        {
            get
            {
                return moRendererTypeConstant.Simple;
            }
        }

        public moSymbol Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }

        #endregion


        #region 方法

        public override moRenderer Clone()
        {
            moSimpleRenderer sRenderer = new moSimpleRenderer();
            sRenderer._Symbol = _Symbol.Clone();
            return sRenderer;
        }

        #endregion

        
    }
}
