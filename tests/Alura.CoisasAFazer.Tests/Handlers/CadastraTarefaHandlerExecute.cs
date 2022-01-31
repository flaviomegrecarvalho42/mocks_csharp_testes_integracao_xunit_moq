using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Tests
{
    public class CadastraTarefaHandlerExecute
    {
        delegate void CaptureMessageLog(LogLevel logLevel, 
                                        EventId eventId, 
                                        object state, 
                                        Exception exception,
                                        Func<object, Exception, string> func);

        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBandoDeDados()
        {
            #region Arrange
            CadastraTarefa command = new CadastraTarefa("Estudar XUnit", 
                                                        new Categoria("Estudo"),
                                                        new DateTime(2021, 12, 31));
            
            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();
            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext")
                                                                         .Options;

            DbTarefasContext context = new DbTarefasContext(options);
            RepositorioTarefa repositorio = new RepositorioTarefa(context);
            CadastraTarefaHandler handler = new CadastraTarefaHandler(repositorio, loggerMock.Object);
            #endregion

            #region Act
            handler.Execute(command);
            #endregion

            #region Assert
            var tarefa = repositorio.ObtemTarefas(t => t.Titulo == "Estudar XUnit").FirstOrDefault();
            Assert.NotNull(tarefa);
            #endregion
        }


        [Fact]
        public void DadaTarefaComInformacoesValidasDeveGerarLogger()
        {
            #region Arrange
            var tituloTarefa = "Usar Moq para aprofundar conhecimento da API";
            CadastraTarefa command = new CadastraTarefa(tituloTarefa,
                                                        new Categoria("Estudo"),
                                                        new DateTime(2021, 12, 31));


            LogLevel logLevelCaptured = LogLevel.Error;
            string messageCaptured = string.Empty;
            CaptureMessageLog captureMessageLog = (LogLevel, EventId, state, exception, func) =>
            {
                logLevelCaptured = LogLevel;
                messageCaptured = func(state, exception);
            };

            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();
            loggerMock.Setup(l => l.Log(It.IsAny<LogLevel>(),
                                        It.IsAny<EventId>(),
                                        It.IsAny<object>(),
                                        It.IsAny<Exception>(),
                                        It.IsAny<Func<object, Exception, string>>()))
                      .Callback(captureMessageLog);

            var repositorioMock = new Mock<IRepositorioTarefas>();

            CadastraTarefaHandler handler = new CadastraTarefaHandler(repositorioMock.Object, loggerMock.Object);
            #endregion

            #region Act
            handler.Execute(command);
            #endregion

            #region Assert
            Assert.Equal(LogLevel.Debug, logLevelCaptured);
            Assert.Contains(tituloTarefa, messageCaptured);
            #endregion
        }

        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSuccessDeveSerFalse()
        {
            #region Arrange
            CadastraTarefa command = new CadastraTarefa("Estudar XUnit",
                                                        new Categoria("Estudo"),
                                                        new DateTime(2021, 12, 31));

            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();
            
            var repositorioMock = new Mock<IRepositorioTarefas>();
            repositorioMock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                           .Throws(new Exception("Houve um erro na inclusão das tarefas"));

            CadastraTarefaHandler handler = new CadastraTarefaHandler(repositorioMock.Object, loggerMock.Object);
            #endregion

            #region Act
            CommandResult result = handler.Execute(command);
            #endregion

            #region Assert
            Assert.False(result.IsSucess);
            Assert.Equal(typeof(CommandResult), result.GetType());
            #endregion
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveCriarUmLogComAMenssagemDaExcecao()
        {
            #region Arrange
            var messageErrorEsperada = "Houve um erro na inclusão das tarefas";
            Exception exceptionEsperada = new Exception(messageErrorEsperada);

            CadastraTarefa command = new CadastraTarefa("Estudar XUnit",
                                                        new Categoria("Estudo"),
                                                        new DateTime(2021, 12, 31));

            var loggerMock = new Mock<ILogger<CadastraTarefaHandler>>();

            var repositorioMock = new Mock<IRepositorioTarefas>();
            repositorioMock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                           .Throws(exceptionEsperada);

            CadastraTarefaHandler handler = new CadastraTarefaHandler(repositorioMock.Object, loggerMock.Object);
            #endregion

            #region Act
            CommandResult result = handler.Execute(command);
            #endregion

            #region Assert
            loggerMock.Verify(l => l.Log(LogLevel.Error, 
                                         It.IsAny<EventId>(), 
                                         It.IsAny<object>(),
                                         exceptionEsperada, 
                                         It.IsAny<Func<object, Exception, string>>()), Times.Once());
            #endregion
        }
    }
}
