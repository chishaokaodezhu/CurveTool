/*
 *  此类对应着曲线显示窗口。该窗口包含了曲线绘制视图、交互控件等。该窗口可以
 *  单独被创建，也可以新建一个线程，然后在新线程中创建。建议使用后一种方式，
 *  因为当曲线绘制时如果数据点较多时绘制可能占用大量CPU资源。
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CurveMonitor.src.Graph
{
    /// <summary>
    /// CurveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CurveWindow : Window
    {
        private class CurveCtrlBlock
        {
            public Color color;
            public float xScale;
            public float yScale;
            public double yOffset;
            public bool show;
            public Brush brush;
            public int curveIdx;
            
        }

        private Hashtable curveCtrlBlocks = new Hashtable();
        private Rectangle[] cbts = null;
        private Color[] defColors = null;

        private void BindCbts()
        {
            defColors = new Color[CurveView.MAX_CURVE_NUMS]{
                Color.FromRgb(0,0,0),       Color.FromRgb(255, 192, 203),
                Color.FromRgb(220,20,60),   Color.FromRgb(219,112,147),
                Color.FromRgb(255,105,180), Color.FromRgb(255,20,147),
                Color.FromRgb(199,21,133),  Color.FromRgb(218,112,214),
                Color.FromRgb(238,130,238), Color.FromRgb(255,0,255),
                Color.FromRgb(139,0,139),   Color.FromRgb(75,0,130),
                Color.FromRgb(119,136,153), Color.FromRgb(30,144,255),
                Color.FromRgb(135,206,235), Color.FromRgb(47,79,79),
            };

            cbts = new Rectangle[CurveView.MAX_CURVE_NUMS];

            cbts[0] = this.cbt1;
            cbts[1] = this.cbt2;
            cbts[2] = this.cbt3;
            cbts[3] = this.cbt4;
            cbts[4] = this.cbt5;
            cbts[5] = this.cbt6;
            cbts[6] = this.cbt7;
            cbts[7] = this.cbt8;
            cbts[8] = this.cbt9;
            cbts[9] = this.cbt10;
            cbts[10] = this.cbt11;
            cbts[11] = this.cbt12;
            cbts[12] = this.cbt13;
            cbts[13] = this.cbt14;
            cbts[14] = this.cbt15;
            cbts[15] = this.cbt16;

            for(int i = 0; i < cbts.Length; i++)
            {
                cbts[i].AddHandler(Rectangle.MouseLeftButtonDownEvent, new RoutedEventHandler(this.LBtnDownEvent));

                CurveCtrlBlock ccb = new CurveCtrlBlock();
                ccb.color = defColors[i];
                ccb.xScale = 1.0f;
                ccb.yOffset = 0.0f;
                ccb.yScale = 1.0f;
                ccb.show = true;
                ccb.curveIdx = i;

                curveCtrlBlocks.Add(cbts[i], ccb);
            }
        }

        public CurveWindow()
        {
            InitializeComponent();
            BindCbts();
        }

        private int lastCurveNums = 0;
        /*
         * 外部通过此方法向窗口添加曲线数据
         */
        public void DeliverData(double[] data)
        {
            this.curveView.AppendData(data);
            if(data.Length != lastCurveNums)
            {
                lastCurveNums = data.Length;
                for(int lineIdx = 0; lineIdx < data.Length; lineIdx++)
                {
                    int idx = lineIdx;
                    this.curveView.SetCurveColor(lineIdx, defColors[lineIdx]);
                    Color color = ((CurveCtrlBlock)curveCtrlBlocks[cbts[lineIdx]]).color;
                    //this.cbts[lineIdx].Fill = new SolidColorBrush(color);
                    this.Dispatcher.BeginInvoke(new Action(()=> {
                        this.cbts[idx].Fill = new SolidColorBrush(color);
                    }));
                }
            }

            CurveUpdate();
        }

        /*
         * 外部通过此方法进行曲线的绘制动作。该方法会调用本窗口的Dispatcher来
         * 处理绘制任务，让UI线程执行ReDraw()。如果本窗口是由非主UI线程创建的，
         * 那么ReDraw()将会在此线程中被执行。
         */
        public void CurveUpdate()
        {
            this.Dispatcher.BeginInvoke(new Action(()=> { this.curveView.DrawPolyLine(); }));
        }

        private void ReDraw()
        {

        }

        /*
         * 下面的代码用于控制曲线的操作，在左边的矩形框中点击一次表示选中，选中在窗口显示该曲线的
         * 控制面板。选中后开始的奇数次点击表示曲线不显示，偶数次点击表示曲线显示。
         */
        private Object LastSelectedCbts = null;
        private Brush disableBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        private void LBtnDownEvent(object sender, RoutedEventArgs arg)
        {
            CurveCtrlBlock ccb = (CurveCtrlBlock)curveCtrlBlocks[sender];

            if (sender != LastSelectedCbts)
            {
                LastSelectedCbts = sender;
                this.y_scale_txt.Text = "" + ccb.yScale;
                this.y_offset_txt.Text = "" + ccb.yOffset;
                this.x_scale_txt.Text = "" + ccb.xScale;
            }
            else
            {
                Rectangle cbtTemp = (Rectangle)sender;
                if (ccb.show)
                {
                    ccb.show = false;
                    ccb.brush = cbtTemp.Fill;
                    cbtTemp.Fill = disableBrush;
                    this.curveView.SetCurveVisable(ccb.curveIdx, false);
                }
                else
                {
                    ccb.show = true;
                    cbtTemp.Fill = ccb.brush;
                    this.curveView.SetCurveVisable(ccb.curveIdx, true);
                }

                CurveUpdate();
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if(LastSelectedCbts == null)
            {
                return;
            }

            CurveCtrlBlock ccb = (CurveCtrlBlock)curveCtrlBlocks[LastSelectedCbts];
            float y_scale = float.Parse(this.y_scale_txt.Text);
            double y_offset = float.Parse(this.y_offset_txt.Text);
            float x_scale = float.Parse(this.x_scale_txt.Text);

            ccb.yScale = y_scale;
            ccb.yOffset = y_offset;
            ccb.xScale = x_scale;

            this.curveView.SetCurveScale(ccb.curveIdx, y_scale, x_scale);
            this.curveView.SetCurveOffset(ccb.curveIdx, y_offset, 0);
            CurveUpdate();
        }

        private bool close = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!close)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public void CloseWindow()
        {
            close = true;
            this.Close();
        }
    }
}
