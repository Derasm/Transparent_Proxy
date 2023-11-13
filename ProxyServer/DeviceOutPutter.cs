using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServerProgram
{
    internal class DeviceOutPutter
    {
        private CaptureFileWriterDevice FileWriter { get; set; }
        private LibPcapLiveDevice Device { get; set; }
        public DeviceOutPutter(LibPcapLiveDevice device)
        {
            this.Device = device;
            this.FileWriter = new CaptureFileWriterDevice(Device.Name);

        }

        void WriteToFile()
        {
            FileWriter.Open(Device);
        }
    }
}
