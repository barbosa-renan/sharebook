using Sharebook.Core.Interfaces;
using Sharebook.Core.Models;

namespace Sharebook.Infra.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContextClass dbContext) : base(dbContext)
        {

        }        
    }
}
