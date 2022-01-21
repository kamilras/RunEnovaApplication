using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RunEnovaApplication.Extension
{
    public class OperatorExt
    {
        // Token: 0x0600002C RID: 44 RVA: 0x00003FB0 File Offset: 0x000021B0
        internal static string GetPasswordHash(Guid databaseGuid, string password, Guid guid)
        {
            byte[] buffer = PasswordToBytes(databaseGuid, password, guid);
            return CoreTools.ByteArrayToString(passwordHash0.ComputeHash(buffer));
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00003FDC File Offset: 0x000021DC
        internal static ValueTuple<string, string> GetPasswordHashes(Guid databaseGuid, string password, Guid guid)
        {
            byte[] buffer = PasswordToBytes(databaseGuid, password, guid);
            return new ValueTuple<string, string>(CoreTools.ByteArrayToString(passwordHash0.ComputeHash(buffer)), CoreTools.ByteArrayToString(passwordHash1.ComputeHash(buffer)));
        }

        // Token: 0x0600002E RID: 46 RVA: 0x0000401C File Offset: 0x0000221C
        public static bool IsValidPassword(Guid databaseGuid, string password, Guid guid, string dbPassword)
        {
            byte[] buffer = PasswordToBytes(databaseGuid, password, guid);
            return dbPassword == CoreTools.ByteArrayToString(passwordHash0.ComputeHash(buffer)) || dbPassword == CoreTools.ByteArrayToString(passwordHash1.ComputeHash(buffer));
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00004068 File Offset: 0x00002268
        private static byte[] PasswordToBytes(Guid databaseGuid, string password, Guid guid)
        {
            return databaseGuid.ToByteArray().Concat(Encoding.Unicode.GetBytes(password)).Concat(guid.ToByteArray()).ToArray<byte>();
        }

        // Token: 0x06000030 RID: 48 RVA: 0x000040A4 File Offset: 0x000022A4
        private static byte[] PasswordToBytes(string password, Guid guid)
        {
            byte[] array = guid.ToByteArray();
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] array2 = new byte[array.Length + bytes.Length];
            array.CopyTo(array2, 0);
            bytes.CopyTo(array2, array.Length);
            return array2;
        }

        // Token: 0x0400001D RID: 29
        private static readonly byte[] passwordKey0 = new byte[]
        {
            157,
            55,
            226,
            54,
            168,
            215,
            175,
            115,
            184,
            24,
            135,
            153,
            114,
            173,
            243,
            141,
            203,
            23,
            171,
            167,
            13,
            177,
            247,
            197,
            40,
            222,
            125,
            21,
            70,
            43,
            33,
            238,
            69,
            101,
            24,
            65,
            12,
            54,
            2,
            137,
            149,
            12,
            68,
            102,
            135,
            42,
            26,
            111,
            208,
            201,
            77,
            231,
            93,
            76,
            71,
            236,
            35,
            29,
            241,
            202,
            61,
            125,
            45,
            141
        };

        // Token: 0x0400001E RID: 30
        private static readonly byte[] passwordKey1 = new byte[]
        {
            54,
            94,
            185,
            184,
            123,
            100,
            223,
            213,
            253,
            51,
            5,
            87,
            27,
            64,
            88,
            111,
            67,
            94,
            63,
            82,
            175,
            129,
            134,
            211
        };

        // Token: 0x0400001F RID: 31
        private static readonly KeyedHashAlgorithm passwordHash0 = new HMACSHA256(passwordKey0);

        // Token: 0x04000020 RID: 32
        private static readonly KeyedHashAlgorithm passwordHash1 = new MACTripleDES(passwordKey1);
    }
}
