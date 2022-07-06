namespace Kachuwa.Web
{
    public abstract class KachuwaWidgetViewComponent<T> : KachuwaViewComponent where T : IWidget, new()
    {

        public override bool IsVisibleOnUI { get; } = false;

    }
}