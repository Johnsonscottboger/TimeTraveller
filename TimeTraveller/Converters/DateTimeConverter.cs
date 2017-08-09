using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TimeTraveller.Converters
{
    public class DateTimeConverter :IValueConverter
    {
        public Object Convert(Object value,Type targetType,Object parameter,CultureInfo culture)
        {
            DateTime dt;
            try
            {
                dt = (DateTime)value;
            }
            catch
            {
                dt = new DateTime();
            }
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public Object ConvertBack(Object value,Type targetType,Object parameter,CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
