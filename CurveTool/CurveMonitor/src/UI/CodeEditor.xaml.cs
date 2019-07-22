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
        private static string startCode = "" +
            "using System;\r\n" +
            "using VirtualChannel;\r\n" +
            "namespace VChannel\r\n" +
            "{\r\n" +
            "    public class VChannelPort : VirtualChannel.VirtualChannel\r\n" +
            "    {\r\n" +
            "        public double GetChannelValue(double[] input)\r\n" +
            "        {\r\n" +
            "            //add code here\r\n" +
            "        }\r\n" +
            "    }\r\n" +
            "}\r\n";

        private class VirtualChannel
        {
            public string channelName = "VirtualChannel";
            public string lastUsableCode = null;
            public string presentCode = startCode;
            public Assembly lastUsableAssembly = null;
            public TextBlock tb = null;

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
        private VirtualChannel presentChannel = null;
        private Session.Session session = null;

        public CodeEditor()
        {
            InitializeComponent();
        }

        public void BindOpHandler(Session.Session s)
        {
            this.session = s;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tb = new TextBlock();
            tb.Text = "NewChannel";

            VirtualChannel nvc = new VirtualChannel();
            nvc.channelName = "NewChannel";
            nvc.tb = tb;
            virtualChannels.Add(nvc);

            this.fileList.Items.Add(tb);
        }

        private void ChannelChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.fileList.SelectedIndex == -1)
            {
                return;
            }

            TextBlock sTb = (TextBlock)this.fileList.SelectedItems[0];
            this.fileName.Text = sTb.Text;
            this.buildBtn.IsEnabled = true;
            for(int i = 0; i < virtualChannels.Count(); i++)
            {
                if(virtualChannels[i].channelName.CompareTo(sTb.Text) == 0)
                {
                    VirtualChannel cv = virtualChannels[i];
                    this.presentChannel = cv;
                    this.codeTextBox.Text = cv.presentCode;
                    if (cv.presentCode.CompareTo(cv.lastUsableCode) == 0)
                    {
                        this.enableBtn.Content = "删除";
                    }
                    else
                    {
                        if (cv.lastUsableCode == null)
                        {
                            this.enableBtn.Content = "启用";
                        }
                        else
                        {
                            this.enableBtn.Content = "更新";
                        }
                    }
                    break;
                }
            }
            this.enableBtn.IsEnabled = true;
        }

        private void EnableBtnClick(object sender, RoutedEventArgs e)
        {
            switch (this.enableBtn.Content)
            {
                case "启用":
                    presentChannel.channelName = this.fileName.Text;
                    presentChannel.presentCode = this.codeTextBox.Text;
                    presentChannel.tb.Text = presentChannel.channelName;

                    //预编译

                    //设置更新值

                    //启用端口
                    break;

                case "更新":
                    presentChannel.channelName = this.fileName.Text;
                    presentChannel.presentCode = this.codeTextBox.Text;
                    presentChannel.tb.Text = presentChannel.channelName;

                    //预编译

                    //设置更新值

                    //删除原端口

                    //启用新端口
                    break;

                case "删除":
                    break;
            }
        }
    }
}
