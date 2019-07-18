/*
 * 本类继承自WPF提供的绘图单元，用于曲线图的显示。绘制时该类将使用DrawingVisual进行
 * 折线图的绘制，绘制完成后才会将绘制好的Visual交付给WPF绘图单元
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CurveMonitor.src.Graph
{
    public class CurveView : Canvas
    {
        /*
         * 默认最多16条曲线，每条曲线最多1024个点。当一条曲线的长度超过一定数值时，即使
         * 增加更多的点，屏幕上绘制出来的结果也几乎是一样的，即多条曲线组成了面。此时并
         * 不利于数据观察，因此该数值不宜过大，也因此在数据类型选型时选择了Double，而不
         * 是float。
         */
        private const int MAX_CURVE_NUMS = 16;
        private const int MAX_CURVE_LENGTH = 1024;

        /*
         * DrawinbVisual可以理解为一张画布，绘制时可能会使用多张画布。显示时使用一张画布。
         * 绘制时使用另一张画布，在绘制完成后将正在显示的画布换下来。
         */
        private List<Visual> visuals = new List<Visual>();
        private DrawingVisual presentVisual = null;

        private Object[] curvesData = new object[MAX_CURVE_NUMS];
        private int[] curvesDataStrartIdx = new int[MAX_CURVE_NUMS];
        private SpinLock[] curvesLock = new SpinLock[MAX_CURVE_NUMS];

        public void AppendData(double[] data)
        {
            int lineIdx = 0;
            for(lineIdx = 0; lineIdx < curvesData.Length && lineIdx < data.Length; lineIdx++)
            {
                
            }

            /* 程序支持动态增加绘制的曲线 */
            for(;lineIdx < data.Length && lineIdx < MAX_CURVE_NUMS; lineIdx++)
            {
                ConcurrentQueue<Double> q = new ConcurrentQueue<double>();
                q.Enqueue(data[lineIdx]);
                curveData.Add(q);
            }
        }




        /*
         * 曲线绘制函数实现，当前仅包含折线图实现，即DrawPolyLine()
         * 
         * 解释：
         *  systemYScale
         *  因为曲线上的点的Y值的量纲是不确定的，有的数据值可能是10000,而有的数据值可能
         *  是0.0001，这些数据在绘制时需要缩放到合适大小，不然绘制的曲线可能在窗口不可视
         *  区域。systemYScale等于0是表示系统自动计算缩放比例
         *  
         *  maxVisablePointNums
         *  虽然系统默认数据帧在时域上是等距的，系统在绘制时会自动为其定义一个自增的X轴
         *  坐标。该自增值由一条曲线的最大可视数据点确定，例如窗口宽度为1200px，最大可视
         *  数据点为100，那么X轴坐标每次自增1200/100=12。该值也可以作为系统X轴缩放参数。
         *  
         *  curvesStartX
         *  即曲线在画布上绘制的起始点。假设窗口宽度为1200px，当曲线只有一个数据点时，那
         *  么曲线的起始绘制X值为1200，当曲线数据点越来越多时该起始点将会越来越小，直到0
         *  停止，X坐标点小于0的数据点将被永久移除。
         *  另外，每条曲线的起始点可能是不同的，比如系统当前只有2条曲线正在绘制，动态地
         *  增加了第三条曲线，那么第三条曲线刚开始时的绘制起始点与前两条是不一样的。
         *  
         *  curveYScale
         *  curveXScale
         *  curvesYOffset
         *  每条曲线有自己独立的缩放参数，Y轴平移参数
         */

        private float systemYScale = 0.0f;
        private float maxVisablePointNums = MAX_CURVE_LENGTH;
        private float[] curvesYScale = null;
        private float[] curvesXScale = null;
        private double[] curvesYOffset = null;
        private float[] curvesStartX = null;
        private Color[] curvesColor = null;

        public void CurveViewInit()
        {
            curvesYScale = new float[MAX_CURVE_NUMS];
            curvesXScale = new float[MAX_CURVE_NUMS];
            curvesYOffset = new Double[MAX_CURVE_NUMS];
            curvesStartX = new float[MAX_CURVE_NUMS];
            curvesColor  = new Color[MAX_CURVE_NUMS]; 

            for(int i = 0; i < MAX_CURVE_NUMS; i++)
            {
                curvesXScale[i] = 1.0f;
                curvesYScale[i] = 1.0f;
                curvesYOffset[i] = 0.0f;
                curvesStartX[i] = 0.0f;
            }

            curvesColor[00] = Color.FromArgb(100, 0,   0,   0  );
            curvesColor[01] = Color.FromArgb(100, 255, 0,   0  );
            curvesColor[02] = Color.FromArgb(100, 0,   255, 0  );
            curvesColor[03] = Color.FromArgb(100, 0,   0,   255);
            curvesColor[04] = Color.FromArgb(100, 255, 255, 0  );
            curvesColor[05] = Color.FromArgb(100, 0,   255, 255);
            curvesColor[06] = Color.FromArgb(100, 255, 0,   255);
            curvesColor[07] = Color.FromArgb(100, 128, 128, 128);
            curvesColor[08] = Color.FromArgb(100, 128, 0,   0  );
            curvesColor[09] = Color.FromArgb(100, 0,   128, 0  );
            curvesColor[10] = Color.FromArgb(100, 0,   0,   128);
            curvesColor[11] = Color.FromArgb(100, 128, 128, 0  );
            curvesColor[12] = Color.FromArgb(100, 0,   128, 128);
            curvesColor[13] = Color.FromArgb(100, 128, 0,   128);
            curvesColor[14] = Color.FromArgb(100, 200, 200, 0  );
            curvesColor[15] = Color.FromArgb(100, 200, 128, 255);

        }

        public void DrawPolyLine()
        {
            /*
             * 确定绘制参数
             */

            int maxCurveLength = 0;
            double maxYValue = 0;
            double minYValue = 0;
            double[] points = null;

            for(int i = 0; i < curveData.Count(); i++)
            {
                int curveLen = curveData[i].Count();
                if (curveLen > maxCurveLength)
                {
                    maxCurveLength = curveLen;
                }

                if(points == null || points.Length < maxCurveLength)
                {
                    points = new double[maxCurveLength];
                }
            }

            /*
             * 曲线绘制
             */


            /*
             * 绘制结果交付
             */
        }



        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
    }
}
