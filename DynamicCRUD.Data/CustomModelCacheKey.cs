using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DynamicCRUD.Data
{
    public class CustomModelCacheKey : ModelCacheKey
    {
        private readonly string _contextVersion;
        public CustomModelCacheKey(DbContext context)
            : base(context)
        {
            _contextVersion = (context as DynamicDbContext)?.Version;
        }

        protected override bool Equals(ModelCacheKey other)
         => base.Equals(other)
            && (other as CustomModelCacheKey)?._contextVersion == _contextVersion;

        public override int GetHashCode() => base.GetHashCode();
    }
}