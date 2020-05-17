using System;

namespace Game.Models.Enemies.Behavior
{
    public class RunningEnemyBehavior : IEnemyBehavior
    {
        private readonly double enemySpeed;

        protected RunningEnemyBehavior(double enemySpeed)
        {
            this.enemySpeed = enemySpeed;
        }

        public InGamePosition Do(GameState state, GameObjectParameters enemyParameters)
        {
            return new InGamePosition{X = -state.Speed * enemySpeed};
        }

        public static Func<IEnemyBehavior> Creator(double speed)
        {
            if (Math.Abs(speed) < double.Epsilon)
                return StandingEnemyBehavior.Creator();
            return () => new RunningEnemyBehavior(speed);
        }
    }
}