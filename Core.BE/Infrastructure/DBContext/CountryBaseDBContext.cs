using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Emeint.Core.BE.Infrastructure.DBContext
{
    public class CountryBaseDBContext : DbContext
    {
        public string CountryCode { get; set; }
        private readonly IIdentityService _identityService;
        private readonly SQLConnectionStringOptions _sqlConnections;
        private readonly IConfiguration _configuration;
        public CountryBaseDBContext()
        {

        }
        public CountryBaseDBContext(DbContextOptions<DbContext> options, IConfiguration configuration, IIdentityService identityService)
            : base(options)
        {
            _configuration = configuration;
            _identityService = identityService;
            _sqlConnections = new SQLConnectionStringOptions(configuration);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string connectionString = GetCountryDBConnectionString(_configuration);

            #region TO BE Removed after adding the new setting to all microservices
            if (string.IsNullOrEmpty(connectionString))
                connectionString = _configuration["ConnectionString"];
            #endregion

            if (_configuration["TestMood"] == "0")
            {
                optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    string commandTimeout = _configuration["DbCommandTimeout"];
                    if (string.IsNullOrEmpty(commandTimeout))
                        commandTimeout = "90";


                    sqlOptions.CommandTimeout(Convert.ToInt32(commandTimeout));
                });

                //enable entites or properties data to save in logging or in Exceptions
                optionsBuilder.EnableSensitiveDataLogging();
            }

            //for Unit test 
            //else
            //{
            //    optionsBuilder.UseInMemoryDatabase("OrderingDb");
            //}
        }

        private string GetCountryDBConnectionString(IConfiguration configuration)
        {

            var connections = _sqlConnections?.Connections;

            string connection = string.Empty;

            if (string.IsNullOrEmpty(CountryCode))
                CountryCode = _identityService?.CountryCode ?? string.Empty;

            if (!string.IsNullOrEmpty(CountryCode))
                connection = connections?.FirstOrDefault(c => c.Key == CountryCode).Value;

            if (string.IsNullOrEmpty(connection))
                connection = connections?.FirstOrDefault().Value;

            return connection;
        }
    }
}
