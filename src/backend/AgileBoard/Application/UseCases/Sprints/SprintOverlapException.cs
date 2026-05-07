namespace AgileBoard.Application.UseCases.Sprints;

public class SprintOverlapException : Exception
{
    public SprintOverlapException(string message) : base(message)
    {
    }
}
