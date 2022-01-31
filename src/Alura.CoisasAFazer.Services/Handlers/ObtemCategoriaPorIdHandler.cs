using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;

namespace Alura.CoisasAFazer.Services.Handlers
{
    public class ObtemCategoriaPorIdHandler
    {
        IRepositorioTarefas _repositorio;

        public ObtemCategoriaPorIdHandler(IRepositorioTarefas repositorio)
        {
            _repositorio = repositorio;
        }
        
        public Categoria Execute(ObtemCategoriaPorId comando)
        {
            return _repositorio.ObtemCategoriaPorId(comando.IdCategoria);
        }
    }
}
