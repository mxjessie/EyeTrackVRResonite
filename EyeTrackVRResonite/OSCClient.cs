using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Elements.Core;
using EyeTrackVR;
using OscCore;

namespace EyeTrackVRResonite
{
    // Credit to yewnyx on the VRC OSC Discord for this

    public class ETVROSC
    {
        private static bool _oscSocketState;
        public static readonly Dictionary<string, float> EyeDataWithAddress = new();

        private static UdpClient? _receiver;
        private static Task? _task;

        private const int DefaultPort = 9000;

        public ETVROSC(int? port = null)
        {
            if (_receiver != null)
            {
                return;
            }

            IPAddress.TryParse("127.0.0.1", out var candidate);

            _receiver = port.HasValue
                ? new UdpClient(new IPEndPoint(candidate, port.Value))
                : new UdpClient(new IPEndPoint(candidate, DefaultPort));

            foreach (var shape in ETVRExpressions.EyeDataWithAddress)
                EyeDataWithAddress.Add(shape, 0f);

            _oscSocketState = true;
            _task = Task.Run(ListenLoop);
        }

        private static async void ListenLoop()
        {
            UniLog.Log("Started EyeTrackVR loop");
            while (_oscSocketState)
            {
                var result = await _receiver.ReceiveAsync();
                var message = OscMessage.Read(result.Buffer, 0, result.Buffer.Length);
                if (!EyeDataWithAddress.ContainsKey(message.Address))
                {
                    continue;
                }

                if (float.TryParse(message[0].ToString(), out var candidate))
                {
                    EyeDataWithAddress[message.Address] = candidate;
                }
            }
        }

        public static void Teardown()
        {
            UniLog.Log("EyeTrackVR teardown called");
            _oscSocketState = false;
            _receiver.Close();
            _task.Wait();
            UniLog.Log("EyeTrackVR teardown completed");
        }
    }
}