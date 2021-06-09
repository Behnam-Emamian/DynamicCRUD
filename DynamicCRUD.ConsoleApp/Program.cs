using DynamicCRUD.Data;
using DynamicCRUD.Emit;
using DynamicCRUD.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DynamicCRUD.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Initial Catalog=DhhsAdo;Integrated Security=SSPI;Trusted_Connection=yes;");
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();
            using var _dynamicDbContext = new DynamicDbContext(optionsBuilder.Options);

            var metadataEntity = new MetadataEntity
            {
                Name = "ServiceProvider",
                TableName = "ServiceProvider",
                SchemaName = "dbo"
            };
            var metadataProps = new Dictionary<string, Type>();
            metadataProps.Add("Id", typeof(Guid));
            metadataProps.Add("OrganizationName", typeof(string));
            metadataProps.Add("Address", typeof(string));

            var dynamicClassFactory = new DynamicClassFactory();
            var entityTypeBuilder = dynamicClassFactory.CreateDynamicTypeBuilder<DynamicEntity>(metadataEntity.Name, metadataProps);
            //dynamicClassFactory.CreatePropertiesForTypeBuilder(entityTypeBuilder, metadataProps, null);
            metadataEntity.EntityType = entityTypeBuilder.CreateType();

            _dynamicDbContext.AddMetadata(metadataEntity);
            
            
            var metadataQuerySet = (IQueryable<DynamicEntity>)_dynamicDbContext.GetType().GetMethod("Set").MakeGenericMethod(metadataEntity.EntityType).Invoke(_dynamicDbContext, null);
        }
    }
}
