namespace AgileBoard.Application.UseCases.Sprints;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
