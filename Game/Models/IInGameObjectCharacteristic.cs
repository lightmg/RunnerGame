namespace Game.Models
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}