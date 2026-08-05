// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es el "puente" entre C# y SQL Server — la clase central de Entity Framework
//           Core. Expone un DbSet<T> por cada tabla (ej. Users, Services), que se comporta
//           como una colección en memoria pero en realidad, cuando se hace la consulta,
//           EF la traduce a SQL y la ejecuta contra la base de datos real. En
//           OnModelCreating() carga automáticamente TODAS las clases de
//           Data/Configurations (una por entidad) sin tener que listarlas a mano.
// Entidades relacionadas: TODAS (User, Professional, WorkType, Service, ServiceProfessional,
//                          Review, Payment, Chat, Penalty, Reward)
// Tablas relacionadas: TODAS las TBL_* de la base de datos
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
