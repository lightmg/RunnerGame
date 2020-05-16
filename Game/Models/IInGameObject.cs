namespace Game.Models
{
    public abstract class InGameObject
    {
        public GameObjectParameters ObjectParameters { get; protected set; }
        public InGamePosition Position => ObjectParameters?.Position;
        public ulong LifetimeTicks { get; protected set; }
    }
}