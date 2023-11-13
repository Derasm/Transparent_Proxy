using SharpPcap;
using PacketDotNet;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using SharpPcap.LibPcap;
using System.Text.RegularExpressions;

// THis will be a basic Man in the Middle proxy server that will forward traffic to the destination server without altering it.
namespace ProxyServerProgram
{
    public class Proxy_Program
    {

        public static async Task Main()
        {
        DeviceController deviceController = new DeviceController();

            //Server.ServerInitialRun();
            var amountOfDevices = deviceController.GetAllDevices();
           
            Console.Out.WriteLineAsync(" hit 'Enter' to stop...");
            // check devices aren't 0.
            if (amountOfDevices == 0)
            {
                Console.WriteLine("No Wifi or Ethernet connection has been found.");
                // this should break the program, as there is no connection to use.
                return;
            }
            // call controller for writing to file.

            deviceController.StartCaptureOfPackets();
            Console.WriteLine("Press enter to stop capturing...");
            Console.ReadLine();
            Console.WriteLine("-- Capture stopped -- cleaning up");

            // cleanup
            deviceController.CloseAllConnections();
        }
        
    }
}
