using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Elements.Core;
using EyeTrackVR;
using OscCore;
using OscCore.LowLevel;

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
                var bytes = new System.ArraySegment<byte>(result.Buffer, 0, result.Buffer.Length);
                if (IsBundle(bytes))
                {
                    var bundle = new OscBundleRaw(bytes);
                    foreach (var message in bundle)
                        ProcessOscMessage(message);
                }
                else
                {
                    var message = new OscMessageRaw(bytes);
                    ProcessOscMessage(message);
                }
            }
        }

        private static void ProcessOscMessage(OscMessageRaw message)
        {
            if (!EyeDataWithAddress.ContainsKey(message.Address))
                return;

            var arg = message[0];

            switch (arg.Type)
            {
                case (OscToken.Float):
                    EyeDataWithAddress[message.Address] = message.ReadFloat(ref arg);
                    break;
                case (OscToken.Int):
                    EyeDataWithAddress[message.Address] = message.ReadInt(ref arg);
                    break;
                default:
                    Console.WriteLine($"Unknown OSC type: {arg.Type}");
                    break;
            }
        }

        private static readonly byte[] BundlePrefix = Encoding.ASCII.GetBytes("#bundle");
        private static bool IsBundle(System.ArraySegment<byte> bytes)
        {
            var prefix = BundlePrefix;
            if (bytes.Count < prefix.Length)
                return false;

            var i = 0;
            foreach (var b in bytes)
            {
                if (i < prefix.Length && b != prefix[i++])
                    return false;
                if (i == prefix.Length)
                    break;
            }
            return true;
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
