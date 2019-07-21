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
using CurveMonitor.src.UI;
using System.Collections;
using CurveMonitor.src.Session;

namespace CurveMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            LoadPluginToMenu();

            /*
            PluginLoader pl = new PluginLoader();
            pl.LoadPlugins();
            DataProvider dp = pl.NewPluginInstance("通用串口");
            string name = dp.PluginName();
            DataProvider dp1 = pl.NewPluginInstance("通用串口");
            if (dp.Equals(dp1))
            {
                string s = "y";
            }
            
            this.pannelList.Items.Add(new PortPannel());
            this.pannelList.Items.Add(new PortPannel());
            */
        }

        Hashtable pmHt = new Hashtable();
        private void LoadPluginToMenu()
        {
            PluginLoader pl = PluginLoader.Instance();
            pl.LoadPlugins();

            string[] pluginNames = pl.PluginNames();
            for (int i = 0; i < pluginNames.Length; i++)
            {
                MenuItem newItem = new MenuItem();
                newItem.Header = pluginNames[i];
                newItem.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.PortOpen));
                pmHt.Add(newItem, pluginNames[i]);
                this.portMenuList.Items.Add(newItem);
            }
        }

        private void PortOpen(object sender, RoutedEventArgs e)
        {
            string portType = (string)pmHt[sender];
            DataProvider dp = PluginLoader.Instance().NewPluginInstance(portType);
            if (dp.Open() >= 0)
            {
                PortPannel pp = new PortPannel();
                this.pannelList.Items.Add(pp);
                Session session = new Session();
                session.dataProvider = dp;
                session.portPannel = pp;
            }
        }
    }
}
