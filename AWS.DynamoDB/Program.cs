using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AWS.DynamoDB.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/get-employees", async (
    [FromServices] IDynamoDBContext _dynamoDbcontext) =>
{

    var employees = await _dynamoDbcontext
                                           .ScanAsync<Employee>(default)
                                           .GetRemainingAsync();

    return employees;
});


app.MapGet("/get-employee-by-id", async (
    [FromServices] IDynamoDBContext _context,
    [FromQuery] int id) =>
{
    var employee = await _context.LoadAsync<Employee>(id);

    return Results.Ok(employee);
});

app.MapPost("/add-employee", async (
    [FromServices] IDynamoDBContext _context,
    [FromBody] Employee employee) =>
{

    await _context.SaveAsync<Employee>(employee);

    return Results.Ok();

});


app.MapPut("/update-employee-by-id", async (
    [FromServices] IDynamoDBContext _context,
    [FromBody] Employee employee) =>
{
    if (await _context.LoadAsync<Employee>(employee.Id) is null)
        return Results.NotFound("entity not found!");

    await _context.SaveAsync(employee);
    return Results.Ok();

});


app.MapDelete("/delete-employee-by-id", async (
    [FromServices] IDynamoDBContext _context,
    [FromQuery] int id) =>
{
    if (await _context.LoadAsync<Employee>(id) is null)
        return Results.NotFound("entity not found");

    await _context.DeleteAsync<Employee>(id);
    return Results.Ok("entity was deleted");
});


app.Run();
