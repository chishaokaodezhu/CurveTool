using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortPlugin
{
    interface DataProvider
    {
        /*每个插件具备一个名字，这个名字将作为插件名显示在菜单栏中*/
        string PluginName();

        int Open();

        double[] LoadData();

        void Close();

        /*
         * 程序框架期望插件自行处理异常，当插件的接口发生错误时通过返回负值来对错误
         * 进行标记，然后通过LastError()获取具体的错位信息。
         */
        string LastError();
    }
}
