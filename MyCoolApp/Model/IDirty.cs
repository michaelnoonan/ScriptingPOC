namespace MyCoolApp.Model
{
    public interface IDirty
    {
        bool IsDirty { get; }
        void MarkAsClean();
    }
}