using auto_deployment_unique_db.Data;
using auto_deployment_unique_db.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace auto_deployment_unique_db.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public DatabaseController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DynamicTableRequest dynamicTableRequest)
        {
            Guid uniqueIdentifier = Guid.NewGuid();
            string uniqueIdentifierString = uniqueIdentifier.ToString();

            try
            {
                using (var connection = _dbContext.Database.GetDbConnection() as NpgsqlConnection)
                {
                    if (connection != null)
                    {
                        await connection.OpenAsync();
                    }

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"CREATE DATABASE \"{uniqueIdentifierString}\"";
                        await command.ExecuteNonQueryAsync();
                    }
                    if (connection != null)
                    {
                        await connection.CloseAsync();
                    }

                }

                //local connection string
                string dynamicConnectionString = $"Server=localhost;Port=5432;Database={uniqueIdentifierString};User Id=postgres;Password=387456";

                var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
                optionsBuilder.UseNpgsql(dynamicConnectionString);

                using var dynamicDbContext = new MyDbContext(optionsBuilder.Options);
                dynamicDbContext.CreateTablesAndColumns(dynamicTableRequest);
                dynamicDbContext.AddPrimaryKeys(dynamicTableRequest);
                dynamicDbContext.AddForeignKeys(dynamicTableRequest);
                dynamicDbContext.AddIsNullable(dynamicTableRequest);
                dynamicDbContext.AddIsUnique(dynamicTableRequest);
                dynamicDbContext.SaveChanges();
                await dynamicDbContext.Database.EnsureCreatedAsync();
                await dynamicDbContext.Database.MigrateAsync();

                return Ok(new DatabaseModel { UniqueIdentifier = uniqueIdentifierString });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create database", error = ex.Message });
            }
        }
    }
}