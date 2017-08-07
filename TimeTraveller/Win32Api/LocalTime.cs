using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeTraveller.Win32Api
{
    public class LocalTime
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SYSTEMTIME time);
    }

    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        public SYSTEMTIME(DateTime dateTime)
        {
            wYear = (ushort)dateTime.Year;
            wMonth = (ushort)dateTime.Month;
            wDayOfWeek = (ushort)dateTime.DayOfWeek;
            wDay = (ushort)dateTime.Day;
            wHour = (ushort)dateTime.Hour;
            wMinute = (ushort)dateTime.Minute;
            wSecond = (ushort)dateTime.Second;
            wMilliseconds = (ushort)dateTime.Millisecond;
        }

        public SYSTEMTIME FromDateTime(DateTime dateTime)
        {
            wYear = (ushort)dateTime.Year;
            wMonth = (ushort)dateTime.Month;
            wDayOfWeek = (ushort)dateTime.DayOfWeek;
            wDay = (ushort)dateTime.Day;
            wHour = (ushort)dateTime.Hour;
            wMinute = (ushort)dateTime.Minute;
            wSecond = (ushort)dateTime.Second;
            wMilliseconds = (ushort)dateTime.Millisecond;
            return this;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMinute, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }

        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }
}
