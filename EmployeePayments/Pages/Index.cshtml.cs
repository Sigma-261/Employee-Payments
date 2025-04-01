using DocumentFormat.OpenXml.InkML;
using EmployeePayments.Interfaces;
using EmployeePayments.Models;
using EmployeePayments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace EmployeePayments.Pages;

public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IParser _parser;
    private readonly ISender _sender;

    public IndexModel(IWebHostEnvironment environment, 
        IHttpClientFactory httpClientFactory,
        IParser parser,
        ISender sender)
    {
        _environment = environment;
        _httpClientFactory = httpClientFactory;
        _parser = parser;
        _sender = sender;
    }

    [BindProperty]
    public IFormFile UploadFile { get; set; }

    public List<string> Test { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var empPayrolls = _parser.ParseExcel(UploadFile);
            //var result = await _sender.SendPaymentMessageAsync(empPayrolls);
            //Test = result;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }

        return Page();
    }
}
