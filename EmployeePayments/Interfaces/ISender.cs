using EmployeePayments.Models;

namespace EmployeePayments.Interfaces;

public interface ISender
{
    public abstract Task<List<ShippingInfo>> SendPaymentMessageAsync(List<EmployeePayment> empPayrolls);
}
