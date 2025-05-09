using System.Globalization;
using Bogus;
using BuildingBlocks.Core.Web;
using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Notification.Api.Extensions.ApplicationBuilderExtensions;
using Spectre.Console;

AnsiConsole.Write(new BarChart()
    .Width(100)
    .Label("[green bold underline]@Developed by Mahmud Yahyayev (%)[/]")
    .CenterLabel()
    .AddItem("Mahmud Yahyayev", 100, Color.Green));


AnsiConsole.Write(
    new FigletText("Notification Service")
        .Centered()
        .Color(Color.FromInt32(new Faker().Random.Int(1, 255))));


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(
    (context, options) =>
    {
        options.ValidateScopes =
            context.HostingEnvironment.IsDevelopment()
            || context.HostingEnvironment.IsTest()
            || context.HostingEnvironment.IsStaging();
    }
);

builder.Services
    .AddControllers(
        options => options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()))
    )
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
    .AddControllersAsServices();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatedOptions<AppOptions>();

builder.AddModulesServices();
builder.AddMinimalEndpoints();

//Default culture
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

await app.ConfigureModules();

app.UseAppCors();

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.MapModulesEndpoints();

app.MapMinimalEndpoints();

app.MapGet(
    "/test",
    context =>
    {
        return Task.FromResult("test");
    }
);

app.UseCustomSwagger();

await app.RunAsync();

