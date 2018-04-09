namespace Kachuwa.Core.DI
{
    internal interface IBootstrapper
    {
        void Init();
        bool Build();
    }
}