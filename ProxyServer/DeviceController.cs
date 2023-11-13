using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProxyServerProgram
{
    public class DeviceController
    {
        
        private static CaptureFileWriterDevice captureFileWriter;
        private int ReadTimeoutMilliseconds = 1000;
        private int PacketIndex = 0;
        public List<LibPcapLiveDevice> Devices { get; set; } = new List<LibPcapLiveDevice>();
        /// <summary>
        /// Starts the flow - gets all devices based on an initial filter. Can be extended later.
        /// </summary>
        /// <returns></returns>
        public int GetAllDevices()
        {
            
            //first get the devices, choose the wifi and the ethernet as those are the two main ones.
            foreach (var device in LibPcapLiveDeviceList.Instance)
            {

                // if it is a device of type Ethernet, add it to the list of devices, else close it again.
                device.Open();
                if (device.LinkType == LinkLayers.Ethernet)
                {
                    Devices.Add(device);
                    Console.WriteLine("-- Listening on {0} {1}",
                              device.Name, device.Description);
                }
                else
                {
                    device.Close();
                }

            }
            return Devices.Count;
        }
        public void StartCaptureOfPackets()
        {
            foreach (var device in Devices)
            {
                device.Open(mode: DeviceModes.DataTransferUdp | DeviceModes.NoCaptureLocal, read_timeout: ReadTimeoutMilliseconds);
                device.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);
                // open a fileWriter with the device. 
                captureFileWriter = new CaptureFileWriterDevice(device.Description);
                captureFileWriter.Open(device);
                device.StartCapture();
            }
        }
        /// <summary>
        /// Uses internal captureFileWriter to write packets to file.
        /// </summary>
        /// <param name="device"></param>
        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            //var device = (ICaptureDevice)sender;

            // Output packet contents to the console
            var time = e.Header.Timeval.Date;
            var len = e.Data.Length;
            var rawPacket = e.GetPacket();
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(rawPacket.ToString());

            // write the packet to the file
            captureFileWriter.Write(rawPacket);
            // parse packet
            Console.WriteLine("Packet dumped to file.");

            if (rawPacket.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                var ethernetPacket = (EthernetPacket)packet;

                Console.WriteLine("{0} At: {1}:{2}: MAC:{3} -> MAC:{4}",
                                  PacketIndex,
                                  rawPacket.Timeval.Date.ToString(),
                                  rawPacket.Timeval.Date.Millisecond,
                                  ethernetPacket.SourceHardwareAddress,
                                  ethernetPacket.DestinationHardwareAddress);
                PacketIndex++;
            }
        }
        public void CloseAllConnections()
        {
            foreach (var device in Devices)
            {
                Console.WriteLine(device.Statistics.ToString());
                device.StopCapture();
                device.Close();
            }
        }
    }

}
