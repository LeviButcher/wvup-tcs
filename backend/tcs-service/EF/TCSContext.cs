using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using tcs_service.Models;

namespace tcs_service.EF
{
  public class TCSContext : DbContext
  {
    public DbSet<ClassTour> ClassTours { get; set; }

    public TCSContext()
    {

    }

    public TCSContext(DbContextOptions options) : base(options)
    {
        try
        {
            Database.Migrate();
        }
        catch (Exception ex)
        {
            // logger.Error("Exception running migrations. ", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
  }
}
