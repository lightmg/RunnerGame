namespace Game.Models
{
    public class GameObjectParameters : ICloneable<GameObjectParameters>
    {
        public InGamePosition Position { get; set; }
        public GameObjectSize Size { get; set; }

        public bool HasCollision(GameObjectParameters other)
        {
            var thisCoords = GetFullCoordinates(this);
            var otherCoords = GetFullCoordinates(other);
            return otherCoords.X < thisCoords.X1 &&
                   thisCoords.X < otherCoords.X1 &&
                   otherCoords.Y < thisCoords.Y1 &&
                   thisCoords.Y < otherCoords.Y1;
        }

        public GameObjectParameters Clone()
        {
            return new GameObjectParameters
            {
                Position = Position.Clone(),
                Size = Size.Clone()
            };
        }

        private (double X, double Y, double X1, double Y1) GetFullCoordinates(GameObjectParameters objectParameters)
        {
            return (X: objectParameters.Position.X,
                Y: objectParameters.Position.Y,
                X1: objectParameters.Position.X + objectParameters.Size.Width,
                Y1: objectParameters.Position.Y + objectParameters.Size.Height);
        }
    }
}