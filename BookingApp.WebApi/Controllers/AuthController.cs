using BookingApp.Business.Operations.User;
using BookingApp.Business.Operations.User.Dtos;
using BookingApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;


        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } // TODO: Ileride acytion filter olarak kodlanacak.

            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                BirthDate = request.BirthDate,
            };

            var result = await _userService.AddUser(addUserDto);

            if (result.IsSucceed)
                return Ok();
            else
                return BadRequest(result.Message);

        }

        // HTTPGET ->  Veti url uzerinden tasinir - querystring
        // - Firewall ve benzeri uygulamalarinizin url'i loglar, boyle bir durumda  da sifrenizi loglar.
        // GUVENLIL ACIGI

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var result = _userService.LoginUser(new LoginUserDto { Email = request.Email, Password = request.Password });

            if (!result.IsSucceed)
                return BadRequest(result.Message);
        }
    }
}
