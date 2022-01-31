using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Moq;
using Xunit;

namespace Alura.CoisasAFazer.Tests
{
    public class ObtemCategoriaPorIdHandlerExecute
    {
        [Fact]
        public void QuandoIdDaCategoriaForExistenteDeveChamarOMetodoObtemCategoriaPorIdUmaUnicaVez()
        {
            #region Arrange
            var idCategoria = 20;
            
            var repositorioMock = new Mock<IRepositorioTarefas>();
            var command = new ObtemCategoriaPorId(idCategoria); //Data de corte para verificar se as tarefas estão em atraso ou não
            var handler = new ObtemCategoriaPorIdHandler(repositorioMock.Object);
            #endregion

            #region Act
            handler.Execute(command);
            #endregion

            #region Assert
            repositorioMock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());
            #endregion
        }
    }
}
