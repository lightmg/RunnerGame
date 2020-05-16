using System;

namespace Game.Models
{
    public class GameState
    {
        private const double MaxGameSpeed = 10d;

        /// <summary>
        /// Минимальное перемещение за тик
        /// </summary>
        public double Speed { get; private set; }

        public ulong Score { get; private set; }
        public bool PlayerAlive { get; set; } = true;
        public int PlayerHealth { get; set; }
        public readonly GameFieldSize FieldSize;

        public GameState(GameFieldSize fieldSize, ulong score, double speed)
        {
            FieldSize = fieldSize;
            Score = score;
            Speed = speed;
        }

        public void Tick()
        {
            Score += (ulong) Speed;
            Speed += Math.Min(0.005, MaxGameSpeed);
        }
    }
}