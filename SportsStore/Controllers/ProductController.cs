using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;

namespace SportsStore.Controllers
{
    public class ProductController: Controller
    {
        private IProductRepository _repository;
        public int PageSize = 3;

        public ProductController(IProductRepository repo)
        {
            _repository = repo;
        }

        public ViewResult List(string category, int productPage = 1) => View(new ProductsListViewModel
        {
            Products = _repository.Products
                .Where(p => p.Category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = category == null ? _repository.Products.Count() : 
                    _repository.Products.Where(p => p.Category == category).Count()
            },
            CurrentCategory = category
        });
        
    }
}
