using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIEcommerce.Repository {

    public class UserRepository : IUserRepository {

        private readonly ApplicationDbContext db;
        private string? secretKey;


        //Propiedades para Identity
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;


        public UserRepository(
            ApplicationDbContext db, 
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper) {

            this.db = db;
            this.secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        //public ICollection<User> getUsers() {
        public ICollection<ApplicationUser> getUsers() {
            //return this.db.Users.OrderBy(u => u.username).ToList();
            return this.db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
        }

        //public User? getUser(int id) {
        public ApplicationUser? getUser(string id) {
            //return this.db.Users.FirstOrDefault(u => u.id == id);
            return this.db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        }

        public bool isUniqueUser(string username) {

            //Any devuelve verdadero si encuentra algun elemento, entonces se tiene que negar.
            return !this.db.Users.Any(u => u.username.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<UserLoginResponseDto> login(UserLoginDto userLoginDto) {
            
            if(string.IsNullOrEmpty(userLoginDto.username)) {
                return new UserLoginResponseDto() {
                    token = "",
                    user = null,
                    message = "El username es requerido"
                };
            }

            //User user = await this.db.Users.FirstOrDefaultAsync<User>(u => u.username.ToLower().Trim() == userLoginDto.username.ToLower().Trim());
            ApplicationUser? user = await this.db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.username.ToLower().Trim());

            if (user == null) {
                return new UserLoginResponseDto() {
                    token = "",
                    user = null,
                    message = "El username no existe"
                };
            }


            if(userLoginDto.password == null) {
                return new UserLoginResponseDto() {
                    token = "",
                    user = null,
                    message = "Password requerido"
                };
            }

            bool isValidPassword = await this.userManager.CheckPasswordAsync(user, userLoginDto.password);

            //if(!BCrypt.Net.BCrypt.Verify(userLoginDto.password, user.password)) {
            if(!isValidPassword) {
                return new UserLoginResponseDto() {
                    token = "",
                    user = null,
                    message = "Credenciales incorrectas"
                };
            }


            //GENERACION DEL JWT
            JwtSecurityTokenHandler handlerToken = new JwtSecurityTokenHandler();

            if(string.IsNullOrWhiteSpace(this.secretKey)) {
                throw new InvalidOperationException("La secretkey no esta configurada");
            }


            IList<string> roles = await this.userManager.GetRolesAsync(user); //Solo cuando se trabaja con identity
            var key = Encoding.UTF8.GetBytes(this.secretKey);//codificacion de la secret key


            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    //new Claim("id", user.id.ToString()),
                    new Claim("id", user.Id.ToString()),
                    //new Claim("username", user.username),
                    new Claim("username", user.UserName ?? string.Empty),
                    //new Claim(ClaimTypes.Role, user.role ?? "")
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "")
                }),

                Expires = DateTime.UtcNow.AddHours(2), // Duracion de dos horas

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = handlerToken.CreateToken(tokenDescriptor);

            return new UserLoginResponseDto() {
                token = handlerToken.WriteToken(token),
                /*user = new UserRegisterDto() {
                    username = user.username,
                    name = user.name,
                    role = user.role,
                    password = user.password ?? ""
                },*/
                user = this.mapper.Map<UserDataDto>(user),
                message = "Usuario logueado correctamente"
            };

        }


        public async Task<UserDataDto> register(CreateUserDto createUserDto) {

            /*string encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.password);

            User user = new User() {
                username = createUserDto.username ?? "No username",
                name = createUserDto.name,
                role = createUserDto.role,
                password = encriptedPassword
            };

            this.db.Users.Add(user);

            await this.db.SaveChangesAsync();

            return user;*/


            if (string.IsNullOrEmpty(createUserDto.username)) throw new ArgumentNullException("El username es requerido");


            if (createUserDto.password == null) throw new ArgumentNullException("El password es requerido");

            ApplicationUser user = new ApplicationUser() {
                UserName = createUserDto.username,
                Email = createUserDto.username,
                NormalizedEmail = createUserDto.username.ToUpper(),
                name = createUserDto.username,
            };

            IdentityResult resultCreateUser = await this.userManager.CreateAsync(user, createUserDto.password);

            if(resultCreateUser.Succeeded) {
                string userRole = createUserDto.role ?? "User";
                bool rolExists = await this.roleManager.RoleExistsAsync(userRole);

                //si no existe entonces lo crea y si existe lo vuelve a utilizar
                if (!rolExists) {
                    IdentityRole identityRole = new IdentityRole(userRole);
                    await this.roleManager.CreateAsync(identityRole);
                }

                await this.userManager.AddToRoleAsync(user, userRole); //Asignamos el rol al usuario

                ApplicationUser? createdUser = this.db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.username);

                return this.mapper.Map<UserDataDto>(createdUser);
            }

            string errors = string.Join(", ", resultCreateUser.Errors.Select(e => e.Description));

            throw new ApplicationException($"No se pudo realizar el registro: {errors}");

        }
    }


}
