﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit.Abstractions;
using TestData = Datasync.Common.TestData;

namespace Microsoft.AspNetCore.Datasync.EFCore.Tests;

[ExcludeFromCodeCoverage]
public class AzureSqlDbContext : DbContext
{
    public static AzureSqlDbContext CreateContext(string connectionString, ITestOutputHelper output = null)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        DbContextOptionsBuilder<AzureSqlDbContext> optionsBuilder = new DbContextOptionsBuilder<AzureSqlDbContext>()
            .UseSqlServer(connectionString);
        if (output != null)
        {
            optionsBuilder.UseLoggerFactory(new TestLoggerFactory(output, new string[] { "Microsoft.EntityFrameworkCore.Database.Command" }));
            optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
        }
        AzureSqlDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        TestData.Movies.OfType<EntityMovie>().ForEach(movie => context.Movies.Add(movie));
        context.SaveChanges();
        return context;
    }

    public AzureSqlDbContext(DbContextOptions<AzureSqlDbContext> options) : base(options)
    {
    }

    public DbSet<EntityMovie> Movies { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("date");
        base.ConfigureConventions(configurationBuilder);
    }

    internal class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d))
        {
        }
    }
}