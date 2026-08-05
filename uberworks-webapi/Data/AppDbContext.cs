// =====================================================================================
// FILE SUMMARY
// What it does: This is the "bridge" between C# and SQL Server — the central Entity
//               Framework Core class. Exposes a DbSet<T> per table (e.g. Users, Services),
//               which behaves like an in-memory collection but, when queried, EF translates
//               it into SQL and runs it against the real database. OnModelCreating()
//               automatically loads ALL the classes in Data/Configurations (one per entity)
//               without having to list them by hand.
// Entities connected: ALL (User, Professional, WorkType, Service, ServiceProfessional,
//                      Review, Payment, Chat, Penalty, Reward, ErrorLog, UserActionLog,
//                      AdminActionLog)
// Tables related: ALL the TBL_* tables in the database
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceProfessional> ServiceProfessionals => Set<ServiceProfessional>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Penalty> Penalties => Set<Penalty>();
    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
    public DbSet<UserActionLog> UserActionLogs => Set<UserActionLog>();
    public DbSet<AdminActionLog> AdminActionLogs => Set<AdminActionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
