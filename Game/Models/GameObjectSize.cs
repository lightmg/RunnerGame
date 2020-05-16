namespace Game.Models
{
    public class GameObjectSize : ICloneable<GameObjectSize>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public GameObjectSize Clone()
        {
            return new GameObjectSize
            {
                Height = Height,
                Width = Width
            };
        }
    }
}