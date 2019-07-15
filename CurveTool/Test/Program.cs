using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        public void Save(int data)
        {
            MemoryMappedFile memory = MemoryMappedFile.CreateFromFile("test.data", System.IO.FileMode.OpenOrCreate, "img", 1024);
            MemoryMappedViewAccessor accessor1 = memory.CreateViewAccessor();

            accessor1.Write(0, data);  // 在指定位置写入int值
            accessor1.Dispose();        //
            
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Save(0x32323232);
        }
    }
}
