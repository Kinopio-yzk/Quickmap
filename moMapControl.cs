using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyMapObjects
{
    public partial class moMapControl : UserControl
    {
        #region 字段

        //设计时属性变量
        private Color _SelectionColor = Color.Cyan;
        private Color _FlashColor = Color.Green;

        //运行时属性变量
        private moLayers _Layers = new moLayers(); //图层集合

        //模块级变量
        private moMapDrawingReference mMapDrawingReference; //地图屏幕坐标转换对象
        private Bitmap mBufferMap1 = new Bitmap(10, 10); //缓冲位图，含要素及其注记
        private Bitmap mBufferMap2 = new Bitmap(10, 10); //缓冲位图，含要素（包括注记）及跟踪图形
        private Bitmap mBufferMap3 = new Bitmap(10, 10); //在屏幕上移动Bitmap2时，先将2绘制到3上，再将3绘制到控件上，以避免清除边界带来的闪烁

        private moSimpleMarkerSymbol mSelectedPointSymbol;   //选择的点符号
        private moSimpleLineSymbol mSelectedLineSymbol;   //选择的线符号
        private moSimpleFillSymbol mSelectedFillSymbol;   //选择的面符号
        private moShapeFlashControler mFlashControler = new moShapeFlashControler(); //闪烁控制器
        private moSimpleMarkerSymbol mFlashPointSymbol;   
        private moSimpleLineSymbol mFlashLineSymbol;   
        private moSimpleFillSymbol mFlashFillSymbol;   
        #endregion

        #region 构造函数

        public moMapControl()
        {
            CreateMapDrawingReference();//地图-屏幕坐标转换对象
            InitializeComponent();
            ResizeBufferMap();//调整缓冲位图尺寸

            InitializeSymbols(); //初始化符号

            mFlashControler.NeedDrawFlashShapes += MFlashControler_NeedDrawFlashShapes;
            mFlashControler.NeedClearFlashShapes += MFlashControler_NeedClearFlashShapes;
            
        }

        
        #endregion

        #region 属性
        [Browsable(true),Description("获取或设置选择颜色")]
        public Color SelectionColor
        {
            get { return _SelectionColor; }
            set { _SelectionColor = value; }
        }

        [Browsable(true), Description("获取或设置闪烁图形颜色")]
        public Color FlashColor
        {
            get { return _FlashColor; }
            set { _FlashColor = value; }
        }


        /// <summary>
        /// 获取或设置图层集合
        /// </summary>
        [Browsable(false)]
        public moLayers Layers
        {
            get { return _Layers; }
            set { _Layers = value; }
        }

        

        /// <summary>
        /// 获取比例尺倒数
        /// </summary>
        [Browsable(false)]
        public double MapScale
        {
            get { return mMapDrawingReference.MapScale; }
        }

        /// <summary>
        /// 获取控件左上点的X坐标（地图坐标）
        /// </summary>
        [Browsable(false)]
        public double MapOffsetX
        {
            get { return mMapDrawingReference.OffsetX; }
        }

        [Browsable(false)]
        public double MapOffsetY
        {
            get { return mMapDrawingReference.OffsetY; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 获取地图窗口对应的地图范围（地图坐标）
        /// </summary>
        /// <returns></returns>
        public moRectangle GetExtent()
        {
            //定义变量
            double sMinX = double.MaxValue, sMaxX = double.MinValue;
            double sMinY = double.MaxValue, sMaxY = double.MinValue;
            moRectangle sExtent;
            //如果工作区为空，则返回空矩形
            Rectangle sClientRect = this.ClientRectangle;
            if (sClientRect.IsEmpty == true)
            {
                sExtent = new moRectangle(sMinX, sMaxX, sMinY, sMaxY);
                return sExtent;
            }
            //定义工作区左上点和右下点的屏幕坐标
            moPoint sTopLeftScreenPoint = new moPoint(0, 0);
            moPoint sBottomRightScreenPoint = new moPoint(sClientRect.Width, sClientRect.Height);
            
            
            //获取工作区左上点和右下点的地图坐标
            moPoint sTopLeftMapPoint = mMapDrawingReference.ToMapPoint(sTopLeftScreenPoint.X, sTopLeftScreenPoint.Y);
            moPoint sBottomRightMapPoint = mMapDrawingReference.ToMapPoint(sBottomRightScreenPoint.X, sBottomRightScreenPoint.Y);
            //定义范围矩形
            sMinX = sTopLeftMapPoint.X;
            sMaxX = sBottomRightMapPoint.X;
            sMinY = sBottomRightMapPoint.Y;
            sMaxY = sTopLeftMapPoint.Y;
            sExtent = new moRectangle(sMinX, sMaxX, sMinY, sMaxY);
            return sExtent;
        }

        /// <summary>
        /// 获取整个地图的范围
        /// </summary>
        /// <returns></returns>
        public moRectangle GetFullExtent()
        {
            //（1）新建一个空矩形
            double sMinX = double.MaxValue, sMaxX = double.MinValue;
            double sMinY = double.MaxValue, sMaxY = double.MinValue;
            moRectangle sFullExtent;
            //（2）如果图层数量为0，则返回空矩形
            Int32 sLayerCount = _Layers.Count;
            if (sLayerCount == 0)
            {
                sFullExtent = new moRectangle(sMinX, sMaxX, sMinY, sMaxY);
                return sFullExtent;
            }
            //（3）计算范围矩形
            for (Int32 i = 0; i <= sLayerCount - 1; i++)
            {
                moMapLayer sLayer = _Layers.GetItem(i);
                moRectangle sExtent = sLayer.Extent;
                if (sExtent.IsEmpty == false)
                {
                    if (sExtent.MinX < sMinX)
                        sMinX = sExtent.MinX;
                    if (sExtent.MaxX > sMaxX)
                        sMaxX = sExtent.MaxX;
                    if (sExtent.MinY < sMinY)
                        sMinY = sExtent.MinY;
                    if (sExtent.MaxY > sMaxY)
                        sMaxY = sExtent.MaxY;
                }
            }
            sFullExtent = new moRectangle(sMinX, sMaxX, sMinY, sMaxY);
            return sFullExtent;
        }

        //全图显示
        public void FullExtent()
        {
            moRectangle sFullExtent = GetFullExtent();
            if(sFullExtent.IsEmpty==false)
            {
                Rectangle sClientRect = this.ClientRectangle;
                mMapDrawingReference.ZoomExtentToWindow(sFullExtent, sClientRect.Width, sClientRect.Height);
                this.UseWaitCursor = true;
                DrawBufferMap1();
                DrawBufferMap2();
                this.UseWaitCursor = false;
                Refresh();
                if (MapScaleChanged != null)
                    MapScaleChanged(this);
            }
        }

        public void ToExtent(moRectangle rectangle)
        {
            if (rectangle.IsEmpty == false)
            {
                Rectangle sClientRect = this.ClientRectangle;
                mMapDrawingReference.ZoomExtentToWindow(rectangle, sClientRect.Width, sClientRect.Height);
                this.UseWaitCursor = true;
                DrawBufferMap1();
                DrawBufferMap2();
                this.UseWaitCursor = false;
                Refresh();
                if (MapScaleChanged != null)
                    MapScaleChanged(this);
            }
        }

        public void ZoomByCenter(moPoint center,double ratio)
        {
            mMapDrawingReference.ZoomByCenter(center, ratio);
            this.UseWaitCursor = true;
            DrawBufferMap1();
            DrawBufferMap2();
            this.UseWaitCursor = false;
            Refresh();
            //触发事件
            if (MapScaleChanged != null)
                MapScaleChanged(this);
        }

        public void ZoomToExtent(moRectangle extent)
        {
            double sWindowWidth = this.ClientRectangle.Width;
            double sWindowHeight = this.ClientRectangle.Height;
            mMapDrawingReference.ZoomExtentToWindow(extent, sWindowWidth, sWindowHeight);
            this.UseWaitCursor = true;
            DrawBufferMap1();
            DrawBufferMap2();
            this.UseWaitCursor = false;
            Refresh();
            //触发事件
            if (MapScaleChanged != null)
                MapScaleChanged(this);
        }

        //平移
        public void PanDelta(double deltaX, double deltaY)
        {
            mMapDrawingReference.PanDelta(deltaX, deltaY);
            this.UseWaitCursor = true;
            DrawBufferMap1();
            DrawBufferMap2();
            this.UseWaitCursor = false;
            Refresh();
        }

        //将屏幕坐标转换为地图坐标
        public moPoint ToMapPoint(double x,double y)
        {
            moPoint sPoint = mMapDrawingReference.ToMapPoint(x, y);
            return sPoint;
        }

        public moPoint FromMapPoint(double x,double y)
        {
            moPoint sPoint = mMapDrawingReference.FromMapPoint(x, y);
            return sPoint;
        }

        public double ToMapDistance(double dis)
        {
            double sDis = mMapDrawingReference.ToMapDistance(dis);
            return sDis;
        }

        public double FromMapDistance(double dis)
        {
            double sDis = mMapDrawingReference.FromMapDistance(dis);
            return sDis;
        }


        public void RedrawMap()
        {
            this.UseWaitCursor = true;
            DrawBufferMap1();
            DrawBufferMap2();
            this.UseWaitCursor = false;
            Refresh();
        }

        public void RedrawTrackingShapes()
        {
            this.UseWaitCursor = true;
            DrawBufferMap2();
            this.UseWaitCursor = false;
            Refresh();
        }

        /// <summary>
        /// 将地图移动到制定位置（屏幕坐标）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PanMapImageTo(float x,float y)
        {
            Graphics g = Graphics.FromImage(mBufferMap3);
            g.Clear(Color.White);
            g.DrawImage(mBufferMap2, x, y);
            g = Graphics.FromHwnd(this.Handle);
            g.DrawImage(mBufferMap3, 0, 0);
            g.Dispose();
        }

        public void FlashShapes(moShape[] shapes,Int32 times,Int32 interval)
        {
            mFlashControler.StartFlash(shapes, times, interval);
        }

        public moUserDrawingTool GetDrawingTool()
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            moUserDrawingTool sDrawingTool = CreateDrawingTool(g);
            return sDrawingTool;
        }

        public void SelectByBox(moRectangle selectingBox,double tolerance,Int32 selectMethod)
        {
            Int32 sLayerCount = _Layers.Count;
            for (Int32 i = 0; i <= sLayerCount - 1; i++) 
            {
                moMapLayer sLayer = _Layers.GetItem(i); 
                if (sLayer.Visible == true && sLayer.Selectable == true) 
                {
                    moFeatures sFeatures = sLayer.SearchByBox(selectingBox, tolerance);
                    sLayer.ExecuteSelect(sFeatures, selectMethod);
                }
                else
                {
                    sLayer.SelectedFeatures.Clear();
                }
            }
        }

        public moFeature SelectByBoxReturnFeature(moRectangle selectingBox, double tolerance, Int32 selectMethod)
        {
            moFeature selected = null;
            Int32 sLayerCount = _Layers.Count;
            for (Int32 i = 0; i <= sLayerCount - 1; i++)
            {
                moMapLayer sLayer = _Layers.GetItem(i);
                if (sLayer.Visible == true && sLayer.Selectable == true)
                {
                    moFeatures sFeatures = sLayer.SearchByBox(selectingBox, tolerance);
                    if(sFeatures.Count>0)
                    {
                        selected = sFeatures.GetItem(0);
                        return selected;
                    }
                    
                }
               
            }
            return selected;
        }
        #endregion

        #region 事件

        /// <summary>
        /// 比例尺发生了变化
        /// </summary>
        /// <param name="sender"></param>
        public delegate void MapScaleChangedHandle(object sender);
        public event MapScaleChangedHandle MapScaleChanged;

        /// <summary>
        /// 跟踪图形绘制完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="drawTool"></param>
        public delegate void AfterTrackingLayerDrawHandle(object sender, moUserDrawingTool drawTool);
        public event AfterTrackingLayerDrawHandle AfterTrackingLayerDraw;

        #endregion

        #region 母版和对象事件处理

        private void moMapControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mBufferMap2, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);

        }

        private void moMapControl_Resize(object sender, EventArgs e)
        {
            ResizeBufferMap();
            RedrawMap();
        }

        #endregion

        #region 私有函数

        //创建地图-屏幕坐标转换对象
        private void CreateMapDrawingReference()
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            double dpm = g.DpiX / 0.0254;
            g.Dispose();
            double mpu = 1.0;
            mMapDrawingReference = new moMapDrawingReference(0, 0, 1000000, dpm, mpu);
        }

        //调整缓冲位图大小
        private void ResizeBufferMap()
        {
            
            Rectangle sClientRectangle = this.ClientRectangle;
            if (sClientRectangle.Width > 0 && sClientRectangle.Height > 0) 
            {
                if (sClientRectangle.Width != mBufferMap1.Width || sClientRectangle.Height != mBufferMap1.Height)
                {
                    mBufferMap1 = new Bitmap(sClientRectangle.Width, sClientRectangle.Height);
                    mBufferMap2 = new Bitmap(sClientRectangle.Width, sClientRectangle.Height);
                    mBufferMap3 = new Bitmap(sClientRectangle.Width, sClientRectangle.Height);
                }
            }

        }

        private void InitializeSymbols()
        {
            //选择点符号
            mSelectedPointSymbol = new moSimpleMarkerSymbol();
            mSelectedPointSymbol.Color = _SelectionColor;
            mSelectedPointSymbol.Size = 3;
            //选择线符号
            mSelectedLineSymbol = new moSimpleLineSymbol();
            mSelectedLineSymbol.Color = _SelectionColor;
            mSelectedLineSymbol.Size = 3000 / mMapDrawingReference.dpm;
            //选择面符号
            mSelectedFillSymbol = new moSimpleFillSymbol();
            mSelectedFillSymbol.Color = Color.Transparent;
            mSelectedFillSymbol.Outline.Color = _SelectionColor;
            mSelectedFillSymbol.Outline.Size = 3000 / mMapDrawingReference.dpm;
            //闪烁点符号
            mFlashPointSymbol = new moSimpleMarkerSymbol();
            mFlashPointSymbol.Color = _FlashColor;
            mFlashPointSymbol.Style = moSimpleMarkerSymbolStyleConstant.SolidCircle;
            mFlashPointSymbol.Size = 3;
            //闪烁线符号
            mFlashLineSymbol = new moSimpleLineSymbol();
            mFlashLineSymbol.Color = _FlashColor;
            mFlashLineSymbol.Size = 1.5;
            //闪烁面符号
            mFlashFillSymbol = new moSimpleFillSymbol();
            mFlashFillSymbol.Color = _FlashColor;
            mFlashFillSymbol.Outline.Color = Color.Black;
            mFlashFillSymbol.Outline.Size = 0.35;
        }

        public void SaveBitmap()
        {
            
            if (mBufferMap1 == null)
                MessageBox.Show("Failed：尚未打开或生成栅格图形！");
            else
            {
                try
                {
                    SaveFileDialog safefile = new SaveFileDialog();
                    safefile.OverwritePrompt = true;
                    safefile.RestoreDirectory = true;
                    safefile.Filter = "bmp文件|*.bmp";
                    if (safefile.ShowDialog() == DialogResult.OK)
                    {
                        string path = safefile.FileName;
                        mBufferMap1.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
                        MessageBox.Show("保存成功");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        //绘制缓冲位图1
        public void DrawBufferMap1()
        {
            //（1）获取地图窗口的范围
            moRectangle sExtent = GetExtent();
            if (sExtent.IsEmpty == true)
                return;
            //（2）绘制所有图层的要素，采用倒序
            double sMapScale = mMapDrawingReference.MapScale;
            double dpm = mMapDrawingReference.dpm;
            double mpu = mMapDrawingReference.mpu;
            Graphics g = Graphics.FromImage(mBufferMap1);
            g.Clear(Color.White);
            Int32 sLayerCount = _Layers.Count;
            for (Int32 i = sLayerCount - 1; i >= 0; i--)
            {
                moMapLayer sLayer = _Layers.GetItem(i);
                if (sLayer.Visible == true)
                {
                    sLayer.DrawFeatures(g, sExtent, sMapScale, dpm, mpu);
                }
            }
            //（3）绘制所有图层的注记，依然倒序
            List<RectangleF> sPlacedLabelExtents = new List<RectangleF>();
            for (Int32 i = sLayerCount - 1; i >= 0; i--)
            {
                moMapLayer sLayer = _Layers.GetItem(i);
                if (sLayer.Visible == true)
                {
                    sLayer.DrawLabels(g, sExtent, sMapScale, dpm, mpu, sPlacedLabelExtents);
                }
            }
            g.Dispose();
        }

        private void DrawBufferMap2()
        {
            //（1）获取地图窗口的范围
            moRectangle sExtent = GetExtent();
            if (sExtent.IsEmpty == true)
                return;
            //（2）绘制缓冲位图1
            Graphics g = Graphics.FromImage(mBufferMap2);
            g.Clear(Color.White);
            Rectangle sRect = new Rectangle(0, 0, mBufferMap1.Width, mBufferMap1.Height);
            g.DrawImage(mBufferMap1, sRect, sRect, GraphicsUnit.Pixel);
            //（3）绘制所有图层的选择要素，采用倒序
            double sMapScale = mMapDrawingReference.MapScale;
            double dpm = mMapDrawingReference.dpm;
            double mpu = mMapDrawingReference.mpu;
            Int32 sLayerCount = _Layers.Count;
            for (Int32 i = sLayerCount - 1; i >= 0; i--)
            {
                moMapLayer sLayer = _Layers.GetItem(i);
                if (sLayer.ShapeType == moGeometryTypeConstant.Point)
                {
                    sLayer.DrawSelectedFeatures(g, sExtent, sMapScale, dpm, mpu, mSelectedPointSymbol);
                }
                else if (sLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    sLayer.DrawSelectedFeatures(g, sExtent, sMapScale, dpm, mpu, mSelectedLineSymbol);
                }
                else if (sLayer.ShapeType == moGeometryTypeConstant.MultiPolygon)
                {
                    sLayer.DrawSelectedFeatures(g, sExtent, sMapScale, dpm, mpu, mSelectedFillSymbol);
                }
            }
            //（4）触发事件，以便用户程序继续绘图
            if (AfterTrackingLayerDraw != null)
            {
                //新建绘图工具
                moUserDrawingTool sDrawingTool = CreateDrawingTool(g);
                AfterTrackingLayerDraw(this, sDrawingTool);
            }
            g.Dispose();
        }

        //新建一个绘图工具
        private moUserDrawingTool CreateDrawingTool(Graphics g)
        {
            moRectangle sExtent = GetExtent();
            double sMapScale = mMapDrawingReference.MapScale;
            double dpm = mMapDrawingReference.dpm;
            double mpu = mMapDrawingReference.mpu;
            moUserDrawingTool sDrawingTool = new moUserDrawingTool(g, sExtent, sMapScale, dpm, mpu);
            return sDrawingTool;
        }



        private void MFlashControler_NeedClearFlashShapes(object sender)
        {
            RedrawTrackingShapes();
        }

        private void MFlashControler_NeedDrawFlashShapes(object sender, moShape[] shapes)
        {
            double sMapScale = mMapDrawingReference.MapScale;
            double dpm = mMapDrawingReference.dpm;
            double mpu = mMapDrawingReference.mpu;
            moRectangle sExtent = GetExtent();
            Graphics g = Graphics.FromImage(mBufferMap2);
            Int32 sShapeCount = shapes.Length;
            for (Int32 i = 0; i <= sShapeCount - 1; i++)
            {
                if (shapes[i].GetType() == typeof(moPoint))
                {
                    moPoint sPoint = (moPoint)shapes[i];
                    moMapDrawingTools.DrawPoint(g, sExtent, sMapScale, dpm, mpu, sPoint, mFlashPointSymbol);
                }
                else if (shapes[i].GetType() == typeof(moPoints))
                {
                    moPoints sPoints = (moPoints)shapes[i];
                    moMapDrawingTools.DrawPoints(g, sExtent, sMapScale, dpm, mpu, sPoints, mFlashPointSymbol);
                }
                else if (shapes[i].GetType() == typeof(moRectangle))
                {
                    moRectangle sRect = (moRectangle)shapes[i];
                    moMapDrawingTools.DrawRectangle(g, sExtent, sMapScale, dpm, mpu, sRect, mFlashFillSymbol);
                }
                else if (shapes[i].GetType() == typeof(moMultiPolyline))
                {
                    moMultiPolyline sMultiPolyline = (moMultiPolyline)shapes[i];
                    moMapDrawingTools.DrawMultiPolyline(g, sExtent, sMapScale, dpm, mpu, sMultiPolyline, mFlashLineSymbol);
                }
                else if (shapes[i].GetType() == typeof(moMultiPolygon))
                {
                    moMultiPolygon sMultiPolygon = (moMultiPolygon)shapes[i];
                    moMapDrawingTools.DrawMultiPolygon(g, sExtent, sMapScale, dpm, mpu, sMultiPolygon, mFlashFillSymbol);
                }
            }
            g.Dispose();
            Refresh();
        }

        #endregion

        
    }
}
