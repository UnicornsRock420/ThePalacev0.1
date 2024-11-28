namespace ThePalace.Core.Client.Core.Interfaces
{
    public interface IProvider : IFeature
    {
        object Provide(params object[] args);
    }
}
