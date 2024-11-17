using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testVITTA.Core;
using testVITTA.MVVM.ViewModel;

namespace testVITTA.MVVM.Model
{
    public class IncomePaymentModel : ObservableObject
    {
        private static decimal _totalPaymentAmount;

        public static decimal TotalPaymentAmount
        {
            get { return _totalPaymentAmount; }
            set { _totalPaymentAmount = value; TotalPaymentAmountChanged?.Invoke(null, EventArgs.Empty); }
        }

        public static event EventHandler TotalPaymentAmountChanged;

        private int _incomeId;
        public int IncomeId
        {
            get { return _incomeId; }
            set
            {
                _incomeId = value;
                OnPropertyChanged();
            }
        }

        private decimal _baseRemainingIncome;
        public decimal BaseRemainingIncome
        {
            get { return _baseRemainingIncome; }
            set { _baseRemainingIncome = value; OnPropertyChanged(); }
        }

        private decimal _remainingIncome;
        public decimal RemainingIncome
        {
            get { return _remainingIncome; }
            set { _remainingIncome = value; OnPropertyChanged(); }
        }

        private decimal _paymentAmount;
        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set
            {
                TotalPaymentAmount -= _paymentAmount;
                if (value < 0)
                {
                    _paymentAmount = 0;
                }
                else if (value > BaseRemainingIncome)
                {
                    _paymentAmount = BaseRemainingIncome;
                } else
                {
                    _paymentAmount = value;
                }
                OnPropertyChanged();
                RemainingIncome = BaseRemainingIncome - PaymentAmount;
                TotalPaymentAmount += _paymentAmount;
            }
        }

        private byte[] _incomeRowVersion;
        public byte[] IncomeRowVersion
        {
            get { return _incomeRowVersion; }
            set
            {
                _incomeRowVersion = value;
                OnPropertyChanged();
            }
        }
        
        private DateTime _incomeDate;
        public DateTime IncomeDate
        {
            get { return _incomeDate; }
            set
            {
                _incomeDate = value;
                OnPropertyChanged();
            }
        }


    }
}
