/*
 * 数据泵的工作负责从数据端口提取数据，然后分发到其他模块中去。同时也负责虚拟通道的
 * 数据计算产生，新产生的数据也将分发到其他模块中去。因为此部分较为简单，暂不做过度
 * 设计。
 */
using CurveMonitor.src.Plugin;
using PluginPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CurveMonitor.src.DataPump
{
    using DataDeliver = BinDataDeliver;

    public class DataPump
    {
        /*
         * dp       作为数据提供提供方
         * sotre    提供数据存储服务，对应模块需要实现DataDeliver接口
         * chart    提供数据显示服务，对应模块需要实现DataDeliver接口
         */
        public DataPump(DataProvider dp, DataDeliver store, DataDeliver chart)
        {

        }

        /*
         * 创建并开启工作线程，工作线程开启后将会持续进行数据提取，分发的工作；直到
         * Stop被调用。
         */
        public void Start()
        {

        }

        public void Stop()
        {

        }

        /*普通通道的数据与虚拟通道的数据分发交付是默认关闭的，需要调用如下接口开启或关闭*/
        public void StoreChannelCtrl(bool dataEn, bool vDatalEn)
        {

        }

        /*普通通道的数据与虚拟通道的数据分发交付是默认关闭的，需要调用如下接口开启或关闭*/
        public void ChartChannelCtrl(bool dataEn, bool vDataEn)
        {

        }

        /*
         * vChannel为虚拟通道数据点的计算函数，它的输入参数为采集到的一帧数据，输出为根据
         * 该帧数据计算出来的新的数据点。计算公式由用户自定义编码完成，编码完成后将会自动编
         * 译然后作为MethodInfo传入，使用MethodInfo等级的反射性能基本够用（只要用户自定义
         * 的代码公式运行起来不要太耗时）。
         */
        public void AppendVirtualChannel(string name, VirtualChannel.VirtualChannel vChannel)
        {

        }

        public void RemoveVirtualChannel(string name)
        {

        }

        private void WorkThread()
        {

        }
    }
}
