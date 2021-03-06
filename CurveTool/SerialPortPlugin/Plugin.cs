﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PluginPort;

namespace SerialPortPlugin
{
    using DATAHANDLE = Int64;

    public class Plugin : DataProvider
    {
        private string error = null;
        private String portName = null;
        private int baudRate = 0;
        private SerialPort serialPort = null;

        public string PluginName()
        {
            return "通用串口";
        }

        public int Open()
        {
            SerialPortOpen w = new SerialPortOpen();
            w.ShowDialog();
            //MessageBox.Show(w.selectedPort);
            if(w.selectedPort == null || w.baudRate == 0)
            {
                error = "no port selected.";
                return -1;
            }
            this.portName = w.selectedPort;
            this.baudRate = w.baudRate;
            //serialPort = new SerialPort(w.selectedPort, w.baudRate);
            return 0;
        }

        public void Start()
        {
            serialPort = new SerialPort(this.portName, this.baudRate);
            serialPort.Open();
        }

        public double[] LoadData()
        {
            string data = serialPort.ReadLine();
            string[] datas = data.Split(',');
            double[] numbers = new double[datas.Length];
            for(int i = 0; i < datas.Length; i++)
            {
                numbers[i] = double.Parse(datas[i]);
            }
            return numbers;
        }

        public void Close()
        {
            if(serialPort != null)
            {
                serialPort.Close();
            }
        }

        public string LastError()
        {
            return error;
        }
    }
}
