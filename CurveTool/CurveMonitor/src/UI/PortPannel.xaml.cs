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
        private Session.Session handler = null;
        public void BindOpHandler(Session.Session h)
        {
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
                try
                {
                    this.handler.OpenDataPort();
                    this.isPortOpen = true;
                    this.portSwitch.Source = openImg;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else
            {
                try
                {
                    this.handler.CloseDataPort();
                    this.isPortOpen = false;
                    this.portSwitch.Source = closeImg;
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message);
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

            try
            {
                this.handler.StoreDataFlowCtrl(!this.isStoreEn);
                this.isStoreEn = !this.isStoreEn;
                if (this.isStoreEn)
                {
                    this.storeLine.Stroke = Brushes.DarkGoldenrod;
                    this.storeSwitch.Source = stopImg;
                }
                else
                {
                    this.storeLine.Stroke = Brushes.Gray;
                    this.storeSwitch.Source = startImg;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void ShowSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.handler == null)
            {
                return;
            }
            try
            {
                this.handler.ShowDataFlowCtrl(!this.isDrawEn);
                this.isDrawEn = !this.isDrawEn;
                if (this.isDrawEn)
                {
                    this.showLine.Stroke = Brushes.DarkGoldenrod;
                    this.showSwitch.Source = stopImg;
                }
                else
                {
                    this.showLine.Stroke = Brushes.Gray;
                    this.showSwitch.Source = startImg;
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
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
                    this.handler.UpdateStoreFile(this.fileName);
                }
            }
        }

        private void CurveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.handler == null)
            {
                return;
            }

            this.handler.CurveWindowCtrl(!this.isWinShow);
            this.isWinShow = !this.isWinShow;
        }
    }
}
