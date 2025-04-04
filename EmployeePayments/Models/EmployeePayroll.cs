namespace EmployeePayments.Models;

/// <summary>
/// Выплаты сотруднику
/// </summary>
public class EmployeePayroll
{
    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Платежка
    /// </summary>
    public string Payroll { get; set; }
}
