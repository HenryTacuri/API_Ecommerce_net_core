using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIEcommerce.Controllers {

    [Route("api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    [ApiVersionNeutral]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase {

        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository, IMapper mapper) {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult getUsers() {

            //ICollection<User> users = this.userRepository.getUsers();
            ICollection<ApplicationUser> users = this.userRepository.getUsers();
            List<UserDto> usersDtos = this.mapper.Map<List<UserDto>>(users);

            return Ok(usersDtos);
        }


        //[HttpGet("{id:int}", Name = "getUser")]
        [HttpGet("{id}", Name = "getUser")]//para identity
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public IActionResult getUser(string id) {
        public IActionResult getUser(string id) { //para identity

            //User user = this.userRepository.getUser(id);

            ApplicationUser? user = this.userRepository.getUser(id);

            if (user == null) return NotFound($"El usuario con el id {id} no existe");

            UserDto userDto = this.mapper.Map<UserDto>(user);

            return Ok(userDto);

        }


        [AllowAnonymous]
        [HttpPost(Name = "registerUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> registerUser([FromBody] CreateUserDto createUserDto) {

            if (createUserDto == null || !ModelState.IsValid) return BadRequest(ModelState);
            
            if(string.IsNullOrWhiteSpace(createUserDto.username)) return BadRequest("El username es requerido");

            bool isUniqueUser = this.userRepository.isUniqueUser(createUserDto.username);

            if(!isUniqueUser) return BadRequest("El usuario ya existe");

            //User user = await this.userRepository.register(createUserDto);
            UserDataDto user = await this.userRepository.register(createUserDto);

            if (user == null) return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");

            return CreatedAtRoute("getUser", new {id = user.id}, user);
        }



        [AllowAnonymous]
        [HttpPost("login", Name = "loginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> loginUser([FromBody] UserLoginDto userLoginDto) {

            if (userLoginDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            UserLoginResponseDto user = await this.userRepository.login(userLoginDto);

            if (user == null) return Unauthorized();

            return Ok(user);
        }

    }

}

