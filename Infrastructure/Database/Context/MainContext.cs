using Application.Abstractions.Data;
using Core.Domain.Authentications;
using Core.Domain.Department;
using Core.Domain.Employee;
using Core.Domain.Leave;
using Core.Domain.Users;
using Infrastructure.Database.Seeds;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Context;

public class MainContext: IdentityDbContext<User>, IApplicationDbContext
{
    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }
    #region DbSet section
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Leave> Leaves { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<InvalidTokens> InvalidTokens { get; set; } = null!;


    #endregion
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // builder.ApplyConfiguration(new DepartmentSeed());

        builder.Entity<User>()
            .HasIndex(u => u.EmployeeId)
            .IsUnique();
        
        builder.Entity<Leave>()
            .HasOne(l => l.Employee)
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // builder.Entity<User>()
        //     .HasOne(u => u.Department)
        //     .WithMany()
        //     .HasForeignKey(u => u.DepartmentId)
        //     .OnDelete(DeleteBehavior.SetNull);
        
        // builder.Entity<Employee>()
        //     .HasOne(e => e.User)
        //     .WithMany()
        //     .HasForeignKey(e => e.UserId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
    
    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void SetAuditFields()
    {
        var now = DateTime.UtcNow;

        // Iterate over all tracked entities that inherit BaseEntity<int>
        foreach (var entry in ChangeTracker.Entries<Core.Domain.BaseEntity.BaseEntity<int>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
    
}