namespace Kachuwa.Log
{
    /// <summary>
    /// Represents a way to get a logs"/>
    /// </summary>
    public interface ILogProvider
    {
        ILogger GetLogger(string name);
    }
}