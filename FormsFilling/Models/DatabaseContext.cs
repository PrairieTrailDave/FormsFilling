using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;



using FormFilling.Models.DatabaseModels;


//using FormFilling.Models.ViewModels.EmployeeViews;

namespace FormFilling.Models;

public class DatabaseContext : DbContext
{

    // Company tables
    public DbSet<Employer> Employers => Set<Employer>();


    // Employee Specific tables

    public DbSet<Employee> Employees => Set<Employee>();

    // Forms
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<FormItem> FormItems => Set<FormItem>();
    public DbSet<FormPrompt> FormPrompts => Set<FormPrompt>();
    public DbSet<W4Data> W4DataStorage => Set<W4Data>();
    public DbSet<SigData> Signatures => Set<SigData>();


    // report tables

    public DbSet<ReportImage> ReportImages => Set<ReportImage>();
    public DbSet<ReportLayoutField> ReportLayoutFields => Set<ReportLayoutField>();
    public DbSet<ReportFont> ReportFonts => Set<ReportFont>();

 
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);


        W4Data.OnModelCreating(builder);

    }
}
