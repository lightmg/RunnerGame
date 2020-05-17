using System;

namespace Game.Models
{
    public class GameObjectSize : ICloneable<GameObjectSize>, IEquatable<GameObjectSize>
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

        public bool Equals(GameObjectSize other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((GameObjectSize) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }
    }
}