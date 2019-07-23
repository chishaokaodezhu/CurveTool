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
using CurveMonitor.src.DataPump;

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
                DataPump dataPump = new DataPump();
                CurveWindow cw = new CurveWindow();
                CodeEditor ce = new CodeEditor();
                Session session = new Session();
                session.dataProvider = dp;
                session.portPannel = pp;
                session.dataPump = dataPump;
                session.curveWindow = cw;
                session.codeEditor = ce;

                this.pannelList.Items.Add(pp);
                /*
                double[] data = new double[2] { 23, 45 };
                for(int i = 0; i < 1600; i++)
                {
                    cw.DeliverData(data);
                }
                */
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Session.CloseAllSession();
            var windows = Application.Current.Windows;
        }
    }
}
