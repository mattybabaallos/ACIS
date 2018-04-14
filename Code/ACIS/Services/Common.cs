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
                case (int)Erros.RECEIVED_FEWER_THAN_TWO_BYTES:
                    return Erros.RECEIVED_FEWER_THAN_TWO_BYTES.ToString();
                case (int)Erros.INVALID_OPERATION:
                    return Erros.INVALID_OPERATION.ToString();
                case (int)Erros.INVALID_DEVICE:
                    return Erros.INVALID_DEVICE.ToString();
                case (int)Erros.COULD_NOT_DECODE_BYTES:
                    return Erros.COULD_NOT_DECODE_BYTES.ToString();
                case (int)Erros.COULD_NOT_PERFORM_OPERATION:
                    return Erros.COULD_NOT_DECODE_BYTES.ToString();
                default:
                    return "Undfined error happened";
            }

        }
    }
}
