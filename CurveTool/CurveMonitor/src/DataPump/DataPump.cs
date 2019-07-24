/*
 * 数据泵的工作负责从数据端口提取数据，然后分发到其他模块中去。同时也负责虚拟通道的
 * 数据计算产生，新产生的数据也将分发到其他模块中去。因为此部分较为简单，暂不做过度
 * 设计。
 */
using CurveMonitor.src.Plugin;
using PluginPort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualChannel;

namespace CurveMonitor.src.DataPump
{
    using DataDeliver = BinDataDeliver;

    public class DataPump
    {
        private DataProvider dataProvider = null;
        private DataDeliver storeDeliver = null;
        private DataDeliver chartDeliver = null;
        /*
         * dp       作为数据提供提供方
         * sotre    提供数据存储服务，对应模块需要实现DataDeliver接口
         * chart    提供数据显示服务，对应模块需要实现DataDeliver接口
         */
        public DataPump(DataProvider dp, DataDeliver store, DataDeliver chart)
        {
            dataProvider = dp;
            storeDeliver = store;
            chartDeliver = chart;

            StartWorkThread();
        }

        public DataPump()
        {
            StartWorkThread();
        }

        private void StartWorkThread()
        {
            Thread th = new Thread(WorkThread);
            th.Start();
        }

        public void ResetChartDeliver(DataDeliver chart)
        {
            chartDeliver = chart;
        }

        public void ResetDataProvider(DataProvider dp)
        {
            dataProvider = dp;
        }

        public void ResetStoreDeliver(DataDeliver store)
        {
            storeDeliver = store;
        }

        private bool isRead = false;
        private Semaphore sem = new Semaphore(0, 1);
        /*
         * 创建并开启工作线程，工作线程开启后将会持续进行数据提取，分发的工作；直到
         * Stop被调用。
         */
        public void Start()
        {
            isRead = true;
            sem.Release();
        }

        public void Stop()
        {
            isRead = false;
        }

        private bool isWork = true;
        public void Close()
        {
            isWork = false;
            isRead = false;
            try
            {
                sem.Release();
            }
            catch(SemaphoreFullException e)
            {
                ;
            }
        }

        private bool storeDataEn = false;
        private bool storeVDataEn = false;
        private bool chartDataEn = false;
        private bool chartVDataEn = false;

        /*普通通道的数据与虚拟通道的数据分发交付是默认关闭的，需要调用如下接口开启或关闭*/
        public void StoreChannelCtrl(bool dataEn, bool vDatalEn)
        {
            storeDataEn = dataEn;
            storeVDataEn = vDatalEn;
        }

        /*普通通道的数据与虚拟通道的数据分发交付是默认关闭的，需要调用如下接口开启或关闭*/
        public void ChartChannelCtrl(bool dataEn, bool vDataEn)
        {
            chartDataEn = dataEn;
            chartVDataEn = vDataEn;
        }

        private const int MAX_VIRTUAL_CHANNELS = 16;
        private VirtualChannel.VirtualChannel[] vcs = null;
        private Hashtable vcsMap = new Hashtable();
        /*
         * vChannel为虚拟通道数据点的计算函数，它的输入参数为采集到的一帧数据，输出为根据
         * 该帧数据计算出来的新的数据点。计算公式由用户自定义编码完成，编码完成后将会自动编
         * 译然后作为MethodInfo传入，使用MethodInfo等级的反射性能基本够用（只要用户自定义
         * 的代码公式运行起来不要太耗时）。
         */
        public void SetVirtualChannel(string name, VirtualChannel.VirtualChannel vChannel)
        {
            if (!vcsMap.ContainsKey(name))
            {
                for (int i = 0; i < MAX_VIRTUAL_CHANNELS; i++)
                {
                    if (vcs[i] == null)
                    {
                        vcs[i] = vChannel;
                        vcsMap.Add(name, i);
                        break;
                    }
                }
            }
            else
            {
                int idx = (int)vcsMap[name];
                vcs[idx] = vChannel;
            }
        }

        private void WorkThread()
        {
            vcs = new VirtualChannel.VirtualChannel[MAX_VIRTUAL_CHANNELS];
            for(int i = 0; i < MAX_VIRTUAL_CHANNELS; i++)
            {
                vcs[i] = null;
            }

            while (isWork)
            {
                sem.WaitOne();
                while (isRead)
                {
                    try
                    {
                        double[] data = dataProvider.LoadData();
                        double[] vData = new double[vcsMap.Count];

                        int vIdx = 0;
                        for(int i = 0; i < MAX_VIRTUAL_CHANNELS; i++)
                        {
                            if(vcs[i] != null)
                            {
                                vData[vIdx++] = vcs[i].GetChannelValue(data);
                            }
                        }

                        double[] finalData = new double[data.Length + vData.Length];
                        vIdx = 0;
                        for(int i = 0; i < data.Length; i++)
                        {
                            finalData[vIdx++] = data[i]; 
                        }

                        for(int i = 0; i < vData.Length; i++)
                        {
                            finalData[vIdx++] = vData[i];
                        }


                        if(storeDeliver != null)
                        {
                            if (this.storeDataEn && this.storeVDataEn)
                            {
                                storeDeliver.Delive(finalData);
                            }
                            else if (this.storeDataEn)
                            {
                                storeDeliver.Delive(data);
                            }
                            else if(this.storeVDataEn)
                            {
                                storeDeliver.Delive(vData);
                            }
                            else
                            {

                            }
                        }

                        if(chartDeliver != null)
                        {
                            if(this.chartDataEn && this.chartVDataEn)
                            {
                                chartDeliver.Delive(finalData);
                            }
                            else if (this.chartDataEn)
                            {
                                chartDeliver.Delive(data);
                            }
                            else if (this.chartVDataEn)
                            {
                                chartDeliver.Delive(vData);
                            }
                            else
                            {

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
            }
        }
    }
}
