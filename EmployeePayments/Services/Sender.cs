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
    public async Task<List<string>> SendPaymentMessageAsync(List<EmployeePayroll> empPayrolls)
    {
        var httpClient = _httpClientFactory.CreateClient("PaymentBot");

        HttpResponseMessage empInfosMessage = await httpClient.GetAsync("users");
        var empInfosResponse = await empInfosMessage.Content.ReadAsStringAsync();

        var empInfos = JsonConvert.DeserializeObject<List<EmployeeInfo>>(empInfosResponse);

        var botId = empInfos.FirstOrDefault(x => x.Username == "salarybot")?.Id;


        var empPayrollInfo = new List<string>();

        foreach(var empPayroll in empPayrolls)
        {
            // Если что, то поменять на почту
            var userId = empInfos.FirstOrDefault(x => x.Username == empPayroll.Name)?.Id;

            if (userId is null)
            {
                empPayrollInfo.Add($"Сотрудник {empPayroll.Name} не найден!");
                continue;
            }

            var createChannelMessage = await 
                httpClient.PostAsync("channels/direct", 
                    new StringContent($"[\"{botId}\", \"{userId}\"]", 
                    Encoding.UTF8, "application/json"));

            var createChannelResponse = await createChannelMessage.Content.ReadAsStringAsync();

            var channelId = JObject.Parse(createChannelResponse)["id"].ToString();

            string message = $"""
                   ТЕСТОВОЕ СООБЩЕНИЕ
                   #ЗарплатаЗа{empPayroll.Month}

                   Общее количество рабочих часов в прошедший месяц: {empPayroll.Hours} ч{empPayroll.Production}
                   Основная компонента зарплаты: {empPayroll.Salary}
                   {empPayroll.Bonuses}{empPayroll.Rewards}
                   **Итого начисленная зарплата:** {empPayroll.Profit}

                   | Распределение выплат ||
                   | --- | --: |
                   {empPayroll.Payments}
                   """;

            PayrollMessage payrollMessage = new PayrollMessage() 
            { 
                Channel_id = channelId, 
                Message = message
            };

            var sendMessage = await 
                httpClient.PostAsync("posts",
                    JsonContent.Create(payrollMessage));

            var sendMessageResponse = await sendMessage.Content.ReadAsStringAsync();

            if (sendMessage.IsSuccessStatusCode == false)
            {
                empPayrollInfo.Add($"Не удалось отправить сообщение сотруднику {empPayroll.Name}");
                continue;
            }

            empPayrollInfo.Add($"Сообщение отправлено успешно сотруднику {empPayroll.Name}");
        }

        return empPayrollInfo;
    }
}

