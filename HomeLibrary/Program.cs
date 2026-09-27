using HomeLibrary.Data;
using HomeLibrary.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("HomeLibrary")
    ?? throw new InvalidOperationException("Connection string 'HomeLibrary' is not configured.");

builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
builder.Services.AddScoped<IBookRepository, NpgsqlBookRepository>();
builder.Services.AddSingleton<ITocService, TocService>();
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Демо-данные нужны только при локальной разработке. В Production загрузчик
// выключен, иначе приложение молча наполнило бы боевую базу вымышленными
// книгами. Флаг в конфигурации позволяет включить его явно.
if (app.Configuration.GetValue("Seeding:Enabled", app.Environment.IsDevelopment()))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
}

app.UseExceptionHandler("/error");
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

// BooksController размечен атрибутами, поэтому обычный маршрут его не
// обслуживает: /Books работает через [HttpGet("")], а корень сайта
// иначе отдаёт 404. Редирект вводит пользователя в список книг.
app.MapGet("/", () => Results.Redirect("/Books"));

app.Run();
