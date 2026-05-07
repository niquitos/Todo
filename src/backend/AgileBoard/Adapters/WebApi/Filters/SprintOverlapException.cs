namespace AgileBoard.Adapters.WebApi.Filters;

public class SprintOverlapException : Exception
{
    public SprintOverlapException(string message) : base(message)
    {
    }
}
