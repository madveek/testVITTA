using System;
using System.Collections.Generic;

namespace testVITTA.MVVM.Model;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public int IncomeId { get; set; }

    public decimal PaymentAmount { get; set; }

    public virtual Income Income { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
