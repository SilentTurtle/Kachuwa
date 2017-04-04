namespace Kachuwa.Logger
{
    /// <summary>
    /// Represents a way to get a logs"/>
    /// </summary>
    public interface ILogProvider
    {
        ILog GetLogger(string name);
    }
}