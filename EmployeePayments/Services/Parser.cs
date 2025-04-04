using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using EmployeePayments.Interfaces;
using EmployeePayments.Models;

namespace EmployeePayments.Services;

/// <summary>
/// Парсер документов
/// </summary>
public class Parser : IParser
{
    private int _empIndex = 4;

    private IXLWorksheet? ws;

    /// <summary>
    /// Парсинг excel документа
    /// </summary>
    /// <param name="file">Документ с данными о выплатах</param>
    /// <returns>Список выплат сотрудникам</returns>
    public List<EmployeePayroll> ParseExcel(IFormFile file)
    {
        using var fileStream = file.OpenReadStream();

        var workbook = new XLWorkbook(file.OpenReadStream());
        ws = workbook.Worksheet(1);

        bool isGPH = false;

        var empPayrolls = new List<EmployeePayroll>();

        for (;; _empIndex++)
        {
            if (ws.Column(_empIndex).IsHidden == true)
                continue;

            var empName = ws.Cell(1, _empIndex).GetString();

            if (empName == "")
            {
                if (!isGPH)
                {
                    isGPH = true;
                    continue;
                }
                break;
            }

            empPayrolls.Add(new EmployeePayroll()
            {
                Name = empName,
                Payroll = GetPayroll(isGPH)
            });
        }

        return empPayrolls;
    }

    /// <summary>
    /// Получение плтежки сотрудника
    /// </summary>
    /// <param name="isGPH">Является ли сотрудник ГПХ</param>
    /// <returns>Платежка</returns>
    private string GetPayroll(bool isGPH)
    {
        var hours = string.Format("{0:f2}", (Convert.ToInt32(ws.Cell(6, 1).GetFormattedString()) * 8));

        var profit = Convert.ToDecimal(ws.Cell(25, _empIndex).GetString());
        var tax = Convert.ToDecimal(ws.Cell(27, _empIndex).GetString());

        var profitWithTask = string.Format("{0:C2}", profit - tax);

        var month = ws.Cell(2, 1).Value.ToString();
        var baseSalary = ws.Cell(11, _empIndex).GetFormattedString();
        var salary = ws.Cell(25, _empIndex).GetFormattedString();
        var bonuses = GetBonuses();
        var rewards = GetRewards();
        var paymentDistribution = GetPayments();
        var production = isGPH == false 
            ? $"\r\nВыработка за месяц: {ws.Cell(5, _empIndex).GetFormattedString()} ставки"
            : "";
        var productionWithBonus = isGPH == false 
            ? $"\r\nВыработка за месяц с учётом больничных и отпускных: {ws.Cell(9, _empIndex).GetFormattedString()} ставок"
            : "";

        return $"""
                   #ЗарплатаЗа{month}

                   Общее количество рабочих часов в прошедший месяц: {hours} ч{production}{productionWithBonus}
                   Основная компонента зарплаты: {baseSalary}
                   {bonuses}{rewards}
                   **Итого начисленная зарплата:** {salary}
                   
                   **Итого зарплата к перечислению** (начисленнная зарплата минус НДФЛ 13 %): {profitWithTask}

                   | Распределение выплат ||
                   | --- | --: |
                   {paymentDistribution}
                   """;
    }

    /// <summary>
    /// Получение выплат по больничным/отпускным/компенсациям
    /// </summary>
    /// <returns></returns>
    private string GetBonuses()
    {
        var compensationPayment = ws!.Cell(11, _empIndex).GetFormattedString();
        var compensationReason = ws!.Cell(12, _empIndex).GetFormattedString();
        var vacationPayment = ws!.Cell(15, _empIndex).GetFormattedString();
        var sickDayPayment = ws!.Cell(16, _empIndex).GetFormattedString();

        var compensation = compensationPayment == ""
            ? ""
            : $"\r\nКомпенсация за {compensationReason} : {compensationPayment}";

        var vacation = vacationPayment == "" ? "" : $"\r\nОтпускные: {vacationPayment}";

        var sickDay = sickDayPayment == "" ? "" : $"\r\nБольничные: {sickDayPayment}";

        return $"{compensation}{vacation}{sickDay}";
    }

    /// <summary>
    /// Получение выплат за проекты
    /// </summary>
    /// <returns></returns>
    private string GetRewards()
    {
        var rewards = "";
        var rewardIndex = 18;

        while (true)
        {
            var project = ws!.Cell(rewardIndex, 2).GetFormattedString();

            if (project == "")
                break;

            var reward = ws!.Cell(rewardIndex, _empIndex).GetFormattedString();

            if (reward != "")
                rewards += $"\r\nПремия за {project}: {reward}";

            rewardIndex++;
        }

        if (rewards != "")
            rewards += "\r\n";

        return rewards;
    }

    /// <summary>
    /// Получение уточнений по выплатам
    /// </summary>
    /// <returns></returns>
    private string GetPayments()
    {
        var payments = "";
        var paymentIndex = 27;
        while (true)
        {
            var paymentName = ws!.Cell(paymentIndex, 2).GetFormattedString();

            if (paymentName == "")
                break;

            var payment = ws!.Cell(paymentIndex, _empIndex).GetFormattedString();

            if (payment != "")
                payments += $"| {paymentName} | {payment} |\r\n";

            paymentIndex++;
        }

        return payments;
    }
}
