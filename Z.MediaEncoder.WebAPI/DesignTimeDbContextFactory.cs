﻿using Microsoft.EntityFrameworkCore.Design;
using Z.CommonInitializer;
using Z.MediaEncoder.Infrastructure;

namespace Z.MediaEncoder.WebAPI
{
    //用IDesignTimeDbContextFactory坑最少，最省事
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MEDbContext>
    {
        public MEDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = DbContextOptionsBuilderFactory.Create<MEDbContext>();
            return new MEDbContext(optionsBuilder.Options, null);
        }
    }
}
