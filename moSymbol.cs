using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    public abstract class moSymbol
    {
        #region 

        /// <summary>
        /// 抽象属性，获取符号类型
        /// </summary>
        public abstract moSymbolTypeConstant SymbolType { get; }

        /// <summary>
        /// 抽象方法，复制
        /// </summary>
        /// <returns></returns>
        public abstract moSymbol Clone();


        #endregion

    }
}
