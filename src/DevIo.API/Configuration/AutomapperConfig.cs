using AutoMapper;
using DevIo.API.DTO;
using DevIo.Business.Models;

namespace DevIo.API.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig() 
        {
            CreateMap<Fornecedor, FornecedorDTO>().ReverseMap();
            CreateMap<Endereco, EnderecoDTO>().ReverseMap();
            CreateMap<Produto, ProdutoDTO>().ReverseMap();
        }
    }
}
