using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MarineRadioRTP
{
    public class MulticastNetwork
    {
        private byte[] bits = { 0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, (byte)0x80 };
        public int LOGICAL_CHANNELS = 81;
        public const int DEFAULT_PORT = 28000;

        private IPAddress netStart;
        private int prefix;
        private byte[] addressBytes;
        private IPEndPoint[] logicalChannels;
        private StringBuilder strategyDescription;

        public MulticastNetwork(string netStart, int prefix)
            : this(IPAddress.Parse(netStart), prefix)
        {
        }

        public IPEndPoint[] LogicalChannels
        {
            get
            {
                return this.logicalChannels;
            }
        }

        public IPEndPoint GetChannel(int channel)
        {
            return this.logicalChannels[channel - 1];
        }

        public MulticastNetwork(IPAddress netStart, int prefix)
        {
            if (netStart.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException("IPv4 only.");
            if (prefix > 32 || prefix < 1 || prefix == 31)
                throw new ArgumentException("Unusable subnet prefix.");
            this.prefix = prefix;
            this.netStart = netStart;
            this.addressBytes = netStart.GetAddressBytes();
            /*if (GetNetworkClass() != 'D')
                throw new ArgumentException(String.Format("The IP you provided is not a class D network ({0}).", GetNetworkClass()));*/
            this.logicalChannels = new IPEndPoint[LOGICAL_CHANNELS];
            this.strategyDescription = new StringBuilder();
            int poolSize = GetMaxNetworkHosts();
            long longIp = GetLong();
            if (poolSize >= logicalChannels.Length)
            {
                strategyDescription.AppendLine(String.Format("You have {0} addresses in your subnet, which accomodates one IP per {1} logical VHF channels starting at {2}.", poolSize, LOGICAL_CHANNELS, this.netStart.ToString()));
                for (int i = 0; i < logicalChannels.Length; i++)
                {
                    logicalChannels[i] = new IPEndPoint(LongToIP(longIp++), DEFAULT_PORT);
                }
            }
            else
            {
                int chanPerIP = poolSize <= 1 ? LOGICAL_CHANNELS : (int)Math.Ceiling((double)LOGICAL_CHANNELS / poolSize);
                strategyDescription.Append(String.Format("Your subnet size is {0}, which means there will be up to {1} logical VHF channels per IP.", poolSize, chanPerIP));
                int port = DEFAULT_PORT;
                for (int i = 0; i < logicalChannels.Length; i++)
                {
                    if (i > 0 && i % chanPerIP == 0)
                    {
                        longIp++;
                        port = DEFAULT_PORT;
                    }
                    else
                    {
                        port++;
                    }
                    logicalChannels[i] = new IPEndPoint(LongToIP(longIp), port);
                }
            }
        }

        private IPAddress LongToIP(long ip)
        {
            long a = (long)(ip & 0xff000000) >> 24;
            long b = (long)(ip & 0x00ff0000) >> 16;
            long c = (long)(ip & 0x0000ff00) >> 8;
            long d = (long)(ip & 0xff);
            return IPAddress.Parse(a + "." + b + "." + c + "." + d);
        }

        private long GetLong()
        {
            return ((long)(ByteToInt(addressBytes[0]) * 256 +
                ByteToInt(addressBytes[1])) * 256 +
                ByteToInt(addressBytes[2])) * 256 +
                ByteToInt(addressBytes[3]);
        }

        public int GetMaxNetworkHosts()
        {
            int hosts = (int)Math.Pow(2, (32 - this.prefix));
            return hosts == 1 ? hosts : hosts - 2;
        }

        public static int ByteToInt(byte b)
        {
            int i = b;
            if (b < 0)
                i = b & 0x7f + 128;
            return i;
        }

        public long GetBroadcast()
        {
            long netMask = ((long)Math.Pow(2, this.prefix) - 1) << (32 - this.prefix);
            long hostMask = (long)Math.Pow(2, 32 - this.prefix) - 1;
            return (GetLong() & netMask) | hostMask;
        }

        private bool IsBitSet(byte b, int bit)
        {
            if (bit >= 1 && bit <= 8)
            {
                return ((b & bits[bit]) != 0);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return strategyDescription.ToString();
        }

        /* Broken */
        public char GetNetworkClass()
        {
            if (!IsBitSet(addressBytes[0], 8))
                return 'A';
            else if (!IsBitSet(addressBytes[1], 7))
                return 'B';
            else if (!IsBitSet(addressBytes[2], 6))
                return 'C';
            else if (!IsBitSet(addressBytes[3], 5))
                return 'D';
            else
                return 'E';
        }
    }
}
