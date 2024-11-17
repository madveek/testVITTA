using System;
using System.Collections.Generic;

namespace testVITTA.MVVM.Model;

public partial class Income
{
    public int IncomeId { get; set; }

    public DateTime IncomeDate { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal RemainingIncome { get; set; }

    public byte[] IncomeRowVersion { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
