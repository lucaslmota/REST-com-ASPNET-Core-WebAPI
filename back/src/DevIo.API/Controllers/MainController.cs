using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using DevIo.Business.Notificacoes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        //validaçao de notificação de erros

        //validação de modelstate

        //validação da operação de negocios

        private readonly INotificador _notificador;
        protected MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if(OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes().Select(n => n.Mensagem)
            });

        }

        protected ActionResult CustomResponse(ModelStateDictionary modelStae) 
        {
            if(!modelStae.IsValid) NotificarErroModelInvalida(modelStae);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelStae)
        {
            var erros = modelStae.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros) 
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }
        protected void NotificarErro (string errorMsg)
        {
            _notificador.Handle(new Notificacao(errorMsg));
        }
    }
}
