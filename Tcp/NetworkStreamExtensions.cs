//using System.IO;
//using System.Net.Sockets;

//namespace Poly.Tcp
//{
//    public static class NetworkStreamExtensions
//    {
//        public static int ReadSafely(this NetworkStream stream, byte[] buffer, int offset, int size)
//        {
//            try
//            {
//                return stream.Read(buffer, offset, size);
//            }
//            catch (IOException)
//            {
//                return 0;
//            }
//        }

//        public static bool ReadExactly(this NetworkStream stream, byte[] buffer, int amount)
//        {
//            int bytesRead = 0;
//            while (bytesRead < amount)
//            {
//                // read up to 'remaining' bytes with the 'safe' read extension
//                int remaining = amount - bytesRead;
//                int result = stream.ReadSafely(buffer, bytesRead, remaining);

//                // .Read returns 0 if disconnected
//                if (result == 0)
//                    return false;

//                // otherwise add to bytes read
//                bytesRead += result;
//            }
//            return true;
//        }
//    }
//}