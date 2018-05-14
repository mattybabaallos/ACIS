using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class Common
    {

        /// <summary>
        /// Create a bit mask.
        /// </summary>
        /// <param name="start">The start of the bit mask 0 inclusive</param>
        /// <param name="end"> The end of the bit mask 0 inclusive</param>
        /// <returns>the bit mask</returns>
        public static uint CrateMask(int start, int end)
        {

            int i;
            uint mask = 0;
            uint one = 1; // used because default magic # is int

            for (i = start; i <= end; ++i)
            {
                mask |= (one << i);
            }
            return mask;
        }


        public static string ErrorCodeToString(int errorCode)
        {
            switch (errorCode)
            {
                case (int)Errors.ReceivedFewerThanTwoBytes:
                    return Errors.ReceivedFewerThanTwoBytes.ToString();
                case (int)Errors.InvalidOperation:
                    return Errors.InvalidOperation.ToString();
                case (int)Errors.InvalidDevice:
                    return Errors.InvalidDevice.ToString();
                case (int)Errors.CouldNotDecodeBytes:
                    return Errors.CouldNotDecodeBytes.ToString();
                case (int)Errors.CouldNotPerformOperation:
                    return Errors.CouldNotDecodeBytes.ToString();
                default:
                    return "Undefined error happened";
            }

        }
    }
}
