using EmployeePayments.Models;

namespace EmployeePayments.Interfaces;

public interface ISender
{
    public abstract Task<List<string>> SendPaymentMessageAsync(List<EmployeePayroll> empPayrolls);
}
