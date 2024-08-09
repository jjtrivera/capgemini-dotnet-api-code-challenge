using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using dotnet_api_code_challenge;
using Microsoft.EntityFrameworkCore;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        // Register the DbContext with an in-memory database
        services.AddDbContext<EmployeeContext>(options =>
            options.UseInMemoryDatabase("EmployeeDb"), ServiceLifetime.Singleton
        );
        // Register the repository and custom SQL service
        services.AddScoped<EmployeeRepository>();
        services.AddScoped<EmployeeCustomService>();
        // Seed DB
        // Seed the database
        var serviceProvider = services.BuildServiceProvider();
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EmployeeContext>();
            context.Database.EnsureCreated();
            //SeedDatabase(context);
        }
    })
    .Build();

host.Run();


void SeedDatabase(EmployeeContext context)
{
    if (!context.Employees.Any())
    {
        context.Employees.AddRange(
            new Employee { Id = 1, Name = "John Doe", Position = "Software Engineer", Salary = 80000, DateOfHire = DateTime.UtcNow.AddMonths(-15) },
            new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Salary = 95000, DateOfHire = DateTime.UtcNow.AddMonths(-9) }
        );
        context.SaveChanges();
    }
}