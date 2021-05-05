using System;
using System.Collections.Generic;
using System.Linq;

namespace Cidr
{
    public static class UintHelper
    {
        private readonly static uint[] _blockSizes;

        static UintHelper()
        {
            _blockSizes = Enumerable.Range(0, 33)
                .Select(cidr => (uint)1 << (32 - cidr))
                .ToArray();
        }

        public static IEnumerable<(uint address, int cidr)> ConsolidateCidrs(this IEnumerable<(uint address, int cidr)> cidrAddresses)
        {
            return cidrAddresses
                .OrderBy(t => t.address)
                .Select(t => (t.address, t.address + _blockSizes[t.cidr]))
                .GetContiguousValues()
                .SelectMany(t => GetCidrs(t.start, t.end));
        }

        public static IEnumerable<(uint start, uint end)> GetContiguousValues(this IEnumerable<(uint start, uint end)> values)
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

        public static IEnumerable<(uint, int)> GetCidrs(uint start, uint end)
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

