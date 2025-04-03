namespace EmployeePayments.Models;

public class PayrollInfo
{
    public string Employee { get; set; }
    public string Error { get; set; }
    public string Payroll { get; set; }
    public bool IsSuccess { get; set; }
}
