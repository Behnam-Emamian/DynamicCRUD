using DynamicCRUD.Data;
using DynamicCRUD.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DynamicCRUD.ConsoleApp
{
    class Program
    {
        private readonly static MetadataHolder metadataHolderFromJson = new MetadataHolder
        {
            Version = "1",
            Entities = new List<MetadataEntity>
                {
                    new MetadataEntity
                    {
                        Name = "ServiceProvider",
                        TableName = "ServiceProvider",
                        SchemaName = "dbo",
                        Properties = new List<MetadataEntityProperty>
                        {
                            new MetadataEntityProperty
                            {
                                Name = "Id",
                                Type = "Guid",
                                ColumnName = "Id"
                            },
                            new MetadataEntityProperty
                            {
                                Name = "OrganizationName",
                                Type = "String",
                                ColumnName = "OrganizationName"
                            },
                            new MetadataEntityProperty
                            {
                                Name = "Address",
                                Type = "String",
                                ColumnName = "Address"
                            }
                        }
                    }
                }
        };

        static void Main()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Initial Catalog=DhhsAdo;Integrated Security=SSPI;Trusted_Connection=yes;");
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();
            using var dynamicDbContext = new DynamicDbContext(optionsBuilder.Options);

            dynamicDbContext.SetMetadataHolder(metadataHolderFromJson);

            var metadataQuerySet = dynamicDbContext.GetDbSetQuery(dynamicDbContext.MetadataEntities[0].Name);

            var response = metadataQuerySet.ToDynamicList();
        }
    }
}
