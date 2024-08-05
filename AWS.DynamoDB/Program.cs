using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
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
    if (await _context.LoadAsync<Employee>(id) is Employee employee)
        return Results.Ok(employee);

    return Results.NotFound("entity not found");
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

app.MapGet("/list-tables", async (
    [FromServices] IAmazonDynamoDB _amazonDynamoDb,
    [FromQuery] int limit) =>
{
    var tableListRequest = new ListTablesRequest()
    {
        Limit = limit
    };

    return Results.Ok(await _amazonDynamoDb.ListTablesAsync(tableListRequest));

});

app.MapDelete("/delete-table-by-name", async (
    [FromServices] IAmazonDynamoDB _amazonDynamoDb,
    [FromQuery] string tableName) =>
{


    var desribeTableRequest = new DescribeTableRequest()
    {
        TableName = tableName
    };

    try
    {
        await _amazonDynamoDb.DescribeTableAsync(desribeTableRequest);
    }
    catch (ResourceNotFoundException ex)
    {
        return Results.NotFound("table not found !");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

    var deletedTableRequest = new DeleteTableRequest()
    {
        TableName = tableName
    };

    var deleteTableResponse = await _amazonDynamoDb.DeleteTableAsync(deletedTableRequest);

    return Results.Ok(deleteTableResponse);

});

app.Run();
