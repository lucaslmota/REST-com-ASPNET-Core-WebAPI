using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.Controllers
{
    [Route("api/logar")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthController(INotificador notificador, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserDTO registerUserDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUserDTO.Email,
                Email = registerUserDTO.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUserDTO.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return CustomResponse(registerUserDTO);
            }
            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }


            return CustomResponse(registerUserDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUserDTO.Email, loginUserDTO.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return CustomResponse(loginUserDTO);
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temprariamente bloqueado por tentativas inválidos");
                return CustomResponse(loginUserDTO);
            }

            NotificarErro("Usuário ou senha incorretos");
            return CustomResponse(loginUserDTO);

        }
    }
}
