using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurveMonitor.src.UI
{
    /// <summary>
    /// PortPannel.xaml 的交互逻辑
    /// </summary>
    public partial class PortPannel : UserControl
    {
        public enum OpType
        {
            PORT_OPEN, PORT_CLOSE,
            STORE_EN, STORE_DISABLE,
            SHOW_EN, SHOW_DISABLE,
            FILE_SELECTED,
            W_SHOW,
            W_CLOSE
        }

        private Object obj = null;
        private PortUIOp handler = null;
        public void BindOpHandler(Object oj, PortUIOp h)
        {
            this.obj = oj;
            this.handler = h;
        }

        private bool isPortOpen = false;
        private bool isStoreEn = false;
        private bool isDrawEn = false;
        private bool isWinShow = false;
        private string fileName = null;

        public string storeFileName()
        {
            return fileName;
        }

        public PortPannel()
        {
            InitializeComponent();
        }

        private static BitmapImage openImg = new BitmapImage(new Uri("icons/port_open.png", UriKind.Relative));
        private static BitmapImage closeImg = new BitmapImage(new Uri("icons/port_close.png", UriKind.Relative));
        private void PortSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.handler == null)
            {
                return;
            }

            if (!this.isPortOpen)
            {
                if (this.handler.UIOp(obj, OpType.PORT_OPEN))
                {
                    this.isPortOpen = true;
                    this.portSwitch.Source = openImg;
                }
            }
            else
            {
                if (this.handler.UIOp(obj, OpType.PORT_CLOSE))
                {
                    this.isPortOpen = false;
                    this.portSwitch.Source = closeImg;
                }
            }
        }

        private static BitmapImage startImg = new BitmapImage(new Uri("icons/start.png", UriKind.Relative));
        private static BitmapImage stopImg = new BitmapImage(new Uri("icons/stop.png", UriKind.Relative));
        private void StoreSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.handler == null)
            {
                return;
            }

            if (!this.isStoreEn)
            {
                if (this.handler.UIOp(obj, OpType.STORE_EN))
                {
                    this.isStoreEn = true;
                    this.storeLine.Stroke = Brushes.DarkGoldenrod;
                    this.storeSwitch.Source = stopImg;
                }
            }
            else
            {
                if (this.handler.UIOp(obj, OpType.STORE_DISABLE))
                {
                    this.isStoreEn = false;
                    this.storeLine.Stroke = Brushes.Gray;
                    this.storeSwitch.Source = startImg;
                }
            }
        }

        private void ShowSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.handler == null)
            {
                return;
            }

            if (!this.isDrawEn)
            {
                if (this.handler.UIOp(obj, OpType.SHOW_EN))
                {
                    this.isDrawEn = true;
                    this.showLine.Stroke = Brushes.DarkGoldenrod;
                    this.showSwitch.Source = stopImg;
                }
            }
            else
            {
                if (this.handler.UIOp(obj, OpType.SHOW_DISABLE))
                {
                    this.isDrawEn = false;
                    this.showLine.Stroke = Brushes.Gray;
                    this.showSwitch.Source = startImg;
                }
            }
        }

        private void FileSelct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            //设置这个对话框的起始保存路径
            sfd.InitialDirectory = @"D:\";
            //设置保存的文件的类型，注意过滤器的语法
            sfd.Filter = "CSV|*.cssv|PY|*.py";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (sfd.ShowDialog() == true)
            {
                this.fileName = sfd.FileName;
                if(this.handler != null)
                {
                    this.handler.UIOp(obj, OpType.FILE_SELECTED);
                }
            }
        }

        private void CurveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.handler == null)
            {
                return;
            }

            if (!this.isWinShow)
            {
                this.isWinShow = true;
                this.handler.UIOp(obj, OpType.W_SHOW);
            }
            else
            {
                this.isWinShow = false;
                this.handler.UIOp(obj, OpType.W_CLOSE);
            }
        }
    }
}
