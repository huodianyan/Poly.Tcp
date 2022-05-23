//namespace Poly.Tcp
//{
//    public static class PolyTcpUtil
//    {
//        public static byte[] IntToBytesBigEndian(int value)
//        {
//            return new byte[] {
//                (byte)(value >> 24),
//                (byte)(value >> 16),
//                (byte)(value >> 8),
//                (byte)value
//            };
//        }

//        public static void IntToBytesBigEndianNonAlloc(int value, byte[] bytes)
//        {
//            bytes[0] = (byte)(value >> 24);
//            bytes[1] = (byte)(value >> 16);
//            bytes[2] = (byte)(value >> 8);
//            bytes[3] = (byte)value;
//        }

//        public static int BytesToIntBigEndian(byte[] bytes)
//        {
//            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
//        }

//    }
//}
