using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
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
            "            return 0;\r\n" +
            "        }\r\n" +
            "    }\r\n" +
            "}\r\n";

        private class VirtualChannelInfo
        {
            //用于对一个通道进行唯一标识，保证通道名修改后也能找到对应通道
            public string channelKey = null;

            //用于界面上通道名的显示，修改
            public string channelName = "VirtualChannel";
            
            public string lastUsableCode = null;
            public string presentCode = startCode;
            public Assembly lastUsableAssembly = null;
            public VirtualChannel.VirtualChannel vc = null;
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
        private List<VirtualChannelInfo> virtualChannels = new List<VirtualChannelInfo>();
        private VirtualChannelInfo presentChannel = null;
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
            for(int i = 0; i < virtualChannels.Count(); i++)
            {
                if(virtualChannels[i].channelName.CompareTo("NewChannel") == 0)
                {
                    MessageBox.Show("当前已有一个名为 NewChannel 的通道");
                    return;
                }
            }

            TextBlock tb = new TextBlock();
            tb.Text = "NewChannel";

            VirtualChannelInfo nvc = new VirtualChannelInfo();
            nvc.channelName = "NewChannel";
            nvc.channelKey = System.Guid.NewGuid().ToString();
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
                    VirtualChannelInfo cv = virtualChannels[i];
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

        private bool UpdatePresentChannelInfo()
        {
            /*尝试更新端口名*/
            for(int i = 0; i < virtualChannels.Count(); i++)
            {
                if(virtualChannels[i] == presentChannel)
                {
                    continue;
                }

                if (virtualChannels[i].channelName.CompareTo(this.fileName.Text) == 0)
                {
                    MessageBox.Show("当前已存在名为 " + this.fileName.Text + "的端口");
                    return false;
                }
            }
            presentChannel.channelName = this.fileName.Text;
            presentChannel.tb.Text = presentChannel.channelName;

            /*更新当前的代码，不管有没有编译通过*/
            presentChannel.presentCode = this.codeTextBox.Text;

            /*编译代码*/
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("lib/VirtualChannel.dll");
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;

            CompilerResults cr = codeProvider.CompileAssemblyFromSource(compilerParameters, presentChannel.presentCode);
            if (cr.Errors.HasErrors)
            {
                string errInfo = "编译时发生错误：\r\n";
                foreach(CompilerError err in cr.Errors)
                {
                    errInfo += ("line " + err.Line + ": " + err.ErrorText + "\r\n");
                }
                MessageBox.Show(errInfo);
                return false;
            }

            presentChannel.lastUsableCode = presentChannel.presentCode;
            presentChannel.lastUsableAssembly = cr.CompiledAssembly;
            Object api = null;
            foreach(Type t in presentChannel.lastUsableAssembly.GetExportedTypes())
            {
                if(t.GetInterface("VirtualChannel") != null)
                {
                    presentChannel.vc = (VirtualChannel.VirtualChannel)Activator.CreateInstance(t);
                    api = presentChannel.vc;
                }
            }

            if(api == null)
            {
                MessageBox.Show("代码中的类并非继承自VirtualChannel接口");
                return false;
            }

            return true;
        }

        private void EnableBtnClick(object sender, RoutedEventArgs e)
        {
            switch (this.enableBtn.Content)
            {
                case "启用":
                case "更新":
                    if (!UpdatePresentChannelInfo())
                    {
                        break;
                    }

                    //启用或更新端口
                    if(this.session != null)
                    {
                        this.session.SetVirtualChannel(presentChannel.channelKey, presentChannel.vc);
                    }
                    break;

                case "删除":
                    //删除指定的端口
                    this.fileList.Items.Remove(presentChannel.tb);
                    if (this.session != null)
                    {
                        this.session.SetVirtualChannel(presentChannel.channelKey, null);
                    }
                    virtualChannels.Remove(presentChannel);

                    if(virtualChannels.Count() == 0)
                    {
                        presentChannel = null;
                        this.codeTextBox.Text = "";
                        this.fileName.Text = "";
                        this.enableBtn.Content = "启用";
                        this.enableBtn.IsEnabled = false;
                        this.buildBtn.IsEnabled = false;
                    }
                    else
                    {
                        this.presentChannel = virtualChannels[0];
                        this.codeTextBox.Text = this.presentChannel.channelName;
                        this.fileName.Text = this.presentChannel.channelName;
                        this.enableBtn.Content = "更新";
                        this.fileList.SelectedItem = presentChannel.tb;
                    }
                    break;
            }
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

        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            presentChannel.presentCode = this.codeTextBox.Text;

            /*编译代码*/
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("lib/VirtualChannel.dll");
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;

            CompilerResults cr = codeProvider.CompileAssemblyFromSource(compilerParameters, presentChannel.presentCode);
            if (cr.Errors.HasErrors)
            {
                string errInfo = "编译时发生错误：\r\n";
                foreach (CompilerError err in cr.Errors)
                {
                    errInfo += ("line " + err.Line + ": " + err.ErrorText + "\r\n");
                }
                MessageBox.Show(errInfo);
            }
            else
            {
                MessageBox.Show("编译通过");
            }
        }
    }
}
