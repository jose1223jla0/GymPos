using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GymPos.Data.DbData;

public class GymPosContextFactory : IDesignTimeDbContextFactory<GymPosContext>
{
    public GymPosContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GymPosContext>();

        optionsBuilder.UseSqlite("Data Source=gympos.db");

        return new GymPosContext(optionsBuilder.Options);
    }
}