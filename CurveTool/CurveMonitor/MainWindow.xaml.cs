using CurveMonitor.src.Graph;
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
using SerialPortPlugin;
using CurveMonitor.src.Plugin;
using PluginPort;

namespace CurveMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        CurveWindow cw = new CurveWindow();
        public MainWindow()
        {
            InitializeComponent();
            cw.Show();

            PluginLoader pl = new PluginLoader();
            pl.LoadPlugins();
            DataProvider dp = pl.NewPluginInstance("通用串口");
            string name = dp.PluginName();
            DataProvider dp1 = pl.NewPluginInstance("通用串口");
            if (dp.Equals(dp1))
            {
                string s = "y";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for(double a = 0; a < 100; a++)
            {
                double[] data = {Math.Sin(a), Math.Sin(a+1) };
                this.cw.DeliverData(data);
            }

            this.cw.CurveUpdate();
        }
    }
}
