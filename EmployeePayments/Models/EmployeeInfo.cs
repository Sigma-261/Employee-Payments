namespace EmployeePayments.Models;

/// <summary>
/// Информация о сотруднике
/// </summary>
public class EmployeeInfo
{
    /// <summary>
    /// Идентификатор 
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Имя в MatterMost
    /// </summary>
    public string Username { get; set; }
}
