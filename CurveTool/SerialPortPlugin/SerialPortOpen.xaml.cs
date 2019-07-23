using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace SerialPortPlugin
{
    /// <summary>
    /// SerialPortOpen.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortOpen : Window
    {
        private string[] portsName = null;
        public string selectedPort = null;
        public int baudRate = 0;

        public SerialPortOpen()
        {
            InitializeComponent();
            LoadSerialPortList();
        }

        public void LoadSerialPortList()
        {
            portsName = SerialPort.GetPortNames();
            for(int i = 0; i < portsName.Length; i++)
            {
                this.portList.Items.Add(portsName[i]);
            }
            
            if(this.portList.Items.Count > 0)
            {
                this.portList.SelectedIndex = 0;
            }
            else
            {
                this.portList.Text = "None";
                this.setBtn.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selectedPort = this.portsName[this.portList.SelectedIndex];
            baudRate = int.Parse(this.baud.Text);
            this.Close();
        }
    }
}
