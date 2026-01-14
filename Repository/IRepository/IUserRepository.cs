using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;

namespace APIEcommerce.Repository.IRepository {

    public interface IUserRepository {

        //ICollection<User> getUsers();

        ICollection<ApplicationUser> getUsers(); //para identity

        //User? getUser(int id);

        ApplicationUser? getUser(string id); //para identity

        bool isUniqueUser(string username);

        Task<UserLoginResponseDto> login(UserLoginDto userLoginDto);

        //Task<User> register(CreateUserDto createUserDto);

        Task<UserDataDto> register(CreateUserDto createUserDto); //para identity
    }

}

