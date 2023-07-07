using auto_deployment_unique_db;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using auto_deployment_unique_db.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register MyDbContext with the local connection string
string localConnectionString = "Server=localhost;Port=5432;User Id=postgres;Password=387456";
//for elephantSQL use this connectionstring
//string elephantSqlConnectionString = "Server=surus.db.elephantsql.com;Port=5432;User Id=pcleqnkw;Password=A86hU15aV-2UcNzcvAkc9GhLmY4Q98dH;Database=pcleqnkw;";

//for azure use this connectionstring
//string azureConnectionString = "Server=postgres-server-unique-db.postgres.database.azure.com;Port=5432;User Id=postgres@postgres-server-unique-db;Password=387456;SslMode=Require;";
builder.Services.AddDbContext<MyDbContext>(options => options.UseNpgsql(localConnectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
       .AllowAnyMethod()
          .AllowAnyHeader());



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();