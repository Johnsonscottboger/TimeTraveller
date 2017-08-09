using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TimeTraveller.Converters
{
    public class HandAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dt = System.Convert.ToDateTime(value);

            var handType = parameter.ToString();

            //简单的算法
            //时针转动360度代表的是12个小时, 所以只需要取当前时间总秒数除以12个小时的秒数
            //分针转动360度代表的是60分钟
            //秒针转动360度代表的是60秒
            
            switch (handType)
            {
                case "hour":
                    return System.Convert.ToDouble((dt.Hour * 3600 + dt.Minute * 60 + dt.Second) * 360) / (12 * 60 * 60);
                case "minute":
                    return System.Convert.ToDouble((dt.Minute * 60 + dt.Second) * 360) / (60 * 60);
                case "second":
                    return (dt.Second * 360) / 60;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
