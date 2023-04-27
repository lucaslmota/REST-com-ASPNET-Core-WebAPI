using DevIo.API.DTO;
using DevIo.API.Extensions;
using DevIo.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DevIo.API.Controllers
{
    [Route("api/logar")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        public AuthController(
            INotificador notificador, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IOptions<AppSettings> appSettings) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
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
                return CustomResponse(GerarJwt());
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
                return CustomResponse(GerarJwt());
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temprariamente bloqueado por tentativas inválidos");
                return CustomResponse(loginUserDTO);
            }

            NotificarErro("Usuário ou senha incorretos");
            return CustomResponse(loginUserDTO);

        }

        private string GerarJwt()
        {
            var tokenHandle = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var token = tokenHandle.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandle.WriteToken(token);
            return encodedToken;
        }
    }
}
