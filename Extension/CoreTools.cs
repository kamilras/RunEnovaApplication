using System;
using System.Text;

namespace RunEnovaApplication.Extension
{
    public class CoreTools
    {
        // Token: 0x06000025 RID: 37 RVA: 0x00003E34 File Offset: 0x00002034
        public static string ByteArrayToString(byte[] t)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in t)
            {
                CoreTools.ByteToHex(b, stringBuilder);
            }
            return stringBuilder.ToString();
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00003E74 File Offset: 0x00002074
        public static byte[] StringToByteArray(string str)
        {
            str = str.Trim();
            int num = str.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = CoreTools.HexToByte(str, 2 * i);
            }
            return array;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00003EC0 File Offset: 0x000020C0
        public static byte HexToByte(string str, int idx)
        {
            return (byte)(16 * CoreTools.GetByte(str[idx]) + CoreTools.GetByte(str[idx + 1]));
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003EF1 File Offset: 0x000020F1
        public static void ByteToHex(byte b, StringBuilder sb)
        {
            sb.Append(CoreTools.GetCharUpp((b & 240) >> 4));
            sb.Append(CoreTools.GetCharUpp((int)(b & 15)));
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00003F1C File Offset: 0x0000211C
        private static int GetByte(char ch)
        {
            bool flag = '0' <= ch && ch <= '9';
            int result;
            if (flag)
            {
                result = (int)(ch - '0');
            }
            else
            {
                bool flag2 = 'a' <= ch && ch <= 'f';
                if (flag2)
                {
                    result = (int)(ch - 'a' + '\n');
                }
                else
                {
                    bool flag3 = 'A' <= ch && ch <= 'F';
                    if (!flag3)
                    {
                        throw new ArgumentException();
                    }
                    result = (int)(ch - 'A' + '\n');
                }
            }
            return result;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00003F88 File Offset: 0x00002188
        private static char GetCharUpp(int v)
        {
            return (v < 10) ? ((char)(48 + v)) : ((char)(65 + v - 10));
        }
    }
}
