namespace MyCoolApp.Domain.Model
{
    public interface IDirty
    {
        bool IsDirty { get; }
        void MarkAsClean();
    }
}