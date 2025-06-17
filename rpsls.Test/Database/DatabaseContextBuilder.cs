using Microsoft.EntityFrameworkCore;
using rpsls.Infrastructure.Database;

namespace rpsls.Test.Database;

public class DatabaseContextBuilder
{
    public static ApplicationDbContext BuildDbContext(string databaseName)
    {
        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(dbContextOptions);
    }
}