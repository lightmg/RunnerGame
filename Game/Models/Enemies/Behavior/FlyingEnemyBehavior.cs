using System;

namespace Game.Models.Enemies.Behavior
{
    public class FlyingEnemyBehavior : RunningEnemyBehavior
    {
        public FlyingEnemyBehavior() : base(0)
        {
        }

        public FlyingEnemyBehavior(double speed) : base(speed)
        {
        }

        public new static Func<FlyingEnemyBehavior> Creator(double speed = 0)
        {
            return () => new FlyingEnemyBehavior(speed);
        }
    }
}