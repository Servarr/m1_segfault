using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace M1Seg
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SQLiteLogCallback(IntPtr pUserData, int errorCode, IntPtr pMessage);

    public class Program
    {
        private static SQLiteLogCallback _callback;

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            _callback = new SQLiteLogCallback(LogCallback);

            UnsafeNativeMethods.print("Hello from C!");

            UnsafeNativeMethods.callback(_callback);
        }

        private static void LogCallback(IntPtr pUserData, int errorCode, IntPtr pMessage)
        {
            var message = UTF8ToString(pMessage, -1);
            Console.WriteLine(message);
        }

        public static string UTF8ToString(IntPtr nativestring, int nativestringlen)
        {
            if (nativestring == IntPtr.Zero || nativestringlen == 0) return String.Empty;
            if (nativestringlen < 0)
            {
                nativestringlen = 0;

                while (Marshal.ReadByte(nativestring, nativestringlen) != 0)
                    nativestringlen++;

                if (nativestringlen == 0) return String.Empty;
            }

            byte[] byteArray = new byte[nativestringlen];

            Marshal.Copy(nativestring, byteArray, 0, nativestringlen);

            return System.Text.Encoding.UTF8.GetString(byteArray, 0, nativestringlen);
        }
    }

    internal static class UnsafeNativeMethods
    {
        private const string LIBNAME = "library.so";

        [DllImport(LIBNAME, EntryPoint="print", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void print(string message);

        [DllImport(LIBNAME, EntryPoint="callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int callback(SQLiteLogCallback func);

        [DllImport(LIBNAME, EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_config_log(int op, SQLiteLogCallback func, IntPtr pvUser);
    }
}
