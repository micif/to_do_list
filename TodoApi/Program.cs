using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת מדיניות CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.WithOrigins("https://to-do-list-yubg.onrender.com")  // כתובת הלקוח שלך
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// קביעת החיבור לבסיס נתונים
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

var app = builder.Build();

// הפעלת Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// שימוש במדיניות CORS
app.UseCors("AllowAll");

app.MapGet("/", async (ToDoDbContext context) =>
{
    return Results.Ok("Welcome");
});

app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    var tasks = await context.Tasks.ToListAsync();
    return Results.Ok(tasks);
});

app.MapPost("/tasks", async (TodoApi.Task task, ToDoDbContext context) =>
{
    context.Tasks.Add(task);
    await context.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapPut("/tasks/{id}", async (int id, TodoApi.Task task, ToDoDbContext context) =>
{
    var existingTask = await context.Tasks.FindAsync(id);
    if (existingTask != null)
    {
        existingTask.Name = task.Name;
        existingTask.IsComplete = task.IsComplete;
        await context.SaveChangesAsync();
        return Results.Ok(existingTask);
    }
    return Results.NotFound();
});

app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext context) =>
{
    var task = await context.Tasks.FindAsync(id);
    if (task != null)
    {
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
