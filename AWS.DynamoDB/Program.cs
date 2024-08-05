using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AWS.DynamoDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

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






app.Run();
