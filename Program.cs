using Blazored.Toast;
using LiveChartsCore;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Omreznina;
using System.Diagnostics;
//Trace.Listeners.Add(new ConsoleTraceListener());
//LiveCharts.EnableLogging = true;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
