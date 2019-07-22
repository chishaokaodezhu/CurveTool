using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace CurveMonitor.src.UI
{
    /// <summary>
    /// CodeEditor.xaml 的交互逻辑
    /// </summary>
    public partial class CodeEditor : Window
    {
        private class VirtualChannel
        {
            public string channelName = "VirtualChannel";
            public string lastUsableCode = null;
            public string presentCode = null;
            public Assembly lastUsableAssembly = null;

            public string Build()
            {
                return null;
            }

            public string Update()
            {
                return null;
            }
        }

        /*最多允许16个元素*/
        private List<VirtualChannel> virtualChannels = new List<VirtualChannel>();
        private VirtualChannel virtualChannel = null;
        private Session.Session session = null;

        public CodeEditor()
        {
            InitializeComponent();
        }

        public void BindOpHandler(Session.Session s)
        {
            this.session = s;
        }

        private void updateVirutalChannelList()
        {

        }

        private void NewChannel(){}

        private void RemoveChannel(VirtualChannel vc){}
    }
}
