namespace EmployeePayments.Models;

/// <summary>
/// Выплаты сотруднику
/// </summary>
public class EmployeePayment
{
    /// <summary>
    /// Месяц
    /// </summary>
    public string Month { get; set; }

    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Отработанные часы за месяц
    /// </summary>
    public string Hours { get; set; }

    /// <summary>
    /// Выработка
    /// </summary>
    public string Production { get; set; }

    /// <summary>
    /// Выработка c учетом больничных и отпусков
    /// </summary>
    public string ProductionWithBonus { get; set; }

    /// <summary>
    /// Основная компонента зарплаты
    /// </summary>
    public string BaseSalary { get; set; }

    /// <summary>
    /// Больничные и отпускные
    /// </summary>
    public string Bonuses { get; set; }

    /// <summary>
    /// Зарплата к перечислению
    /// </summary>
    public string Salary { get; set; }

    /// <summary>
    /// Зарплата к перечислению с налогом
    /// </summary>
    public string ProfitWithTax { get; set; }

    /// <summary>
    /// Премии за проекты
    /// </summary>
    public string Rewards { get; set; }

    /// <summary>
    /// Распределение выплат
    /// </summary>
    public string PaymentDistribution { get; set; }
}
