using Microsoft.Extensions.DependencyInjection;

namespace ExcelFaceplateDataExtractorApp.StartupHelpers;

public static class ServiceExtensions
{
    public static void AddWpfComponentFactory<T>(this IServiceCollection services)
        where T : class
    {
        services.AddTransient<T>(); // Register the type reference
        services.AddSingleton<Func<T>>(x => () => x.GetService<T>()!); // Register IServiceProvider service locator function for AbstractFactory (compiler complains, null-forgiving)
        services.AddSingleton<IAbstractFactory<T>, AbstractFactory<T>>(); // Register the factory (injected with IServiceProvider service locator func)
    }
}
