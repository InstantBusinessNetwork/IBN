using System;

namespace Mediachase.IBN.Business.EMail
{
    internal class Crc16
    {
        /// <summary>
        /// Calculates Crc16 by specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static int Calculate(byte[] buffer, int offset, int length)
        {
			int crc = 0xFFFF;

			for (int i = 0; i < length; i++)
			{
				crc = crc ^ buffer[offset + i];

				for (int j = 0; j < 8; ++j)
				{
					if ((crc & 0x01) == 1)
						crc = (crc >> 1 ^ 0xA001);
					else
						crc >>= 1;
				}
			}
			return (crc);

        }
    }
}
