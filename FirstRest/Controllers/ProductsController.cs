using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using LiteDB;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;

namespace FirstRest.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProductsController : ApiController
    {

        public ProductsController()
        {
            LiteDB = new LiteDbRepo();
        }
        public static List<Product> InMemProducts = new List<Product>
        {
            new Ring()
            {
                sku = "1019",
                description = "VINTAGE SILVER BLACK TIBETAN SKELETON SKULL RING",
                quantity = 0,
                cost = 0.93,
                price = 10.00,
                image = "http://dkw99robnrest/RobNRest/pictures/skull-ring.png"
            },
            new Ring()
            {
                sku = "1002",
                description = "ROSE GOLD FINISH MENS WEDDING BAND TITANIUM STEAL  6MM",
                quantity = 2,
                cost = 0.64,
                price = 10.00,
                image = "http://dkw99robnrest/RobNRest/pictures/rose-gold-ring.png"
            },
            new Ring()
            {
                sku = "1013",
                description = "ANTIQUE SILVER LEAF RING  (MIDDLE FINGER)",
                quantity = 1,
                cost = 0.21,
                price = 10.00,
                image = "http://dkw99robnrest/RobNRest/pictures/antique-silver-leaf.png"
            },
        };

        public LiteDbRepo LiteDB { get; private set; }

        [SwaggerResponse(HttpStatusCode.OK, type: typeof(IEnumerable<Product>))]
        [HttpGet]
        [Route("api/products/")]
        public IHttpActionResult GetAllProducts()
        {
            return Ok(LiteDB.GetAllProducts());
        }

        [SwaggerResponse(HttpStatusCode.OK, type: typeof(Product))]
        [HttpGet]
        [Route("api/products/{sku}/")]
        public IHttpActionResult GetAProduct(string sku)
        {
            var foundProduct = LiteDB.GetAProduct(sku);
            if (foundProduct != null)
            {
                return Ok(foundProduct);
            }
            else
            {
                return NotFound();
            }
        }

        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.OK, type: typeof(Product))]
        [HttpPost]
        [Route("api/products/")]
        public IHttpActionResult AddProduct([FromBody] Product newProduct)
        {
            if (LiteDB.GetAProduct(newProduct.sku) != null)
            {
                return BadRequest("Product with sku already exists.");
            }

            LiteDB.AddAProduct(newProduct);
            return Ok(newProduct);
        }


        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK, type: typeof(Product))]
        [HttpPost]
        [Route("api/products/{sku}")]
        public IHttpActionResult UpdateProduct(string sku, [FromBody] UpdateProduct updatedProduct)
        {
            var foundProduct = LiteDB.GetAProduct(sku);

            if (foundProduct == null)
            {
                return NotFound();
            }

            var returnProduct = LiteDB.UpdateProduct(sku, updatedProduct);
            return Ok(returnProduct);
        }

        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [HttpDelete]
        [Route("api/products/{sku}")]
        public IHttpActionResult DeleteProduct(string sku)
        {
            var foundProduct = LiteDB.GetAProduct(sku);
            if (foundProduct != null)
            {
                LiteDB.DeleteProduct(sku);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }

    public class UpdateProduct
    {
        public string description { get; set; }
        public double? cost { get; set; }
        public double? price { get; set; }
        public long? quantity { get; set; }
        public string image { get; set; }
    }

    public class Product
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
        [Required] public string sku { get; set; }
        public string description { get; set; }

        [Required] public double cost { get; set; }

        [Required] public double price { get; set; }

        [Required] public long quantity { get; set; }
        public string image { get; set; }
    }

    public class Ring : Product
    {
    }

    public class LiteDbRepo
    {
        public IEnumerable<Product> GetAllProducts()
        {
            using (var db = new LiteDatabase(@"C:\data\RobNRest.db"))
            {
                var col = db.GetCollection<Product>("products");
                var products =  col.FindAll();
                return products;//products;
            }
        }

        public Product GetAProduct(string sku)
        {
            using (var db = new LiteDatabase(@"C:\data\RobNRest.db"))
            {
                var col = db.GetCollection<Product>("products");
                var products = col.FindOne(p => p.sku == sku);
                return products;
            }
        }

        public Product AddAProduct(Product product)
        {
            using (var db = new LiteDatabase(@"C:\data\RobNRest.db"))
            {
                var col = db.GetCollection<Product>("products");
                col.Insert(product);

                var newProduct = col.FindOne(p => p.sku == product.sku);
                return newProduct;
            }
        }

        public Product UpdateProduct(string sku, UpdateProduct updatedProduct)
        {

            using (var db = new LiteDatabase(@"C:\data\RobNRest.db"))
            {
                var col = db.GetCollection<Product>("products");
                

                var foundProduct = col.FindOne(p => p.sku == sku);


                if (!string.IsNullOrEmpty(updatedProduct.image))
                    foundProduct.image = updatedProduct.image;
                if (updatedProduct.cost.HasValue)
                    foundProduct.cost = updatedProduct.cost.Value;
                if (!string.IsNullOrEmpty(updatedProduct.description))
                    foundProduct.description = updatedProduct.description;
                if (updatedProduct.price.HasValue)
                    foundProduct.price = updatedProduct.price.Value;
                if (updatedProduct.quantity.HasValue)
                    foundProduct.quantity = updatedProduct.quantity.Value;

                col.Update(foundProduct);
                return foundProduct;
            }
        }

        public void DeleteProduct(string sku)
        {

            using (var db = new LiteDatabase(@"C:\data\RobNRest.db"))
            {
                var col = db.GetCollection<Product>("products");
                col.Delete(p => p.sku == sku);
            }
        }
    }
}