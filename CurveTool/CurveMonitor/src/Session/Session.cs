using CurveMonitor.src.DataPump;
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
        private class WinDataDeliver : BinDataDeliver
        {
            private CurveWindow cw = null;
            public WinDataDeliver(CurveWindow c)
            {
                this.cw = c;
            }

            public int Delive(double[] data)
            {
                if(cw != null)
                {
                    cw.DeliverData(data);
                }

                return data.Length;
            }
        }

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
            if(this.mCodeEditor != null)
            {
                this.mCodeEditor.CloseWindow();
            }

            if(this.mCurveWindow != null)
            {
                this.mCurveWindow.CloseWindow();
            }

            if(this.mDataPump != null)
            {
                this.mDataPump.Close();
            }

            if (this.mDataProvider != null)
            {
                this.mDataProvider.Close();
            }
        }

        private DataProvider mDataProvider = null;
        public DataProvider dataProvider
        {
            get { return mDataProvider; }
            set
            {
                mDataProvider = value;
                if(mDataPump != null)
                {
                    mDataPump.ResetDataProvider(mDataProvider);
                }
            }
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
            set
            {
                mDataPump = value;
                if(mDataProvider != null)
                {
                    mDataPump.ResetDataProvider(mDataProvider);
                }

                if(mWinDataDeliver != null)
                {
                    mDataPump.ResetChartDeliver(mWinDataDeliver);
                }
            }
        }

        private CurveWindow mCurveWindow = null;
        private WinDataDeliver mWinDataDeliver = null;
        public CurveWindow curveWindow
        {
            get { return mCurveWindow; }
            set
            {
                mCurveWindow = value;
                mWinDataDeliver = new WinDataDeliver(mCurveWindow);
                if(mDataPump != null)
                {
                    mDataPump.ResetChartDeliver(mWinDataDeliver);
                }
            }
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
            mDataProvider.Start();
            mDataPump.Start();
        }

        public void CloseDataPort()
        {
            mDataPump.Stop();
            mDataProvider.Close();
        }

        public void StoreDataFlowCtrl(bool en)
        {
            mDataPump.StoreChannelCtrl(en, false);
        }

        public void ShowDataFlowCtrl(bool en)
        {
            mDataPump.ChartChannelCtrl(en, en);
        }

        public void CurveWindowCtrl(bool show)
        {
            if (mCurveWindow.IsVisible)
            {
                mCurveWindow.Hide();
            }
            else
            {
                mCurveWindow.Show();
                //mCurveWindow.Topmost = true;
            }
        }

        public void UpdateStoreFile(string file_name)
        {

        }

        public void OpenCodeEditor()
        {
            if (mCodeEditor.IsVisible)
            {
                mCodeEditor.Hide();
            }
            else
            {
                mCodeEditor.ShowDialog();
            }
        }

        /* 
         * 不存在名为name的通道时意味着新增，存在则意味着替换，assembly为null则意味着删除 */
        public void SetVirtualChannel(string name, VirtualChannel.VirtualChannel vc)
        {
            this.mDataPump.SetVirtualChannel(name, vc);
        }
    }
}
