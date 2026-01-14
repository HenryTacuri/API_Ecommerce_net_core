using APIEcommerce.Constants;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace APIEcommerce.Controllers.V2 {

    //Como se especifica la version de la API, entonces la version de la api ahora va como parte de la URL y ya no como parametro de la URL
    [Route("api/v{version:apiVersion}/[controller]")] //http:localhost:8888/test    [controller] es para que tome el nombre de la clase pero sin controller
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(Roles = "admin")] //Roles son los roles que debe de tener el usuario para los endpoints privados.
                                 //[EnableCors(PolicyNames.AllowSpecificOrigin)] //Nivel de controlador
    public class CategoryController : ControllerBase {

        private readonly ICategoryRepository categoryRepository;

        private readonly IMapper mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper) {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [AllowAnonymous] //Con esto el endpoint ya no necesita autoriazación
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[MapToApiVersion("2.0")] //Esto no seria neceario si todo el controlador responde a una sola version
        //[EnableCors(PolicyNames.AllowSpecificOrigin)] //Nivel de motodo
        public IActionResult getCategoriesOrderByID() {

            IOrderedEnumerable<Category> categories = this.categoryRepository.getCategories().OrderBy(cat => cat.id);
            List<CategoryDto> categoriesDto = new List<CategoryDto>();

            foreach (Category category in categories) {

                //El mapper tiene como destino CategoryDto y como origen category
                categoriesDto.Add(this.mapper.Map<CategoryDto>(category));
            }

            return Ok(categoriesDto);

        }



        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "getCategory")]
        //[ResponseCache(Duration = 10)] //Solo por diez segundos se guarda en caché. Esto es Caché sin perfiles.
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Peticion mala
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Cuando no se encuentre una categoria
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getCategory(int id) {

            System.Console.WriteLine($"Categoria con el ID: {id} a las  {DateTime.Now}"); //Este mensaje solo se mostrara cuando pasen los 10 segundos

            Category category = this.categoryRepository.getCategory(id);

            System.Console.WriteLine($"Respuesta con el ID: {id}");

            if (category == null) {
                return NotFound($"La categoria con el id {id} no existe");
            }

            CategoryDto categoryDto = this.mapper.Map<CategoryDto>(category);

            return Ok(category);
        }



        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Peticion mala
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] //Usuario no autenticado
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult createCategory([FromBody] CreateCategoryDto createCategoryDto) {

            if (createCategory == null) return BadRequest(ModelState); //BadRequest de como se encuentra el modelo

            bool categoryExists = this.categoryRepository.categoryExists(createCategoryDto.name);

            if (categoryExists) {
                ModelState.AddModelError("CustomError", $"La categoria con el nombre {createCategoryDto.name} ya existe");
                return BadRequest(ModelState);
            }

            Category category = this.mapper.Map<Category>(createCategoryDto);

            bool stateCreateCategory = this.categoryRepository.createCategory(category);

            if (!stateCreateCategory) {
                ModelState.AddModelError("CustomError", $"Error al guardar la categoria {category.name}");
                return StatusCode(500, ModelState);
            }

            //Devolmos un 201Created y tambien se devuelve la ubicacion del recurso recien creado utilizando el nombre de la ruta getCategory
            return CreatedAtRoute("getCategory", new { id = category.id }, category);
        }


        [HttpPatch("{id:int}", Name = "updateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Peticion mala
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] //Usuario no autenticado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult updateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto) {


            bool categoryExists = this.categoryRepository.categoryExists(id);

            if (!categoryExists) return NotFound($"La categoria con el id {id} no existe");

            if (updateCategoryDto == null) return BadRequest(ModelState); //BadRequest de como se encuentra el modelo

            categoryExists = this.categoryRepository.categoryExists(updateCategoryDto.name);

            if (categoryExists) {
                ModelState.AddModelError("CustomError", $"La categoria con el nombre {updateCategoryDto.name} ya existe");
                return BadRequest(ModelState);
            }

            Category category = this.mapper.Map<Category>(updateCategoryDto);
            category.id = id;

            bool stateUpdateCategory = this.categoryRepository.updateCategory(category);

            if (!stateUpdateCategory) {
                ModelState.AddModelError("CustomError", $"Error al actualizar la categoria {category.name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



        [HttpDelete("{id:int}", Name = "deleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //Peticion mala
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] //Usuario no autenticado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult deleteCategory(int id) {

            bool categoryExists = this.categoryRepository.categoryExists(id);

            if (!categoryExists) return NotFound($"La categoria con el id {id} no existe");

            Category category = this.categoryRepository.getCategory(id);

            if (category == null) return NotFound($"La categoria con el id {id} no existe");

            bool stateDeleteCategory = this.categoryRepository.deleteCategory(category);

            if (!stateDeleteCategory) {
                ModelState.AddModelError("CustomError", $"Error al eliminar la categoria {category.name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    }

}



// 403 es cuando el usuario no esta autorizado para acceder a este recurso

