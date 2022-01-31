using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Tests
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void QuandoTarefasEstiveremAtrasadasDeveMudarSeuStatus()
        {
            #region Arrange
            Categoria compCateg = new Categoria(1, "Compras");
            Categoria casaCateg = new Categoria(2, "Casa");
            Categoria trabCateg = new Categoria(3, "Trabalho");
            Categoria saudCateg = new Categoria(4, "Saúde");
            Categoria higiCateg = new Categoria(5, "Higiene");

            List<Tarefa> tarefas = new List<Tarefa>
            {
                //atrasadas a partir de 1/1/2022
                new Tarefa(1, "Tirar lixo", casaCateg, new DateTime(2021,1,1), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer o almoço", casaCateg, new DateTime(2021,2,2), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir à academia", saudCateg, new DateTime(2021,3,15), null, StatusTarefa.Criada),
                new Tarefa(7, "Concluir o relatório", trabCateg, new DateTime(2021,5,7), null, StatusTarefa.Pendente),
                new Tarefa(10, "Beber água", saudCateg, new DateTime(2021,12,31), null, StatusTarefa.Criada),
                
                //dentro do prazo em 1/1/2022
                new Tarefa(8, "Comparecer à reunião", trabCateg, new DateTime(2021,11,12), new DateTime(2021,11,30), StatusTarefa.Concluida),
                new Tarefa(2, "Arrumar a cama", casaCateg, new DateTime(2022,4,5), null, StatusTarefa.Criada),
                new Tarefa(3, "Escovar os dentes", higiCateg, new DateTime(2022,1,2), null, StatusTarefa.Criada),
                new Tarefa(5, "Comprar presente pro João", compCateg, new DateTime(2022,10,8), null, StatusTarefa.Criada),
                new Tarefa(6, "Comprar ração", compCateg, new DateTime(2022,11,20), null, StatusTarefa.Criada)
            };

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext")
                                                                         .Options;

            DbTarefasContext context = new DbTarefasContext(options);
            RepositorioTarefa repositorioTarefa = new RepositorioTarefa(context);

            repositorioTarefa.IncluirTarefas(tarefas.ToArray());

            var command = new GerenciaPrazoDasTarefas(new DateTime(2022, 1, 1)); //Data de corte para verificar se as tarefas estão em atraso ou não
            var handler = new GerenciaPrazoDasTarefasHandler(repositorioTarefa);
            #endregion

            #region Act
            handler.Execute(command);
            #endregion

            #region Assert
            var tarefasEmAtraso = repositorioTarefa.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());
            #endregion
        }

        [Fact]
        public void QuandoOExecuteForChamadoDeveExecutarOMetodoAtualizarTarefasNaQtdeDeVezesDoTotalDeTarefasAtrasadas()
        {
            #region Arrange
            Categoria casaCateg = new Categoria(2, "Casa");
            Categoria saudCateg = new Categoria(4, "Saúde");

            List<Tarefa> tarefas = new List<Tarefa>
            {
                new Tarefa(1, "Tirar lixo", casaCateg, new DateTime(2021,1,1), null, StatusTarefa.Criada),
                new Tarefa(2, "Fazer o almoço", casaCateg, new DateTime(2021,2,2), null, StatusTarefa.Criada),
                new Tarefa(3, "Ir à academia", saudCateg, new DateTime(2021,3,15), null, StatusTarefa.Criada),
            };

            var repositorioMock = new Mock<IRepositorioTarefas>();
            repositorioMock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa,bool>>()))
                           .Returns(tarefas);

            var command = new GerenciaPrazoDasTarefas(new DateTime(2022, 1, 1)); //Data de corte para verificar se as tarefas estão em atraso ou não
            var handler = new GerenciaPrazoDasTarefasHandler(repositorioMock.Object);
            #endregion

            #region Act
            handler.Execute(command);
            #endregion

            #region Assert
            repositorioMock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
            #endregion
        }
    }
}
