using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Cidr
{
    public static class IPAddressHelper
    {
        public static IEnumerable<IPAddress> ParseIpAddresses(this IEnumerable<string> ipAddresses)
        {
            return ipAddresses.Select(IPAddress.Parse);
        }

        public static IEnumerable<IPNetwork> ParseCidrs(this IEnumerable<string> cidrAddresses)
        {
            return cidrAddresses.Select(IPNetwork.Parse);
        }
    } 
}

