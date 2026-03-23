using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Integration.Test.Support;

public abstract class RepositoryTestBase
{
    protected RepositoryTestBase()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GeneralProfile).Assembly);
        });

        Mapper = mapperConfig.CreateMapper();
    }

    protected IMapper Mapper { get; }

    protected ApplicationDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
