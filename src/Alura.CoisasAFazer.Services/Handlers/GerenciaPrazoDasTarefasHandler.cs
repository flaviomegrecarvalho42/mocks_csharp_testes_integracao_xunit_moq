using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using System.Linq;

namespace Alura.CoisasAFazer.Services.Handlers
{
    public class GerenciaPrazoDasTarefasHandler
    {
        IRepositorioTarefas _repositorio;

        public GerenciaPrazoDasTarefasHandler(IRepositorioTarefas repositorio)
        {
            _repositorio = repositorio;
        }

        public void Execute(GerenciaPrazoDasTarefas comando)
        {
            var agora = comando.DataHoraAtual;

            //pegar todas as tarefas não concluídas que passaram do prazo
            var tarefas = _repositorio.ObtemTarefas(t => t.Prazo <= agora && t.Status != StatusTarefa.Concluida)
                                      .ToList();

            //atualizá-las com status Atrasada
            tarefas.ForEach(t => t.Status = StatusTarefa.EmAtraso);

            //salvar tarefas
            _repositorio.AtualizarTarefas(tarefas.ToArray());
        }
    }
}
