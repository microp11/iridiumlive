/*
 * microp11 2017
 * 
 * https://stackoverflow.com/questions/461742/how-to-convert-an-ipv4-address-into-a-integer-in-c
 * https://www.codeproject.com/Articles/132623/Basic-UDP-Receiver
 * 
 * Receives UDP packets over the selected network card.
 * 
 */

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace BlazorUdp.Udp
{
    public class UdpReceiver : IDisposable
    {
        public UdpClient udpc;

        public UdpReceiver(int port)
        {
            IPEndPoint local = new IPEndPoint(IPAddress.Any, port);
            udpc = new UdpClient(local);
            udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

        public void Start()
        {
            udpc.BeginReceive(RequestCallback, null);
        }

        private void RequestCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                if (udpc.Client == null)
                {
                    return;
                }

                byte[] receiveBytes = udpc.EndReceive(ar, ref receivedIpEndPoint);

                UdpPacketArgs upa = new UdpPacketArgs
                {
                    UdpPacket = receiveBytes
                };
                OnUDPPacket?.Invoke(this, upa);

                // Restart listening for udp data
                udpc.BeginReceive(RequestCallback, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            udpc?.Close();
        }

        public event EventHandler<UdpPacketArgs> OnUDPPacket;
    }

    public class UdpPacketArgs : EventArgs
    {
        public byte[] UdpPacket { get; set; }
    }
}