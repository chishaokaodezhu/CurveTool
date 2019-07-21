using CurveMonitor.src.Graph;
using CurveMonitor.src.UI;
using PluginPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurveMonitor.src.UI.PortPannel;

namespace CurveMonitor.src.Session
{
    class Session : PortUIOp
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

        private DataProvider mDataProvider;
        public DataProvider dataProvider
        {
            get { return mDataProvider; }
            set { mDataProvider = value; }
        }

        private PortPannel mPortPannel;
        public PortPannel portPannel
        {
            set
            {
                mPortPannel = value;
                mPortPannel.BindOpHandler(this, this);
            }
        }

        private DataPump.DataPump mDataPump;
        public DataPump.DataPump dataPump
        {
            get { return mDataPump; }
            set { mDataPump = value; }
        }

        private CurveWindow mCurveWindow;
        public CurveWindow curveWindow
        {
            get { return mCurveWindow; }
            set { mCurveWindow = value; }
        }

        /*
         * 在这里完成所有的对某个端口的所有用户交互
         */
        public bool UIOp(Object obj, OpType type)
        {
            return true;
        }
    }
}
