using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;

namespace EmployeePayments.Pages;

public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string _botToken = "ofxaww1dntr87cbjmw4kp7gqka";
    private int _empIndex = 4;

    public IndexModel(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
    {
        _environment = environment;
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public IFormFile Upload { get; set; }

    public async Task OnPostAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("PaymentBot");
        using var fileStream = Upload.OpenReadStream();

        var workbook = new XLWorkbook(Upload.OpenReadStream());
        var ws = workbook.Worksheet(1);

        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://mattermost.singularis-lab.com/api/v4"),
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ofxaww1dntr87cbjmw4kp7gqka");

        HttpResponseMessage empInfoRequest = await httpClient.GetAsync("/users");

        var empInfoResponse = await empInfoRequest.Content.ReadAsStringAsync();

        List<EmployeeInfo> empInfos = JsonConvert.DeserializeObject<List<EmployeeInfo>>(empInfoResponse);

        var botId = empInfos.First(x => x.Username == "salarybot").Id;

        var month = ws.Cell(2, 1).Value.ToString();

        bool isGPH = false;

        var empPayrolls = new List<EmployeePayroll>();

        while (true)
        {
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

            var empPayroll = new EmployeePayroll()
            {
                Name = empName,
                Hours = ws.Cell(2, _empIndex).GetFormattedString(),
                Production = isGPH == true ? $"\r\nВыработка за месяц: {ws.Cell(2, _empIndex).GetFormattedString()} ставки" : "",
                Salary = ws.Cell(10, _empIndex).GetFormattedString(),
                Profit = ws.Cell(24, _empIndex).GetFormattedString(),
                Bonuses = GetBonuses(ws, _empIndex),
                Rewards = GetRewards(ws, 17, _empIndex),
                Payments = GetPayments(ws, 26, _empIndex),
            };

            var userId = empInfos.FirstOrDefault(x => x.Username == empPayroll.Name)?.Id;

            if (userId is not null)
            {
                var json = JsonConvert.SerializeObject(new string[2] { botId, userId });
                var createChannelRequest = await httpClient.PostAsync("/channels/direct", new StringContent(json, Encoding.UTF8, "application/json"));

                var createChannelResponse = await createChannelRequest.Content.ReadAsStringAsync();

                ResponseDirect testDirect = JsonConvert.DeserializeObject<ResponseDirect>(createChannelResponse);

                string temp = $"""
                   #ЗарплатаЗа{month}

                   Общее количество рабочих часов в прошедший месяц: {empPayroll.Hours} ч{empPayroll.Production}
                   Основная компонента зарплаты: {empPayroll.Salary}
                   {empPayroll.Bonuses}{empPayroll.Rewards}
                   **Итого начисленная зарплата:** {empPayroll.Profit}

                   | Распределение выплат ||
                   | --- | --: |
                   {empPayroll.Payments}
                   """;

                RequestHelloWorld world = new RequestHelloWorld() { channel_id = testDirect.Id, message = temp };

                JsonContent jsonContent = JsonContent.Create(world);

                var sendMessageRequest = await httpClient.PostAsync("/posts", jsonContent);

                var sendMessageResponse = await sendMessageRequest.Content.ReadAsStringAsync();

            }

            //employees.Add(employee);

            _empIndex++;
        }
    }
}
