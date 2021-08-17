using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IOptions<DatabaseSettings> dbSettingsOptions) : base(dbSettingsOptions)
        {
            base.SetCollection("Products");
        }
    }
}
