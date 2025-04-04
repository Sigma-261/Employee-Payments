using EmployeePayments.Models;

namespace EmployeePayments.Interfaces;

public interface IParser
{
    public abstract List<EmployeePayroll> ParseExcel(IFormFile file);
}
