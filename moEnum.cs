using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMapObjects
{
    /// <summary>
    /// 值类型常数
    /// </summary>
    public enum moValueTypeConstant
    {
        dint16=0,
        dint32=1,
        dint64=2,
        dSingle=3,
        dDouble=4,
        dText=5
    }

    /// <summary>
    /// 符号类型常数
    /// </summary>
    public enum moSymbolTypeConstant
    {
        SimpleMarkerSymbol=0,
        SimpleLineSymbol=1,
        SimpleFillSymbol=2
    }

    

    public enum moSimpleMarkerSymbolStyleConstant
    {
        Circle=0,
        SolidCircle=1,
        Triangle=2,
        SolidTriangle=3,
        Square=4,
        SolidSquare=5,
        CircleDot=6,
        CircleCircle=7
    }

    public enum moSimpleLineSymbolStyleConstant
    {
        Solid=0,
        Dash=1,
        Dot=2,
        DashDot=3,
        DashDotDot=4
    }

    /// <summary>
    /// 几何类型常数
    /// </summary>
    public enum moGeometryTypeConstant
    {
        Point=0,
        MultiPolyline=1,
        MultiPolygon=2
    }

    /// <summary>
    /// 渲染类型常数
    /// </summary>
    public enum moRendererTypeConstant
    {
        Simple=0,
        UniqueValue=1,
        ClassBreaks=2
    }

    public enum moTextSymbolAlignmentConstant
    {
        TopLeft=0,
        TopCenter=1,
        TopRight=2,
        CenterLeft=3,
        CenterCenter=4,
        CenterRight=5,
        BottomLeft=6,
        BottomCenter=7,
        BottomRight=8
    }

}
