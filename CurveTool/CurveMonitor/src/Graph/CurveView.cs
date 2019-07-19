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
using System.Windows;
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

        /*
         * curvesData中存储着Double类型的数组，该数组用于存储曲线数据，同时该数组也会与
         * curvesDataBuf中的并发队列一同组成一个带缓存的环线队列。外部交付数据时数据被存
         * 放在curvesDataBuf中，内部更新curvesData中的环形队列时才从并发队列中取出数据。
         */
        private List<ConcurrentQueue<double>> curvesDataBuf = new List<ConcurrentQueue<double>>();
        private Object[] curvesData = new Object[MAX_CURVE_NUMS];
        private int[] curvesDataStrartIdx = new int[MAX_CURVE_NUMS];
        private int[] curvesDataNums = new int[MAX_CURVE_NUMS];
        private int maxCurvesDataNums = MAX_CURVE_LENGTH;
        

        public void AppendData(double[] data)
        {
            int lineIdx = 0;
            for(lineIdx = 0; lineIdx < curvesData.Length && lineIdx < data.Length; lineIdx++)
            {
                curvesDataBuf[lineIdx].Enqueue(data[lineIdx]);
            }

            /* 程序支持动态增加绘制的曲线 */
            for(;lineIdx < data.Length && lineIdx < MAX_CURVE_NUMS; lineIdx++)
            {
                ConcurrentQueue<double> q = new ConcurrentQueue<double>();
                q.Enqueue(data[lineIdx]);
                curvesData[lineIdx] = new double[maxCurvesDataNums];
                curvesDataStrartIdx[lineIdx] = 0;
                curvesDataNums[lineIdx] = 0;

                curvesXScale[lineIdx] = 1.0f;
                curvesYScale[lineIdx] = 1.0f;
                curvesYOffset[lineIdx] = 0.0f;
                curvesStartX[lineIdx] = 0.0f;

                byte r = (byte)(255 -  lineIdx * 16 + 1);
                byte g = (byte)(lineIdx * 16 - 1);
                byte b = (byte)(255 - r);
                curvesColor[lineIdx] = Color.FromArgb(100, r, g, b);

                curvesDataBuf.Add(q);
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
        private float[] curvesYScale = new float[MAX_CURVE_NUMS];
        private float[] curvesXScale = new float[MAX_CURVE_NUMS];
        private double[] curvesYOffset = new double[MAX_CURVE_NUMS];
        private float[] curvesStartX = new float[MAX_CURVE_NUMS];
        private Color[] curvesColor = new Color[MAX_CURVE_NUMS];

        public void DrawPolyLine()
        {
            /*
             * 确定绘制参数
             */
             
            int maxCurveLength = 0;
            double maxYValue = Double.MinValue;
            double minYValue = Double.MaxValue;

            //遍历时以环形队列的缓存数量作为遍历次数，因为动态增加曲线时，其是最后更新的量
            for(int curveIdx = 0; curveIdx < curvesDataBuf.Count(); curveIdx++)
            {
                int curveLength = this.curvesDataNums[curveIdx];

                if(curveLength > maxCurveLength)
                {
                    maxCurveLength = curveLength;
                }

                if(systemYScale != 0.0f)
                {
                    continue;
                }

                double[] points = (double[])this.curvesData[curveIdx];
                for(int pointIdx = 0; pointIdx < curveLength; pointIdx++)
                {
                    double pointV = points[pointIdx];
                    if (pointV > maxYValue)
                    {
                        maxYValue = pointV;
                    }

                    if(pointV < minYValue)
                    {
                        minYValue = pointV;
                    }
                }

            }

            float realSysYScal = systemYScale != 0.0f ? systemYScale : (float)(this.ActualHeight / (maxYValue - minYValue) * 0.8);
            float realXStep = (float)this.ActualWidth / this.maxVisablePointNums;

            /*
             * 曲线绘制
             */

            if(this.presentVisual != null)
            {
                this.RemoveVisual(presentVisual);
            }
            this.presentVisual = new DrawingVisual();
            DrawingContext dc = this.presentVisual.RenderOpen();

            for(int curveIdx = 0; curveIdx < curvesDataBuf.Count(); curveIdx++)
            {
                double lastPx = 0;
                int curveLength = this.curvesDataNums[curveIdx];
                if(curveLength < this.maxVisablePointNums)
                {
                    lastPx = (this.maxVisablePointNums - curveLength) * realXStep;
                }

                //
                Pen curvePen = new Pen(new SolidColorBrush(this.curvesColor[curveIdx]), 1);
                curvePen.Freeze();

                //确定具体绘制参数
                int pointIdx = this.curvesDataStrartIdx[curveIdx];
                double[] points = (double[])this.curvesData[curveIdx];
                float yScale = this.curvesYScale[curveIdx];
                double yOffset = this.curvesYOffset[curveIdx];
                double lastPy = points[pointIdx] * yScale * realSysYScal + yOffset;

                for (int loopTimes = 0; loopTimes < this.curvesDataNums[curveIdx]; loopTimes++)
                {
                    double px = lastPx + realXStep;
                    double py = points[pointIdx] * yScale* realSysYScal +yOffset;
                    pointIdx++;
                    pointIdx %= this.maxCurvesDataNums;

                    //daraw
                    Point p1 = new Point(lastPx, lastPy);
                    Point p2 = new Point(px, py);
                    dc.DrawLine(curvePen, p1, p2);

                    lastPx = px;
                    lastPy = py;
                }

            }
            dc.Close();


            //结果交付
            this.AddVisual(this.presentVisual);

            //数据更新
            DataUpdate();
        }

        /* 此过程必须在曲线绘制完成后被调用，且在一个线程内调用 */
        private void DataUpdate()
        {
            for(int curveIdx = 0; curveIdx < this.curvesDataBuf.Count(); curveIdx++)
            {
                double newData = 0.0f;
                while (this.curvesDataBuf[curveIdx].TryDequeue(out newData))
                {
                    int iPos = this.curvesDataStrartIdx[curveIdx] + this.curvesDataNums[curveIdx];
                    iPos %= this.maxCurvesDataNums;
                    double[] data = (double[])this.curvesData[curveIdx];
                    data[iPos] = newData;

                    this.curvesDataStrartIdx[curveIdx] += 1;
                    this.curvesDataStrartIdx[curveIdx] %= this.maxCurvesDataNums;
                    this.curvesDataNums[curveIdx] += 1;
                    if(this.curvesDataNums[curveIdx] > this.maxCurvesDataNums)
                    {
                        this.curvesDataNums[curveIdx] = this.maxCurvesDataNums;
                    }
                }
            }
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
