using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using testVITTA.MVVM.Model;

namespace testVITTA.Converters
{
    public class IncomeStatusToImageConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, object parametr, CultureInfo culture)
        {
            if (value is Income income)
            {
                if (income.RemainingIncome == 0) { return "/Images/multiply.png"; }
                else { return "/Images/ruble.png"; }
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
