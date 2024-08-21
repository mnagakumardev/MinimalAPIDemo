using MinimalAPIDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


var todoItems = new List<TodoItem>();

app.MapGet("/todoitems", () => todoItems);

app.MapGet("/todoitems/{id}", (int id) => todoItems.FirstOrDefault(t => t.Id == id));

app.MapPost("/todoitems", (TodoItem todo) => {
    todo.Id = todoItems.Count + 1;
    todoItems.Add(todo);
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", (int id, TodoItem updatedTodo) => {
    var todo = todoItems.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();
    todo.Name = updatedTodo.Name;
    todo.IsComplete = updatedTodo.IsComplete;
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", (int id) => {
    var todo = todoItems.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();
    todoItems.Remove(todo);
    return Results.NoContent();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
