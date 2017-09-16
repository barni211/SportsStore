using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Pagination_View_Model()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1" },
                new Product {ProductId = 2, Name = "P2" },
                new Product {ProductId = 3, Name = "P3" },
                new Product {ProductId = 4, Name = "P4" },
                new Product {ProductId = 5, Name = "P5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(2, pageInfo.CurrentPage);
            Assert.AreEqual(3, pageInfo.ItemsPerPage);
            Assert.AreEqual(5, pageInfo.TotalItems);
            Assert.AreEqual(2, pageInfo.TotalPages);
        }

        [TestMethod]
        public void Can_Paginate()
        {
            //prepare
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1" },
                new Product {ProductId = 2, Name = "P2" },
                new Product {ProductId = 3, Name = "P3" },
                new Product {ProductId = 4, Name = "P4" },
                new Product {ProductId = 5, Name = "P5" }
            });

            ProductController controller= new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual("P4", prodArray[0].Name);
            Assert.AreEqual("P5", prodArray[1].Name);
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo= new PagingInfo { CurrentPage = 2, TotalItems = 28, ItemsPerPage = 10 };

            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"
            + @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());

        }

        [TestMethod]
        public void Can_Filter_Products_By_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductId = 1, Name = "P1", Category = "cat1"},
                new Product { ProductId = 2, Name = "P2", Category = "cat2"},
                new Product { ProductId = 3, Name = "P3", Category = "cat3"},
                new Product { ProductId = 4, Name = "P4", Category = "cat2"},
                new Product { ProductId = 5, Name = "P5", Category = "cat2"},
                new Product { ProductId = 6, Name = "P6", Category = "cat3"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            Product[] result = ((ProductListViewModel)controller.List("cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[0].Category == "cat2");
            
        }

        [TestMethod]
        public void Can_Create_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductId = 1, Name = "P1", Category = "Śliwki"},
                new Product { ProductId = 2, Name = "P2", Category = "Gruszki"},
                new Product { ProductId = 3, Name = "P3", Category = "Śliwki"},
                new Product { ProductId = 4, Name = "P4", Category = "Śliwki"},
                new Product { ProductId = 5, Name = "P5", Category = "Jabłka"},
                new Product { ProductId = 6, Name = "P6", Category = "Jabłka"}
            });

            NavController target = new NavController(mock.Object);

            string[] results = ((IEnumerable<string>)(target.Menu().Model)).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Gruszki", results[0]);
            Assert.AreEqual("Jabłka", results[1]);
            Assert.AreEqual("Śliwki", results[2]);
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductId = 1, Name = "P1", Category = "Śliwki"},
                new Product { ProductId = 2, Name = "P2", Category = "Gruszki"},
            });

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Gruszki";
            

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);

        }
    }
}
