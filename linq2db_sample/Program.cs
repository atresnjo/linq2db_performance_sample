using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using linq2db_sample.EFCore;
using linq2db_sample.Linq2Db;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Category = linq2db_sample.Linq2Db.Category;
using Company = linq2db_sample.Linq2Db.Company;
using User = linq2db_sample.Linq2Db.User;

namespace linq2db_sample
{
    public static class TestData
    {
        public static string ConnectionString =
            "Server=127.0.0.1;Port=5432; Database=test; User Id=postgres; Password=mysecretpassword";
    }

    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseNpgsql(TestData.ConnectionString);
            return new DatabaseContext(builder.Options);
        }
    }

    [SimpleJob(RunStrategy.Throughput, 1, 5, 50)]
    [MemoryDiagnoser]
    [MinColumn]
    [MaxColumn]
    public class OrmBenchmarkCreate
    {
        private static readonly TemporaryDbContextFactory _dbContextFactory = new();

        private static readonly LinqToDbConnectionOptions _options =
            new LinqToDbConnectionOptionsBuilder()
                .UseConnectionString(ProviderName.PostgreSQL, TestData.ConnectionString).Build();

        [Benchmark]
        public async Task CreateEfCore_10_Rows()
        {
            await using var context = _dbContextFactory.CreateDbContext(Array.Empty<string>());
            var service = new EFCoreService(context);
            await service.Create(10);
        }
        
        [Benchmark]
        public async Task CreateLinq2Db_10_Rows()
        {
            await using var connection = new AppDataConnection(_options);
            var service = new Linq2DbService(connection);
            await service.Create(10);
        }
        
        [Benchmark]
        public async Task CreateEfCore_100_Rows()
        {
            await using var context = _dbContextFactory.CreateDbContext(Array.Empty<string>());
            var service = new EFCoreService(context);
            await service.Create(100);
        }
        
        
        [Benchmark]
        public async Task CreateLinq2Db_100_Rows()
        {
            await using var connection = new AppDataConnection(_options);
            var service = new Linq2DbService(connection);
            await service.Create(100);
        }

        [Benchmark]
        public async Task CreateEfCore_500_Rows()
        {
            await using var context = _dbContextFactory.CreateDbContext(Array.Empty<string>());
            var service = new EFCoreService(context);
            await service.Create(500);
        }
        
        [Benchmark]
        public async Task CreateLinq2Db_500_Rows()
        {
            await using var connection = new AppDataConnection(_options);
            var service = new Linq2DbService(connection);
            await service.Create(500);
        }
        
        [Benchmark]
        public async Task CreateEfCore_1000_Rows()
        {
            await using var context = _dbContextFactory.CreateDbContext(Array.Empty<string>());
            var service = new EFCoreService(context);
            await service.Create(1000);
        }
        
        [Benchmark]
        public async Task CreateLinq2Db_1000_Rows()
        {
            await using var connection = new AppDataConnection(_options);
            var service = new Linq2DbService(connection);
            await service.Create(1000);
        }
  
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            PostgreSQLTools.DefaultBulkCopyType = BulkCopyType.ProviderSpecific;
            BenchmarkRunner.Run<OrmBenchmarkCreate>();
        }

        private static void CreateDb()
        {
            var options =
                new LinqToDbConnectionOptionsBuilder()
                    .UseConnectionString(ProviderName.PostgreSQL, TestData.ConnectionString)
                    .Build();

            using var connection = new AppDataConnection(options);
            connection.CreateTable<User>();
            connection.CreateTable<Contract>();
            connection.CreateTable<Company>();
            connection.CreateTable<Category>();
        }
    }
}