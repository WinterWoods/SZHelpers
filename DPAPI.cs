
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
namespace Crypt32
{
    public class DPAPI
    {
        private const int CRYPTPROTECT_LOCAL_MACHINE = 4;
        private const int CRYPTPROTECT_UI_FORBIDDEN = 1;
        private static KeyType defaultKeyType = KeyType.MachineKey;
        private static IntPtr NullPtr = IntPtr.Zero;

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CryptProtectData(ref DATA_BLOB pPlainText, string szDescription, ref DATA_BLOB pEntropy, IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pCipherText);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CryptUnprotectData(ref DATA_BLOB pCipherText, ref string pszDescription, ref DATA_BLOB pEntropy, IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pPlainText);
        public static string RdpFileTemplate = "screen mode id:i:{0}\r\ndesktopwidth:i:{1}\r\ndesktopheight:i:{2}\r\nsession bpp:i:24\r\nfull address:s:{3}\r\nconnect to console:i:{4}\r\ncompression:i:1\r\nkeyboardhook:i:2\r\naudiomode:i:2\r\nredirectdrives:i:1\r\nredirectprinters:i:0\r\nredirectcomports:i:0\r\nredirectsmartcards:i:0\r\ndisplayconnectionbar:i:1\r\nautoreconnection enabled:i:1\r\ndomain:s:{5}\r\nusername:s:{6}\r\npassword 51:b:{7}\r\nauthentication level:i:0\r\nprompt for credentials:i:0\r\ndisable wallpaper:i:0\r\ndisable full window drag:i:1\r\ndisable themes:i:0\r\ndisable cursor setting:i:1\r\nbitmapcachepersistenable:i:1";

        public static string Decrypt(string cipherText)
        {
            string str;
            return Decrypt(cipherText, string.Empty, out str);
        }

        public static string Decrypt(string cipherText, out string description)
        {
            return Decrypt(cipherText, string.Empty, out description);
        }
        public static string GetRdp(string ip,string port,string loginName,string password,string width="1024",string height="768")
        {
            string password1 = Encrypt(password).Replace("-", "");
            return string.Format(RdpFileTemplate, new object[] { "2", "1024", "768", ip + ":" + port, "0", "", loginName, password1 });
        }
        public static byte[] Decrypt(byte[] cipherTextBytes, byte[] entropyBytes, out string description)
        {
            byte[] buffer2;
            DATA_BLOB pPlainText = new DATA_BLOB();
            DATA_BLOB blob = new DATA_BLOB();
            DATA_BLOB data_blob3 = new DATA_BLOB();
            CRYPTPROTECT_PROMPTSTRUCT ps = new CRYPTPROTECT_PROMPTSTRUCT();
            InitPrompt(ref ps);
            description = string.Empty;
            try
            {
                try
                {
                    InitBLOB(cipherTextBytes, ref blob);
                }
                catch (Exception exception)
                {
                    throw new Exception("Cannot initialize ciphertext BLOB.", exception);
                }
                try
                {
                    InitBLOB(entropyBytes, ref data_blob3);
                }
                catch (Exception exception2)
                {
                    throw new Exception("Cannot initialize entropy BLOB.", exception2);
                }
                int dwFlags = 1;
                if (!CryptUnprotectData(ref blob, ref description, ref data_blob3, IntPtr.Zero, ref ps, dwFlags, ref pPlainText))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception("CryptUnprotectData failed.", new Win32Exception(error));
                }
                byte[] destination = new byte[pPlainText.cbData];
                Marshal.Copy(pPlainText.pbData, destination, 0, pPlainText.cbData);
                buffer2 = destination;
            }
            catch (Exception exception3)
            {
                throw new Exception("DPAPI was unable to decrypt data.", exception3);
            }
            finally
            {
                if (pPlainText.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pPlainText.pbData);
                }
                if (blob.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(blob.pbData);
                }
                if (data_blob3.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(data_blob3.pbData);
                }
            }
            return buffer2;
        }

        public static string Decrypt(string cipherText, string entropy, out string description)
        {
            if (entropy == null)
            {
                entropy = string.Empty;
            }
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipherText), Encoding.UTF8.GetBytes(entropy), out description));
        }

        public static string Encrypt(string plainText)
        {
            return Encrypt(defaultKeyType, plainText, string.Empty, string.Empty);
        }

        public static string Encrypt(KeyType keyType, string plainText)
        {
            return Encrypt(keyType, plainText, string.Empty, string.Empty);
        }

        public static string Encrypt(KeyType keyType, string plainText, string entropy)
        {
            return Encrypt(keyType, plainText, entropy, string.Empty);
        }

        public static byte[] Encrypt(KeyType keyType, byte[] plainTextBytes, byte[] entropyBytes, string description)
        {
            byte[] buffer2;
            if (plainTextBytes == null)
            {
                plainTextBytes = new byte[0];
            }
            if (entropyBytes == null)
            {
                entropyBytes = new byte[0];
            }
            if (description == null)
            {
                description = string.Empty;
            }
            DATA_BLOB blob = new DATA_BLOB();
            DATA_BLOB pCipherText = new DATA_BLOB();
            DATA_BLOB data_blob3 = new DATA_BLOB();
            CRYPTPROTECT_PROMPTSTRUCT ps = new CRYPTPROTECT_PROMPTSTRUCT();
            InitPrompt(ref ps);
            try
            {
                try
                {
                    InitBLOB(plainTextBytes, ref blob);
                }
                catch (Exception exception)
                {
                    throw new Exception("Cannot initialize plaintext BLOB.", exception);
                }
                try
                {
                    InitBLOB(entropyBytes, ref data_blob3);
                }
                catch (Exception exception2)
                {
                    throw new Exception("Cannot initialize entropy BLOB.", exception2);
                }
                int dwFlags = 1;
                if (keyType == KeyType.MachineKey)
                {
                    dwFlags |= 4;
                }
                if (!CryptProtectData(ref blob, description, ref data_blob3, IntPtr.Zero, ref ps, dwFlags, ref pCipherText))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception("CryptProtectData failed.", new Win32Exception(error));
                }
                byte[] destination = new byte[pCipherText.cbData];
                Marshal.Copy(pCipherText.pbData, destination, 0, pCipherText.cbData);
                buffer2 = destination;
            }
            catch (Exception exception3)
            {
                throw new Exception("DPAPI was unable to encrypt data.", exception3);
            }
            finally
            {
                if (blob.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(blob.pbData);
                }
                if (pCipherText.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pCipherText.pbData);
                }
                if (data_blob3.pbData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(data_blob3.pbData);
                }
            }
            return buffer2;
        }

        public static string Encrypt(KeyType keyType, string plainText, string entropy, string description)
        {
            if (plainText == null)
            {
                plainText = string.Empty;
            }
            if (entropy == null)
            {
                entropy = string.Empty;
            }
            return BitConverter.ToString(Encrypt(keyType, Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(entropy), description));
        }

        private static void InitBLOB(byte[] data, ref DATA_BLOB blob)
        {
            byte[] source = Encoding.Convert(Encoding.ASCII, Encoding.Unicode, data);
            blob.pbData = Marshal.AllocHGlobal(source.Length);
            if (blob.pbData == IntPtr.Zero)
            {
                throw new Exception("Unable to allocate data buffer for BLOB structure.");
            }
            blob.cbData = source.Length;
            Marshal.Copy(source, 0, blob.pbData, source.Length);
        }

        private static void InitPrompt(ref CRYPTPROTECT_PROMPTSTRUCT ps)
        {
            ps.cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT));
            ps.dwPromptFlags = 0;
            ps.hwndApp = NullPtr;
            ps.szPrompt = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        public enum KeyType
        {
            MachineKey = 2,
            UserKey = 1
        }
    }
}

