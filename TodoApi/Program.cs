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

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
// Console.WriteLine();
System.Console.WriteLine($"Connection String: {connectionString}");
builder.Services.AddDbContext<ToDoDbContext>(options =>

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 
var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();  
app.UseSwaggerUI
(c =>
{
    
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");;
    c.RoutePrefix = string.Empty;
});

app.MapGet("/ToDos", async(ToDoDbContext db) =>{return await db.Items.ToListAsync();} );
app.MapPost("ToDos/", (Item item, ToDoDbContext db) => 
{
    db.Items.Add(item);
    db.SaveChanges(); 
    return Results.Created($"/{item.Id}", item); 
});
app.MapPut("/ToDos/{id}", ( ToDoDbContext db,int id, bool IsComplete) =>
{
    var it = db.Items.Find(id);
    if (it == null) return Results.NotFound();
    else
    {
        it.IsComplete =IsComplete;
        db.SaveChanges();
        return Results.Ok(it);
    }
});

app.MapDelete("/ToDos/{id}", (int id, ToDoDbContext db) =>
{
    var item = db.Items.Find(id);
    if (item == null) return Results.NotFound();
    db.Items.Remove(item);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/",()=>"ToDoApi server is runing");
app.Run();