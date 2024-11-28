namespace ThePalace.Core.Client.Core.Interfaces
{
    public interface IConsumer : IFeature
    {
        void Consume(params object[] args);
    }
}
