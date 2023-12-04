using System.Net;

namespace HealthChecks.MySql.Tests.Functional;

public class mysql_healthcheck_should
{
    [Fact]
    public async Task be_healthy_when_mysql_server_is_available()
    {
        var connectionString = "server=localhost;port=3306;database=information_schema;uid=root;password=Password12!";

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddHealthChecks()
                .AddMySql(connectionString, tags: ["mysql"]);
            })
            .Configure(app =>
            {
                app.UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("mysql")
                });
            });

        using var server = new TestServer(webHostBuilder);

        using var response = await server.CreateRequest("/health").GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task be_unhealthy_when_mysql_server_is_unavailable()
    {
        var connectionString = "server=255.255.255.255;port=3306;database=information_schema;uid=root;password=Password12!";

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddHealthChecks()
                .AddMySql(connectionString, tags: ["mysql"]);
            })
            .Configure(app =>
            {
                app.UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("mysql")
                });
            });

        using var server = new TestServer(webHostBuilder);

        using var response = await server.CreateRequest("/health").GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task be_unhealthy_when_mysql_server_is_unavailable_using_options()
    {
        var connectionString = "server=255.255.255.255;port=3306;database=information_schema;uid=root;password=Password12!";

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var mysqlOptions = new MySqlHealthCheckOptions
                {
                    ConnectionString = connectionString
                };
                services.AddHealthChecks()
                    .AddMySql(mysqlOptions, tags: ["mysql"]);
            })
            .Configure(app =>
            {
                app.UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("mysql")
                });
            });

        using var server = new TestServer(webHostBuilder);

        using var response = await server.CreateRequest("/health").GetAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }
}
