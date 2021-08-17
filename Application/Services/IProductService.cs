using Application.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProductService
    {
        Task<ProductWriteResponse> BulkWrite(IEnumerable<ProductWriteRequestModel> products);
        Task<List<ProductListItemModel>> ListProducts();
    }
}
