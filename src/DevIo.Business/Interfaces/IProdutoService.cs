using DevIo.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIo.Business.Interfaces
{
    public interface IProdutoService : IDisposable
    {
        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);
    }
}
