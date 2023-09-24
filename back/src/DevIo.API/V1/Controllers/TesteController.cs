using DevIo.API.Controllers;
using DevIo.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DevIo.API.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador) : base(notificador)
        {
        }

        [HttpGet]
        public string Valor()
        {
            return "teste v1";
        }
    }
}
