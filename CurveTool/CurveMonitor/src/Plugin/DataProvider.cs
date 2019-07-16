/**
 * DataProvider抽象了一个插件的基本操作，插件被加载后将会是一个具备此接口的实例。
 * 对于每个插件，它代表了某一类端口的数据解析与读取。一个插件是可以打开多个数据端
 * 口的，因此插件接口的设计与C语言文件操作API类似。
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurveMonitor.src.Plugin
{
    using DATAHANDLE = Int64;

    interface DataProvider
    {
        /*每个插件具备一个名字，这个名字将作为插件名显示在菜单栏中*/
        string PluginName();

        /*
         * 一个插件可能对应着串口数据、网口数据、或其他自定义的数据。这些数据端口
         * 具备不同的配置参数，程序框架无法确定具体参数。因此该接口没有输入参数，
         * 它需负责打开自定义的对话框，让用户输入或选择具体参数。对于程序框架，则
         * 只需要根据返回的DATAHANDLE对数据端口进行标记和追踪。
         */
        DATAHANDLE Open();

        /*
         * 对于每个端口，数据泵每次从该端口取出一帧数据。LoadData就负责从指定的
         * 数据端口(dh)加载一帧数据，存储到buf中，buf的长度为maxLen，数据帧内数
         * 据的个数由LoadData返回值表示。
         */
        int LoadData(DATAHANDLE dh, float[] buf, int maxLen);


        int Close(DATAHANDLE dh);

        /*
         * 程序框架期望插件自行处理异常，当插件的接口发生错误时通过返回负值来对错误
         * 进行标记，然后通过LastError()获取具体的错位信息。
         */
        string LastError();
    }
}
