using EmployeePayments.Interfaces;
using EmployeePayments.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("PaymentBot", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://mattermost.singularis-lab.com/api/v4/");
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ofxaww1dntr87cbjmw4kp7gqka");
});

builder.Services.AddTransient<IParser, Parser>();
builder.Services.AddTransient<ISender, Sender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
