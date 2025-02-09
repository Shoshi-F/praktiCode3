using TodoApi;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// קבלת מחרוזת החיבור
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string 'ToDoDB' is missing or empty in appsettings.json");
}

Console.WriteLine($"Connection String: {connectionString}");

try
{
    builder.Services.AddDbContext<ToDoDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    );
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring database: {ex.Message}");
    throw;
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    c.RoutePrefix = string.Empty;
});

app.MapGet("/ToDos", async (ToDoDbContext db) => await db.Items.ToListAsync());

app.MapPost("/ToDos/", async (Item item, ToDoDbContext db) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/{item.Id}", item);
});

app.MapPut("/ToDos/{id}", async (ToDoDbContext db, int id, bool IsComplete) =>
{
    var it = await db.Items.FindAsync(id);
    if (it == null) return Results.NotFound();

    it.IsComplete = IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(it);
});

app.MapDelete("/ToDos/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/", () => "ToDoApi server is running");

app.Run();
