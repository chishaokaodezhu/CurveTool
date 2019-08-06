using CurveMonitor.src.DataPump;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CurveMonitor.src.DataStore
{
    public class CSVStore : BinDataDeliver, StoreCtrl
    {
        private Queue dataQueue = null;
        private Thread workThread = null;
        private StreamWriter sw = null;
        private Semaphore sem = new Semaphore(0, 1024*1024);

        public CSVStore(string file)
        {
            dataQueue = Queue.Synchronized(new Queue());
            sw = new StreamWriter(file);
            workThread = new Thread(Run);
            workThread.Start();
        }

        public int Delive(double[] data)
        {
            try {
                sem.Release();
                dataQueue.Enqueue(data);
            }
            catch (SemaphoreFullException)
            {
                dataQueue.Dequeue();
                sem.WaitOne();
            }
            
            return 0;
        }

        public void Start()
        {

        }

        private bool isRun = true;
        public void Run()
        {
            try
            {
                while (isRun)
                {
                    sem.WaitOne();
                    double[] d = (double[])dataQueue.Dequeue();
                    for (int i = 0; i < d.Length - 1; i++)
                    {
                        sw.Write(d[i]);
                        sw.Write(',');
                    }

                    if(d.Length > 0)
                    {
                        sw.Write(d[d.Length - 1]);
                    }

                    if (d.Length > 0)
                    {
                        sw.WriteLine();
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                ;
            }
            
        }

        public void Close()
        {
            isRun = false;
            workThread.Interrupt();
        }
    }
}
