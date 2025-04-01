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
    private int _rewardIndex = 17;
    private int _paymentIndex = 26;

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

        if (ws is null)
            throw new Exception("Error");

        bool isGPH = false;

        var empPayrolls = new List<EmployeePayroll>();

        while (true)
        {
            //Тут должно быть не фио, а имя на маттермосте или почта
            var empName = ws.Cell(1, _empIndex).Value.ToString();

            if (empName == "")
            {
                if (!isGPH)
                {
                    _empIndex++;
                    isGPH = true;
                    continue;
                }
                break;
            }

            empPayrolls.Add(new EmployeePayroll()
            {
                Name = empName,
                Month = ws.Cell(2, 1).Value.ToString(),
                Hours = ws.Cell(2, _empIndex).GetFormattedString(),
                Salary = ws.Cell(10, _empIndex).GetFormattedString(),
                Profit = ws.Cell(24, _empIndex).GetFormattedString(),
                Bonuses = GetBonuses(),
                Rewards = GetRewards(),
                Payments = GetPayments(),
                Production = isGPH == true ? "" : $"\r\nВыработка за месяц: {ws.Cell(5, _empIndex).GetFormattedString()} ставки",
            });

            _empIndex++;
        }

        return empPayrolls;
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
        while (true)
        {
            var project = ws!.Cell(_rewardIndex, 2).GetFormattedString();

            if (project == "")
                break;

            var reward = ws!.Cell(_rewardIndex, _empIndex).GetFormattedString();

            if (reward != "")
                rewards += $"\r\nПремия за {project}: {reward}";

            _rewardIndex++;
        }

        if (rewards != "")
        {
            rewards += "\r\n";
        }

        return rewards;
    }

    /// <summary>
    /// Получение уточнений по выплатам
    /// </summary>
    /// <returns></returns>
    private string GetPayments()
    {
        var payments = "";

        while (true)
        {
            var paymentName = ws!.Cell(_paymentIndex, 2).GetFormattedString();

            if (paymentName == "")
                break;

            var payment = ws!.Cell(_paymentIndex, _empIndex).GetFormattedString();

            if (payment != "")
                payments += $"| {paymentName} | {payment} |\r\n";

            _paymentIndex++;
        }

        return payments;
    }
}
