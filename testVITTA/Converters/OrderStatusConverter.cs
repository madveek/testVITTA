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
    public class OrderStatusConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, object parametr, CultureInfo culture)
        {
            if (value is Order order)
            {
                return order.TotalAmount != order.PaidAmount;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    } // Конвертер статуса оплаты заказа
}
