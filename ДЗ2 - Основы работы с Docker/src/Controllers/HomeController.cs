using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using src.Models;

namespace src.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IConfiguration configuration,
        ILogger<HomeController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/health/")]
    public IActionResult Health() {
        return Json(new { Status = "OK" });
    }

    [Route("/serverError/")]
    public IActionResult ServerError() {
        throw new Exception("Some server error.");
    }

    [Route("/readCfg/")]
    public IActionResult ReadCfg() {
        return Json(new { MyKey = _configuration["ASPNETCORE_OTHERKEY"] });
    }

    [Route("/select/")]
    public IActionResult Select(string host, int? port) {
        if (string.IsNullOrWhiteSpace(host)) { host = "arch.homework"; }
        if (port == null) { port = 30000; }
        var password = Environment.GetEnvironmentVariable("PGPASSWORD");
        var connectionString = $"Host={host}:{port};Username=postgres;Password={password};Database=test";

        try {
            using var dataSource = NpgsqlDataSource.Create(connectionString);
            var result = new List<string>();
            using (var cmd = dataSource.CreateCommand("SELECT code FROM public.\"test-table-1\""))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            return Json(new {
                Status = "OK",
                Host = host,
                Port = port,
                Result = result,
            });
        } catch (Exception ex) {
            return Json(new {
                Status = "Error",
                Host = host,
                Port = port,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
            });
        }
    }
}
