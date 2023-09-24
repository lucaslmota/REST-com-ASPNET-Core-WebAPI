using DevIo.API.Controllers;
using DevIo.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador) : base(notificador)
        {
        }

        [HttpGet]
        public string Valor()
        {
            return "teste v2";
        }
    }
}
