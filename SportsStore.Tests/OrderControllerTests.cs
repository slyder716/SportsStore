using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void CannotCheckoutEmptyCart()
        {
            // Arrange -- create mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            
            // Arrange - create an empty cart
            Cart cart = new Cart();

            //Arrage - Create Order
            Order order = new Order();

            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            // Act
            ViewResult result = target.Checkout(order) as ViewResult;

            // Assert - check that the order hasn't been stored.
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            // Assert - check that the method is returning the default view.
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            // Assert - check that I am passing an invalid model to the view. 
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void CannotCheckoutInvalidShippingDetails()
        {
            // Arrange -- create mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            // Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);
            target.ModelState.AddModelError("error", "error");

            // Act - Try to checkout
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            // Assert - check that the order hasn't been passed stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            // Assert  - check that the method is returning the default view.
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void CanCheckoutAndSubmitOrder()
        {
            // Arrange -- create mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            // Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            RedirectToActionResult result = target.Checkout(new Order()) as RedirectToActionResult;

            // Assert - check that the order has been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            // Assert - check that the medhod is redirecting to the Completed action.
            Assert.Equal("Completed", result.ActionName);
        }
    }
}
