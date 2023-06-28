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
        public async Task<IActionResult> Post()
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
                //for elephantSQL use this connection string
                //string dynamicConnectionString = "Server=surus.db.elephantsql.com;Port=5432;User Id=pcleqnkw;Password=A86hU15aV-2UcNzcvAkc9GhLmY4Q98dH;Database=pcleqnkw;";

                //for azure use this connection string
                //string dynamicConnectionString = $"Server=postgres-server-unique-db.postgres.database.azure.com;Port=5432;Database={uniqueIdentifierString};User Id=postgres@postgres-server-unique-db;Password=387456;SslMode=Require;";

                var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
                optionsBuilder.UseNpgsql(dynamicConnectionString);

                using var dynamicDbContext = new MyDbContext(optionsBuilder.Options);
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
