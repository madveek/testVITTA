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
    public class IncomeStatusConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, object parametr, CultureInfo culture)
        {
            if (value is Income income)
            {
                return income.RemainingIncome != 0;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    } // Конвертер статуса платежа
}
