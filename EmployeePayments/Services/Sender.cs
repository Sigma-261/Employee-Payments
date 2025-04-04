using DocumentFormat.OpenXml.Bibliography;
using EmployeePayments.Interfaces;
using EmployeePayments.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Web.Http;

namespace EmployeePayments.Services;

/// <summary>
/// Отправитель сообщений
/// </summary>
public class Sender : ISender
{
    private readonly IHttpClientFactory _httpClientFactory;
    public Sender(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Отправка сообщений о выплатах сотрудникам
    /// </summary>
    /// <returns></returns>
    public async Task<List<ShippingInfo>> SendPaymentMessageAsync(List<EmployeePayroll> empPayrolls)
    {
        var httpClient = _httpClientFactory.CreateClient("PaymentBot");

        HttpResponseMessage empInfosMessage = await httpClient.GetAsync("users");
        var empInfosResponse = await empInfosMessage.Content.ReadAsStringAsync();

        var emps = JsonConvert.DeserializeObject<List<EmployeeInfo>>(empInfosResponse);

        var botId = emps?.FirstOrDefault(x => x.Username == "salarybot")?.Id;

        var empPayrollInfo = new List<ShippingInfo>();

        foreach(var empPayroll in empPayrolls)
        {
            var userId = emps?.FirstOrDefault(x => x.Username == empPayroll.Name)?.Id;

            if (userId is null)
            {
                empPayrollInfo.Add(new ShippingInfo()
                {
                    Employee = empPayroll.Name,
                    Payroll = empPayroll.Payroll,
                    IsSuccess = false,
                    Error = "Сотрудник не найден!"
                });

                continue;
            }

            var createChannelMessage = await 
                httpClient.PostAsync("channels/direct", 
                    new StringContent($"[\"{botId}\", \"{userId}\"]", 
                    Encoding.UTF8, "application/json"));

            var createChannelResponse = await createChannelMessage.Content.ReadAsStringAsync();

            var channelId = JObject.Parse(createChannelResponse)["id"]?.ToString();

            var payrollMessage = new
            { 
                Channel_id = channelId, 
                Message = empPayroll.Payroll
            };

            var sendMessage = await
                httpClient.PostAsync("posts",
                    JsonContent.Create(payrollMessage));

            empPayrollInfo.Add(new ShippingInfo()
            {
                Employee = empPayroll.Name,
                Payroll = empPayroll.Payroll,
                IsSuccess = sendMessage.IsSuccessStatusCode,
                Error = sendMessage.IsSuccessStatusCode == true
                    ? ""
                    : sendMessage.Content.ReadAsAsync<HttpError>().ToString() ?? ""
            });
        }

        return empPayrollInfo;
    }
}

