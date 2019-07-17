/*
 *  此类对应着曲线显示窗口。该窗口包含了曲线绘制视图、交互控件等。该窗口可以
 *  单独被创建，也可以新建一个线程，然后在新线程中创建。建议使用后一种方式，
 *  因为当曲线绘制时如果数据点较多时绘制可能占用大量CPU资源。
 */
using System;
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
        public CurveWindow()
        {
            InitializeComponent();
        }

        /*
         * 外部通过此方法向窗口添加曲线数据
         */
        public void DeliverData(float[] data)
        {

        }

        /*
         * 外部通过此方法进行曲线的绘制动作。该方法会调用本窗口的Dispatcher来
         * 处理绘制任务，让UI线程执行ReDraw()。如果本窗口是由非主UI线程创建的，
         * 那么ReDraw()将会在此线程中被执行。
         */
        public void CurveUpdate()
        {
            
        }

        private void ReDraw()
        {

        }
    }
}
