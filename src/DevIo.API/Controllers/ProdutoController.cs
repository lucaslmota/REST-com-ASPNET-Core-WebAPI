using AutoMapper;
using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using DevIo.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.Controllers
{
    [Route("api/produtos")]
    public class ProdutoController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        public ProdutoController(
            INotificador notificador,
            IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            IMapper mapper) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoDTO>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoDTO>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterProdutoFornecedor(id));

            if (produto == null) return NotFound();

            return produto;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Adicionar(ProdutoDTO produtoDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgagemNome = Guid.NewGuid() + "_" + produtoDTO.Imagem;
            if(!UploadArquivo(produtoDTO.ImagemUpload, imgagemNome))
            {
                return CustomResponse(produtoDTO);
            }

            produtoDTO.Imagem = imgagemNome;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDTO));

            return CustomResponse(produtoDTO);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> Excluir(Guid id)
        {
            var produto = _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterProdutoFornecedor(id));
            
            if(produto == null) return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produto);  
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {

            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto");
                return false;
            }

            var imgDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imgDataByteArray);

            return true;
        }
    }
}
