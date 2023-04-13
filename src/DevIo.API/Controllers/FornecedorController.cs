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

        public FornecedorController(IFornecedorRepository fornecedorRepository, IFornecedorService fornecedorService, IMapper mapper)
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
            if (!ModelState.IsValid) return BadRequest();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);

            await _fornecedorService.Adicionar(fornecedor);

            return Ok(fornecedor);

        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Atualizar(Guid id, FornecedorDTO fornecedorDTO)
        {
            if(id != fornecedorDTO.Id) return BadRequest();

            if(!ModelState.IsValid) return BadRequest();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDTO);

            await _fornecedorService.Atualizar(fornecedor);
            return Ok(fornecedor);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorDTO>> Excluir(Guid id)
        {
            var obterFornecedor = _mapper.Map<FornecedorDTO>(await _fornecedorRepository.ObterFornecedorEndereco(id));

            if (obterFornecedor == null) return NotFound();

            await _fornecedorService.Remover(id);

            return NoContent();
        }



    }
}
