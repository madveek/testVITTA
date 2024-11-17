using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using testVITTA.Core;
using testVITTA.MVVM.Model;

namespace testVITTA.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private TestContext _testContext;


        private bool _canSelectIncomes;
        public bool CanSelectIncomes
        {
            get { return _canSelectIncomes; }
            set { _canSelectIncomes = value; OnPropertyChanged(); }
        }
        private bool _canSendPayments;
        public bool CanSendPayments
        {
            get { return _canSendPayments; }
            set { _canSendPayments = value; OnPropertyChanged(); }
        }

        private decimal _remainingAmount;
        public decimal RemainingAmount
        {
            get { return _remainingAmount; }
            set { _remainingAmount = value; OnPropertyChanged(); }
        }

        private decimal _totalPaymentAmount;
        public decimal TotalPaymentAmount
        {
            get { return _totalPaymentAmount; }
            set { _totalPaymentAmount = value; OnPropertyChanged(); }
        }

        private decimal _paidAndPaymentsAmount;
        public decimal PaidAndPaymentsAmount
        {
            get { return _paidAndPaymentsAmount; }
            set
            {
                _paidAndPaymentsAmount = value;
                OnPropertyChanged();
                CanSendPayments = PaidAndPaymentsAmount <= SelectedOrder.TotalAmount;
            }
        }

        private Visibility _isVisibleOrderSet;
        public Visibility IsVisibleOrderSet
        {
            get { return _isVisibleOrderSet; }
            set { _isVisibleOrderSet = value; OnPropertyChanged(); }
        }



        public ObservableCollection<Income> Incomes { get; set; }
        public ObservableCollection<Payment> Payments { get; set; }
        public ObservableCollection<Order> Orders { get; set; }


        private ObservableCollection<IncomePaymentModel> _incomePayments;

        public ObservableCollection<IncomePaymentModel> IncomePayments
        {
            get { return _incomePayments; }
            set
            {
                _incomePayments = value;
                OnPropertyChanged();
            }
        }


        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();

                SelectedIncomes.Clear();
                CanSelectIncomes = _selectedOrder != null;
                CanSendPayments = false;
                IncomePayments.Clear();
                if (_selectedOrder != null)
                {
                    IsVisibleOrderSet = Visibility.Visible;
                }
                else
                {
                    IsVisibleOrderSet = Visibility.Collapsed;
                }
            }
        }

        private ObservableCollection<Income> _selectedIncomes;
        public ObservableCollection<Income> SelectedIncomes
        {
            get { return _selectedIncomes; }
            set
            {
                _selectedIncomes = value;

                CalculatePayments();
                OnPropertyChanged();
            }
        }

        public RelayCommand LoadDataCommand { get; set; }
        public RelayCommand ProcessPaymentCommand { get; set; }


        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(o => LoadData());
            ProcessPaymentCommand = new RelayCommand(o => ProcessPayment());

            Orders = new ObservableCollection<Order>();
            Incomes = new ObservableCollection<Income>();
            Payments = new ObservableCollection<Payment>();

            SelectedIncomes = new ObservableCollection<Income>();
            IncomePayments = new ObservableCollection<IncomePaymentModel>();


            _testContext = new TestContext();

            IsVisibleOrderSet = Visibility.Collapsed;

            LoadData();

            IncomePaymentModel.TotalPaymentAmountChanged += (s, e) =>
            {
                TotalPaymentAmount = IncomePaymentModel.TotalPaymentAmount;
                PaidAndPaymentsAmount = SelectedOrder.PaidAmount + TotalPaymentAmount;
            };


        }
        private void LoadData()
        {
            try
            {
                Orders.Clear();
                Incomes.Clear();
                Payments.Clear();

                var orders = _testContext.Orders
                    .AsNoTracking()
                    .Include(o => o.Payments)
                    .ToList();
                var incomes = _testContext.Incomes
                    .AsNoTracking()
                    .Include(o => o.Payments)
                    .ToList();
                var payments = _testContext.Payments
                    .AsNoTracking()
                    .ToList();

                foreach (var order in orders) Orders.Add(order);
                foreach (var income in incomes) Incomes.Add(income);
                foreach (var payment in payments) Payments.Add(payment);
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessPayment()
        {
            try
            {
                var incomePaymentsTable = new DataTable();
                incomePaymentsTable.Columns.Add("IncomeID", typeof(int));
                incomePaymentsTable.Columns.Add("PaymentAmount", typeof(decimal));
                incomePaymentsTable.Columns.Add("IncomeRowVersion", typeof(byte[]));

                foreach (var payment in IncomePayments)
                {
                    if (payment.PaymentAmount != 0)
                    {
                        incomePaymentsTable.Rows.Add(payment.IncomeId, payment.PaymentAmount, payment.IncomeRowVersion);
                    }
                }

                var orderIdParam = new SqlParameter("@OrderID", SelectedOrder.OrderId);
                var orderRowVersionParam = new SqlParameter("@OrderRowVersion", SelectedOrder.OrderRowVersion);
                var incomePaymentParam = new SqlParameter("@IncomePayments", incomePaymentsTable)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "IncomePayments"
                };

                _testContext.Database.ExecuteSqlRaw(
                    "EXEC [dbo].[ProcessPaymentForOrder] @OrderId, @OrderRowVersion, @IncomePayments",
                    orderIdParam, orderRowVersionParam, incomePaymentParam);

                LoadData();


            }
            catch (SqlException ex)
            {
                LoadData();
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CalculatePayments()
        {
            if (SelectedOrder == null) return;

            IncomePayments.Clear();
            IncomePaymentModel.TotalPaymentAmount = 0;

            RemainingAmount = SelectedOrder.TotalAmount - SelectedOrder.PaidAmount;

            var sortedIncomes = SelectedIncomes.OrderBy(i => i.IncomeDate);

            foreach (var income in sortedIncomes)
            {
                decimal paymentAmount = Math.Min(RemainingAmount, income.RemainingIncome);
                IncomePayments.Add(new IncomePaymentModel
                {
                    IncomeId = income.IncomeId,
                    BaseRemainingIncome = income.RemainingIncome,
                    IncomeDate = income.IncomeDate,
                    PaymentAmount = paymentAmount,
                    IncomeRowVersion = income.IncomeRowVersion
                });
                RemainingAmount -= paymentAmount;

            }
        }


    }
}
