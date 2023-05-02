using System.ComponentModel.DataAnnotations;

namespace DevIo.API.DTO
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage ="O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage ="o campo {0} está em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage ="O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage ="As senhas não conferem.")]
        public string ConfirmarPassword { get; set; }

    }

    public class LoginUserDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "o campo {0} está em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class UserTokenDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimDTO> Claims { get; set; }
    }

    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenDTO UserTokenDTO { get; set; }
    }

    public class ClaimDTO
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
