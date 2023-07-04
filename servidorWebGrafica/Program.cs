using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearRegression;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.MapPost("/api/calculator", async context =>
{
    var request = await context.Request.ReadFromJsonAsync<CalculationRequest>();

    int sum = request.Number1 + request.Number2;
    int subtract = request.Number1 - request.Number2;

    var result = new CalculationResult
    {
        Sum = sum,
        Subtract = subtract
    };

    var resultJson = JsonConvert.SerializeObject(result);

    await context.Response.WriteAsync(resultJson);
});

app.MapPost("/api/regresion", async context =>
{
    var data = await context.Request.ReadFromJsonAsync<Dictionary<int, double>>();
    List<int> years = new List<int>(data.Keys);
    List<double> households = new List<double>(data.Values);
    (double pendiente, double interceptor) = CalculateLinearRegression(years, households);
    var result = new { Pendiente = pendiente, Interceptor = interceptor };
    await context.Response.WriteAsJsonAsync(result);
});

(double pendiente, double interceptor) CalculateLinearRegression(List<int> years, List<double> pc)
{
    double sumA�os = 0;
    double sumPc = 0;
    double sumA�osPc = 0;
    double sumA�osCuadrados = 0;

    for (int i =0; i< years.Count; i++)
    {
        sumA�os += years[i];
        sumPc += pc[i];
        sumA�osPc += pc[i] * years[i];
        sumA�osCuadrados += Math.Pow(years[i], 2);
    }

    int n = years.Count;
    double pendiente = (n * sumA�osPc - sumA�os * sumPc) /(n * sumA�osCuadrados - Math.Pow(sumA�os, 2));
    double interceptor = (sumPc - pendiente * sumA�os) /n;


    return (pendiente, interceptor);
}


app.UseAuthorization();

app.MapRazorPages();

app.Run();


public class CalculationRequest
{
    public int Number1 { get; set; }
    public int Number2 { get; set; }
}


public class CalculationResult
{
    public int Sum { get; set; }
    public int Subtract { get; set; }
}
