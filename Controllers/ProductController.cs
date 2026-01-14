using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Models.Dtos.Responses;
using APIEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIEcommerce.Controllers {

    [Route("api/v{version:apiVersion}/[controller]")]
    //Con esto lo que hacemos es que cuando en el swagger se cambie la version entonces en la URL del endpoint también cambia la version
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    //Con esto ya es necesario poner ApiVersion, ahora en el swagger cuando se cambie de version se tiene que especificar
    //en el endpoint que version de la api se va ha utilizar
    [ApiVersionNeutral]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ProductController : ControllerBase {

        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper) {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getProducts() {
            ICollection<Product> products = this.productRepository.getProducts();

            //Hacemos uso del automapper para evitar el bucle.
            List<ProductDto> productsDto = this.mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }


        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "getProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getProduct(int productId) {
            Product product = this.productRepository.getProduct(productId);

            if (product == null) return NotFound($"El producto con el id {productId} no existe");

            ProductDto productDto = this.mapper.Map<ProductDto>(product);

            return Ok(productDto);
        }


        [AllowAnonymous]
        [HttpGet("paged", Name = "getProductsInPage")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getProductsInPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5) {

            if (pageNumber < 1 || pageSize < 1) return BadRequest("Los parametros de paginacion no son validos");

            int totalProducts = this.productRepository.getTotalProducts();

            //Con Math.Ceiling se redondea a un numero entero
            int totalPages = (int) Math.Ceiling((double) totalProducts / pageSize);

            if (pageNumber > totalPages) return NotFound("No hay mas paginas disponibles");

            ICollection<Product> products = this.productRepository.getProductsInPage(pageNumber, pageSize);

            List<ProductDto> productsDto = this.mapper.Map<List<ProductDto>>(products);

            PaginationResponse<ProductDto> paginationResponse = new PaginationResponse<ProductDto> {
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPages = totalPages,
                items = productsDto
            };

            return Ok(paginationResponse);
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult createProduct([FromForm] CreateProductDto createProductDto) {

            if (createProductDto == null) return BadRequest(ModelState);

            bool productExists = this.productRepository.productExists(createProductDto.name);

            if(productExists) {
                ModelState.AddModelError("CustomError", "El producto ya existe");
                return BadRequest(ModelState);
            }


            bool categoryExists = this.categoryRepository.categoryExists(createProductDto.category_id);

            if (!categoryExists) {
                ModelState.AddModelError("CustomError", $"La categoria con el id {createProductDto.category_id} no existe");
                return BadRequest(ModelState);
            }


            Product product = this.mapper.Map<Product>(createProductDto);

            //Agregando imagen
            if(createProductDto.image != null) {
                uploadProductImage(createProductDto, product);
            } else {
                product.imgUrl = "https://placehold.co/300x300";
            }

            bool stateCreateProduct = this.productRepository.createProduct(product);

            if(!stateCreateProduct) {
                ModelState.AddModelError("CustomError", $"Error al registrar el producto {product.name}");
                return StatusCode(500, ModelState);
            }


            //Se devuelve el producto con el nombre de la categoria
            Product createdProduct = this.productRepository.getProduct(product.id);
            ProductDto productDto = this.mapper.Map<ProductDto>(createdProduct);

            return CreatedAtRoute("getProduct", new { productId = product.id}, productDto);
        }



        [HttpGet("searchByCategory/{categoryId:int}", Name = "getProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getProductsByCategory(int categoryId) {

            ICollection<Product> products = this.productRepository.getProductsByCategory(categoryId);

            if (products.Count == 0) return NotFound($"Los productos con la categoria {categoryId} no existen");

            List<ProductDto> productsDto = this.mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }



        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "buyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult buyProduct(string name, int quantity) {

            if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return BadRequest("El nombre del producto o la cantidad no son validos");

            bool productExists = this.productRepository.productExists(name);

            if (!productExists) return NotFound($"El producto con el nombre {name} no existe");
            
            bool stateBuyProduct = this.productRepository.buyProduct(name, quantity);

            if(!stateBuyProduct) {
                ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {name} o la cantidad solicitada es mayor al stock disponible");
                return BadRequest(ModelState);
            }

            string units = quantity == 1 ? "unidad" : "unidades";

            return Ok($"Se compro {quantity} {units} del producto {name}");

        }



        [HttpPatch("{productId:int}", Name = "updateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult updateProduct(int productId, [FromForm] UpdateProductDto updateProductDto) {

            if (updateProductDto == null) return BadRequest(ModelState);

            bool productExists = this.productRepository.productExists(productId);

            if (!productExists) {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }

            bool categoryExists = this.categoryRepository.categoryExists(updateProductDto.category_id);

            if (!categoryExists) {
                ModelState.AddModelError("CustomError", $"La categoria con el id {updateProductDto.category_id} no existe");
                return BadRequest(ModelState);
            }

            Product product = this.mapper.Map<Product>(updateProductDto);
            product.id = productId; //esto se hace para que no lo cree si no lo actualice


            //Agregando imagen
            if (updateProductDto.image != null) {
                uploadProductImage(updateProductDto, product);
            } else {
                product.imgUrl = "https://placehold.co/300x300";
            }


            bool stateUpdateProduct = this.productRepository.updateProduct(product);

            if (!stateUpdateProduct) {
                ModelState.AddModelError("CustomError", $"Error al actualizar el producto {product.name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



        [HttpDelete("{productId:int}", Name = "deleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult deleteProduct(int productId) {

            if (productId == 0) return BadRequest(ModelState);

            Product product = this.productRepository.getProduct(productId);

            if (product == null) return NotFound($"El producto con el id {productId} no existe");

            ProductDto productDto = this.mapper.Map<ProductDto>(product);

            bool stateDeleteProduct = this.productRepository.deleteProduct(product);

            if (!stateDeleteProduct) {
                ModelState.AddModelError("CustomError", $"Error al eliminar el producto {product.name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



        private void uploadProductImage(dynamic productDto, Product product) {
            string fileName = product.id + Guid.NewGuid().ToString() + Path.GetExtension(productDto.image.FileName);

            //La ruta wwwroot es la encargada de tener todos los archivos estaticos y products_images es para las
            //imagenes de los productos
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products_images");

            //Si no esta creado el directorio lo crea
            if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

            //Verificamos si existe la imagen
            string filePath = Path.Combine(imagesFolder, fileName);
            FileInfo file = new FileInfo(filePath);

            if (file.Exists) file.Delete();

            //Guardamos la imagen, el using es necesario ya que lo que se hace es crear el archivo
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            productDto.image.CopyTo(fileStream);

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.imgUrl = $"{baseUrl}/products_images/{fileName}";
            product.imgUrlLocal = filePath;
        }

    }

}

