using AutoMapper;
using DevIo.API.Controllers;
using DevIo.API.DTO;
using DevIo.Business.Interfaces;
using DevIo.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIo.API.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
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
            if (!UploadArquivo(produtoDTO.ImagemUpload, imgagemNome))
            {
                return CustomResponse(produtoDTO);
            }

            produtoDTO.Imagem = imgagemNome;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDTO));

            return CustomResponse(produtoDTO);
        }

        [HttpPost("adcionar")]
        public async Task<ActionResult<ProdutoDTO>> AdicionarImagemGrande(ProdutoImagemDTO produtoDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgagemPrefixo = Guid.NewGuid() + "_";
            if (!await UploadArquivoGrande(produtoDTO.ImagemUpload, imgagemPrefixo))
            {
                return CustomResponse(ModelState);
            }

            produtoDTO.Imagem = imgagemPrefixo + produtoDTO.ImagemUpload?.FileName;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDTO));

            return CustomResponse(produtoDTO);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.Id)
            {
                NotificarErro("Os Ids divergem.");
                return CustomResponse();
            }

            var produtoAtualizacao = _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterProdutoFornecedor(id));
            produtoDTO.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoDTO.ImagemUpload != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + produtoDTO.Imagem;
                if (!UploadArquivo(produtoDTO.ImagemUpload, imagemNome))
                {
                    return CustomResponse(ModelState);
                }
                produtoAtualizacao.Imagem = imagemNome;
            }

            produtoAtualizacao.Nome = produtoDTO.Nome;
            produtoAtualizacao.Descricao = produtoDTO.Descricao;
            produtoAtualizacao.Valor = produtoDTO.Valor;
            produtoAtualizacao.Ativo = produtoDTO.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return CustomResponse(produtoDTO);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoDTO>> Excluir(Guid id)
        {
            var produto = _mapper.Map<ProdutoDTO>(await _produtoRepository.ObterProdutoFornecedor(id));

            if (produto == null) return NotFound();

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

        private async Task<bool> UploadArquivoGrande(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto.");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com esse nome.");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            return true;
        }

        //{ exemplo de json para criar produto
        //  "fornecedorId": "35945f78-91f5-4d69-e354-08db406e7441",
        //  "nome": "teste de produto",
        //  "descricao": "sdsdfsd",
        //  "imagemUpload": "tem que converter uma imagem para base64",
        //  "imagem": "nome da imagem",
        //  "valor": 12,
        //  "dataCadastro": "2023-11-19T23:50:32.758Z",
        //  "ativo": true
        //}
}
}
