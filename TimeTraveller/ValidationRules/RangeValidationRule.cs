using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace TimeTraveller.ValidationRules
{
    public class RangeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "值不能为空");
            }
            var i = 1;
            if (!int.TryParse(value.ToString(), out i) || i <= 0 || i > 1000)
            {
                return new ValidationResult(false, "请输入大于0,并且小于1000的数字");
            }
            return new ValidationResult(true, null);
        }
    }
}
