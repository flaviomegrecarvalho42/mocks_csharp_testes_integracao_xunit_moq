using Xunit;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Core.Models;

namespace Alura.CoisasAFazer.Tests
{
    public class TarefasControllerEndPointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesVaildasDeverRestornar200OK()
        {
            #region Arrange
            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext")
                                                                         .Options;
            var contexto = new DbTarefasContext(options);
            contexto.Categorias.Add(new Categoria(20, "Estudo"));
            contexto.SaveChanges();

            var repositorioTarefa = new RepositorioTarefa(contexto);
            var controller = new TarefasController(repositorioTarefa, loggerMock.Object);

            var viewModel = new CadastraTarefaViewModel
            {
                IdCategoria = 20,
                Titulo = "Estudar XUnit",
                Prazo = new DateTime(2021, 12, 31)
            };
            #endregion

            #region Act
            var result = controller.EndpointCadastraTarefa(viewModel);
            #endregion

            #region Assert
            Assert.IsType<OkResult>(result);
            #endregion
        }

        [Fact]
        public void DadaTarefaComInformacoesInVaildasDeverRestornar500InternalServerError()
        {
            #region Arrange
            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();
            
            var repositorioTarefaMock = new Mock<IRepositorioTarefas>();
            repositorioTarefaMock.Setup(r => r.ObtemCategoriaPorId(20))
                                 .Returns(new Categoria(20, "Estudar XUnit"));

            repositorioTarefaMock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                                 .Throws(new Exception("Houve um erro ao incluir a tarefa"));

            var controller = new TarefasController(repositorioTarefaMock.Object, loggerMock.Object);

            var viewModel = new CadastraTarefaViewModel
            {
                IdCategoria = 20,
                Titulo = "Estudar XUnit",
                Prazo = new DateTime(2021, 12, 31)
            };
            #endregion

            #region Act
            var result = controller.EndpointCadastraTarefa(viewModel);
            #endregion

            #region Assert
            var statusCodeRetornado = (result as StatusCodeResult).StatusCode;
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeRetornado);
            #endregion
        }
    }
}
