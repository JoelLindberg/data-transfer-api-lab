using Microsoft.EntityFrameworkCore;

using DataTransferApiLab.Models;

namespace DataTransferApiLab.Data;


public class DataTransferApiLabContext : DbContext
{
    public DataTransferApiLabContext (DbContextOptions<DataTransferApiLabContext> options)
        : base(options) { }

    public DbSet<Transfer> Transfers { get; set; }
}