namespace Alura.CoisasAFazer.Services.Handlers
{
    public class CommandResult
    {
        public CommandResult(bool isSucess)
        {
            IsSucess = isSucess;
        }

        public bool IsSucess { get; }
    }
}