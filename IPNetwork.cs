using System;
using System.Net;
using System.Net.Sockets;

namespace Cidr
{
    public class IPNetwork
    {
        public IPNetwork(IPAddress firstIpAddress, int cidr)
        {
            if (firstIpAddress is null)
            {
                throw new ArgumentNullException(nameof(firstIpAddress));
            }

            if (firstIpAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("Value must be an IPv4 address.", nameof(firstIpAddress));
            }

            if (cidr < 0 || cidr > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(cidr), "Value must be between 0 and 32 (inclusive).");
            }

            FirstIpAddress = firstIpAddress;
            Cidr = cidr;
        }

        public IPAddress FirstIpAddress { get; }

        public int Cidr { get; }

        public override string ToString()
        {
            return $"{FirstIpAddress}/{Cidr}";
        }
    }
}

