using System;

namespace Game.Models.Enemies.Behavior
{
    public class FlyingEnemyBehavior : RunningEnemyBehavior
    {
        private FlyingEnemyBehavior(double speed) : base(speed)
        {
        }

        public new static Func<IEnemyBehavior> Creator(double speed = 0)
        {
            return () => new FlyingEnemyBehavior(speed);
        }
    }
}