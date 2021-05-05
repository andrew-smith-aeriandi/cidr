using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Cidr
{
    public static class IPAddressHelper
    {
        public static IEnumerable<uint> ParseIpAddresses(this IEnumerable<string> ipAddresses)
        {
            return ipAddresses.Select(ParseIpAddress);
        }

        public static uint ParseIpAddress(string ipAddress)
        {
            return IPAddress.Parse(ipAddress).FromIPAddress();
        }

        public static IEnumerable<(uint address, int cidr)> ParseCidrs(this IEnumerable<string> cidrAddresses)
        {
            return cidrAddresses.Select(ParseCidr);
        }

        public static (uint address, int cidr) ParseCidr(string cidrAddress)
        {
            var index = cidrAddress.IndexOf('/');
            return index >= 0
                ? (ParseIpAddress(cidrAddress.Substring(0, index)), int.Parse(cidrAddress.Substring(index + 1)))
                : (ParseIpAddress(cidrAddress), 32);
        }

        public static uint FromIPAddress(this IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return ((((((uint)bytes[0] << 8) | bytes[1]) << 8) | bytes[2]) << 8) | bytes[3];
        }

        public static IPAddress ToIPAddress(this uint value)
        {
            return new IPAddress(
                new byte[]
                { 
                    (byte)((value >> 24) & 0xff),
                    (byte)((value >> 16) & 0xff), 
                    (byte)((value >> 8) & 0xff), 
                    (byte)(value & 0xff)
                });
        }

        public static IPNetwork ToIPNetwork(this (uint, int) value)
        {
            var (address, cidr) = value;
            return new IPNetwork(address.ToIPAddress(), cidr);
        }
     } 
}

