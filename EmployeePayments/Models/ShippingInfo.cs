namespace EmployeePayments.Models;

/// <summary>
/// Информация о доставленной платежке
/// </summary>
public class ShippingInfo
{
    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Employee { get; set; }

    /// <summary>
    /// Ошибка, если возникла
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Платежка
    /// </summary>
    public string Payroll { get; set; }

    /// <summary>
    /// Успешно ли доставлено сообщение с платежкой
    /// </summary>
    public bool IsSuccess { get; set; }
}
