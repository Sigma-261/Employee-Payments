using EmployeePayments.Models;

namespace EmployeePayments.Interfaces;

public interface IParser
{
    public abstract List<EmployeePayment> ParseExcel(IFormFile file);
}
