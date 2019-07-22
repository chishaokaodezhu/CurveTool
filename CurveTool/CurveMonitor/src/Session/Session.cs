using CurveMonitor.src.Graph;
using CurveMonitor.src.UI;
using PluginPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CurveMonitor.src.UI.PortPannel;

namespace CurveMonitor.src.Session
{
    public class Session
    {
        private static List<Session> sessionList = new List<Session>();

        public static Session[] SessionList
        {
            get
            {
                return sessionList.ToArray();
            }
        }

        public static void CloseAllSession()
        {
            for(int i = 0; i < sessionList.Count; i++)
            {
                sessionList[i].Close();
            }

            sessionList.Clear();
        }

        public Session()
        {
            sessionList.Add(this);
        }

        /* 在这里关闭文件、线程、窗口、端口等操作 */
        public void Close()
        {

        }

        private DataProvider mDataProvider = null;
        public DataProvider dataProvider
        {
            get { return mDataProvider; }
            set { mDataProvider = value; }
        }

        private PortPannel mPortPannel = null;
        public PortPannel portPannel
        {
            set
            {
                mPortPannel = value;
                mPortPannel.BindOpHandler(this);
            }
        }

        private DataPump.DataPump mDataPump = null;
        public DataPump.DataPump dataPump
        {
            get { return mDataPump; }
            set { mDataPump = value; }
        }

        private CurveWindow mCurveWindow = null;
        public CurveWindow curveWindow
        {
            get { return mCurveWindow; }
            set { mCurveWindow = value; }
        }

        private CodeEditor mCodeEditor = null;
        public CodeEditor codeEditor
        {
            get { return mCodeEditor; }
            set
            {
                mCodeEditor = value;
                mCodeEditor.BindOpHandler(this);
            }
        }

        public void OpenDataPort()
        {
            
        }

        public void CloseDataPort()
        {
            
        }

        public void StoreDataFlowCtrl(bool en)
        {
            
        }

        public void ShowDataFlowCtrl(bool en)
        {

        }

        public void CurveWindowCtrl(bool show)
        {

        }

        public void UpdateStoreFile(string file_name)
        {

        }

        public void AddVirtualChannel(string name, Assembly assembly)
        {

        }

        public void RemoveVirutalChannel(string name)
        {

        }
    }
}
