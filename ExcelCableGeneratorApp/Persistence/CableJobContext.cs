using ExcelCableGeneratorApp.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;


namespace ExcelCableGeneratorApp.Persistence;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// To rebuid the database: (must be built before run)
/// "dotnet ef migrations add InitialCreate"
/// "dotnet ef database update"
/// "dotnet ef migrations remove"
/// </remarks>
internal class CableJobContext : DbContext
{
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Cable> Cables { get; set; }

    public string DbPath { get; }

    public CableJobContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "cabledata.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

}
