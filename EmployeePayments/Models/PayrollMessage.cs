namespace EmployeePayments.Models;

/// <summary>
/// Сообщения с платежкой сотрудника
/// </summary>
public class PayrollMessage
{
    /// <summary>
    /// Идентификатор канала
    /// </summary>
    public string Channel_id { get; set; }

    /// <summary>
    /// Сообщение с платежкой
    /// </summary>
    public string Message { get; set; }
}
