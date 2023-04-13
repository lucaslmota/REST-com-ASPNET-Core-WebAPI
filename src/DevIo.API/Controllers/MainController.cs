using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        //validaçao de notificação de erros

        //validação de modelstate

        //validação da operação de negocios
    }

    //[Route("api/fornecedor")]
    //public class FornecedorController : MainController
    //{
    //    private readonly IFornecedorRepository _fornecedorRepository;

    //    public FornecedorController(IFornecedorRepository fornecedorRepository)
    //    {
    //        _fornecedorRepository = fornecedorRepository;
    //    }
    //    public async Task<ActionResult<IEnumerable<FornecedorDTO>>> ObterTodos()
    //    {
    //        var fornecedor = await _fornecedorRepository.ObterTodos();

    //        return Ok(fornecedor);
    //    }
    //}
}
