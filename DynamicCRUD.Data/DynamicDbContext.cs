using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using DynamicCRUD.Metadata;
using System.Reflection.Emit;
using DynamicCRUD.Emit;

namespace DynamicCRUD.Data
{
    public class DynamicDbContext : DbContext
    {
        public string Version { get; set; }
        public List<MetadataEntity> MetadataEntities { get; set; }

        public DynamicDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var metadataEntity in MetadataEntities)
            {
                modelBuilder.Entity(metadataEntity.EntityType).ToTable(metadataEntity.TableName, metadataEntity.SchemaName).HasKey("Id");

                foreach (var metaDataEntityProp in metadataEntity.Properties)
                {
                    if (!metaDataEntityProp.IsNavigation)
                    {
                        var propBuilder = modelBuilder.Entity(metadataEntity.EntityType).Property(metaDataEntityProp.Name);

                        if (!string.IsNullOrEmpty(metaDataEntityProp.ColumnName))
                            propBuilder.HasColumnName(metaDataEntityProp.ColumnName);
                    }
                }
            }
            base.OnModelCreating(modelBuilder);
        }

        public void SetMetadataHolder(MetadataHolder metadataHolder)
        {
            if (metadataHolder == null)
                throw new NullReferenceException("Metadata load failed.");

            if (Version != metadataHolder.Version)
            {
                var entityTypeBuilderList = new Dictionary<string, TypeBuilder>();

                MetadataEntities = new List<MetadataEntity>();

                var dynamicClassFactory = new DynamicClassFactory();
                foreach (var metadataEntity in metadataHolder.Entities)
                {
                    var metadataProps = new Dictionary<string, Type>();
                    foreach (var metaDataEntityProp in metadataEntity.Properties.Where(p => !p.IsNavigation))
                    {
                        // TODO YASIN datetime vb eklenebilir.
                        switch (metaDataEntityProp.Type)
                        {
                            case "String":
                                metadataProps.Add(metaDataEntityProp.Name, typeof(string));
                                break;
                            case "Int":
                                metadataProps.Add(metaDataEntityProp.Name, typeof(int));
                                break;
                            case "Guid":
                                metadataProps.Add(metaDataEntityProp.Name, typeof(Guid));
                                break;
                            default:
                                //Implement for Other types
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(metadataEntity.CustomAssemblyType))
                    {
                        var entityTypeBuilder = dynamicClassFactory.CreateDynamicTypeBuilder<DynamicEntity>(metadataEntity.Name, metadataProps);
                        entityTypeBuilderList.Add(metadataEntity.Name, entityTypeBuilder);
                    }
                    else
                    {
                        metadataEntity.EntityType = Type.GetType(metadataEntity.CustomAssemblyType);
                    }

                    MetadataEntities.Add(metadataEntity);
                }

                foreach (var metadataEntity in MetadataEntities)
                {
                    var existEntityTypeBuilder = entityTypeBuilderList.FirstOrDefault(p => p.Key == metadataEntity.Name).Value;

                    foreach (var metaDataEntityProp in metadataEntity.Properties.Where(p => p.IsNavigation))
                    {
                        var relatedEntityTypeBuilder = entityTypeBuilderList.FirstOrDefault(p => p.Key == metaDataEntityProp.Type).Value;

                        var relatedProp = new Dictionary<string, Type>();

                        if (metaDataEntityProp.NavigationType == "Single")
                        {
                            relatedProp.Add(metaDataEntityProp.Name, relatedEntityTypeBuilder);
                        }
                        else
                        {
                            var listGenericType = typeof(List<>);

                            var listRelateEntityType = listGenericType.MakeGenericType(relatedEntityTypeBuilder);

                            relatedProp.Add(metaDataEntityProp.Name, listRelateEntityType);
                        }
                        new DynamicClassFactory().CreatePropertiesForTypeBuilder(existEntityTypeBuilder, relatedProp, null);
                    }
                    metadataEntity.EntityType = existEntityTypeBuilder.CreateType();
                }

                Version = metadataHolder.Version;
            }
        }

        public IQueryable<DynamicEntity> GetDbSetQuery(string dbSetName)
        {
            var metadataEntity = MetadataEntities.Find(metadataEntity => metadataEntity.Name == dbSetName);
            if (metadataEntity == null)
            {
                return default;
            }
            return (IQueryable<DynamicEntity>)GetType().GetMethod("Set", Array.Empty<Type>()).MakeGenericMethod(metadataEntity.EntityType).Invoke(this, null);
        }
    }
}
