using AutoMapper;
using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using DevIo.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.Controllers
{
    [Route("api/fornecedores")]
    public class FornecedorController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedorController(
            IFornecedorRepository fornecedorRepository, 
            IFornecedorService fornecedorService, 
            IMapper mapper, 
            INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorDTO>> ObterTodos()
        {
            var fornecedor = _mapper.Map<IEnumerable<FornecedorDTO>>(await _fornecedorRepository.ObterTodos());

            return fornecedor;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> ObterPorId(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor == null) return NotFound();

            return Ok(fornecedor);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorDTO>> Adcionar(FornecedorDTO fornecedorDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorDTO));

            return CustomResponse(fornecedorDTO);

        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedorDTO)
        {
            if (id != fornecedorDTO.Id)
            {
                NotificarErro("O id informado diverge do que foi passado");
                return CustomResponse(fornecedorDTO);
            }

            if(!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorDTO));
            return CustomResponse(fornecedorDTO);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Excluir(Guid id)
        {
            var obterFornecedor = _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorEndereco(id));

            if (obterFornecedor == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse(obterFornecedor);
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<EnderecoDTO> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoDTO>(await _fornecedorRepository.ObterPorId(id));
        }

        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoDTO enderecoDTO)
        {
            if(id != enderecoDTO.Id) return BadRequest();

            if(!ModelState.IsValid) return CustomResponse(ModelState);

            var endereco = _mapper.Map<Endereco>(enderecoDTO);
            await _fornecedorService.AtualizarEndereco(endereco);
            return CustomResponse(enderecoDTO);
        }


    }
}
