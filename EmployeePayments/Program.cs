using EmployeePayments;
using EmployeePayments.Interfaces;
using EmployeePayments.Services;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("PaymentBot", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://mattermost.singularis-lab.com/api/v4/");
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
});

builder.Services.AddTransient<IParser, Parser>();
builder.Services.AddTransient<ISender, Sender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

Process.Start("cmd", "/C start http://localhost:5000");
app.Run();