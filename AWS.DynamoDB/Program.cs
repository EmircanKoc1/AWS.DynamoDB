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





app.Run();
