using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Core.Web.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment env) => env.IsEnvironment("test");

    public static bool IsDocker(this IHostEnvironment env) => env.IsEnvironment("docker");
    public static bool IsProduction(this IHostEnvironment env) => env.IsEnvironment("Production");
}
