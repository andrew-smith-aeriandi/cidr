using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Cidr
{
    public static class IPNetworkExtensions
    {
        private readonly static uint[] _blockSizes;

        static IPNetworkExtensions()
        {
            _blockSizes = Enumerable.Range(0, 33)
                .Select(cidr => (uint)1 << (32 - cidr))
                .ToArray();
        }

        public static IEnumerable<IPNetwork> ConsolidateCidrs(this IEnumerable<IPNetwork> values)
        {
            return values
                .Select(value => {
                    var start = ToUnsignedInt(value.FirstIpAddress);
                    return (start: start, end: start + _blockSizes[value.Cidr]);
                })
                .OrderBy(t => t.start)
                .GetContiguousValues()
                .SelectMany(t => GetCidrs(t.start, t.end))
                .Select(t => new IPNetwork(ToIPAddress(t.address), t.cidr));
        }

        private static uint ToUnsignedInt(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return ((((((uint)bytes[0] << 8) | bytes[1]) << 8) | bytes[2]) << 8) | bytes[3];
        }

        private static IPAddress ToIPAddress(uint value)
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

        private static IEnumerable<(uint start, uint end)> GetContiguousValues(this IEnumerable<(uint start, uint end)> values)
        {
            var iterator = values.GetEnumerator();
            if (!iterator.MoveNext())
            {
                yield break;
            }

            if (iterator.Current.start >= iterator.Current.end)
            {
                throw new ArgumentException("In all cases the end value must be greater than the start value.", nameof(values));
            }

            var (start, end) = iterator.Current;

            while (iterator.MoveNext())
            {
                if (iterator.Current.start >= iterator.Current.end)
                {
                    throw new ArgumentException("In all cases the end value must be greater than the start value.", nameof(values));
                }

                if (iterator.Current.start <= end)
                {
                    if (iterator.Current.start < start)
                    {
                        throw new ArgumentException("Start values are not sorted in ascending order.", nameof(values));
                    }

                    if (iterator.Current.end > end)
                    {
                        end = iterator.Current.end;
                    }
                    continue;
                }
                
                yield return (start, end);

                (start, end) = iterator.Current;
            }

            yield return (start, end);
        }

        private static IEnumerable<(uint address, int cidr)> GetCidrs(uint start, uint end)
        {
            var count = end - start;
            
            while (count > 0) 
            {
                var cidr = 32;
                var maskBit = (uint)2;
                while ((start & (maskBit - 1)) == 0 && maskBit <= count)
                {
                    cidr--;
                    maskBit <<= 1;
                }

                yield return (start, cidr);

                var blockSize = _blockSizes[cidr];
                start += blockSize;
                count -= blockSize;
            }
        }
    }
}

