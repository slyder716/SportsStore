﻿using Moq;
using SportsStore.Models;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using SportsStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SportsStore.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void IndexContainsAllProducts()
        {
            // Arrange -- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }.AsQueryable<Product>());

            // Arrange -- create a controller
            AdminController target = new AdminController(mock.Object);

            // Action
            Product[] result = GetViewModel<IEnumerable<Product>>(target.Index())?.ToArray();

            // Assert 
            Assert.Equal(3, result.Length);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
            Assert.Equal("P3", result[2].Name);
        }

        [Fact]
        public void CanEditProduct()
        {
            // Arrange -- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" },
                new Product { ProductID = 4, Name = "P4" }
            }.AsQueryable<Product>());

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product p1 = GetViewModel<Product>(target.Edit(1));
            Product p2 = GetViewModel<Product>(target.Edit(2));
            Product p3 = GetViewModel<Product>(target.Edit(3));

            // Assert 
            Assert.Equal(1, p1.ProductID);
            Assert.Equal(2, p2.ProductID);
            Assert.Equal(3, p3.ProductID);
        }

        [Fact]
        public void CannotEditNonexistentProduct()
        {
            // Arrange -- create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }.AsQueryable<Product>());

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product result = GetViewModel<Product>(target.Edit(4));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanSaveValidChanges()
        {
            // Arrange -- create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create mock temp data
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object)
            {
                TempData = tempData.Object
            };
            //Arrange - create a product
            Product product = new Product { Name = "Test" };

            // Act - try to save the product
            IActionResult result = target.Edit(product);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));

            // Assert - check the result type is a redirection
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void CannotSaveInvalidChanges()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Arrange - create a controller
            AdminController target = new AdminController(mock.Object);

            //Arrage - create product
            Product product = new Product { Name = "Test" };

            // Arrange - add error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act
            IActionResult result = target.Edit(product);

            // Assert - check that the repository was not called.
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);

            // Assert - check the method type result
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void CanDeleteValidProducts()
        {
            // Arrange -- create a product
            Product prod = new Product { ProductID = 2, Name = "Test" };

            // Arrange -- create a mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ ProductID = 1, Name="P1" },
                new Product{ ProductID = 3, Name="P3" }
            }.AsQueryable<Product>);

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act - delete the product
            target.Delete(prod.ProductID);

            // Assert - ensure that the repository delete method was called with the correct product.
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }
    }
}
