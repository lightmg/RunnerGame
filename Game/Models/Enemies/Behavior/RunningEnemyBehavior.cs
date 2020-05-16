using System;

namespace Game.Models.Enemies.Behavior
{
    public class RunningEnemyBehavior : IEnemyBehavior
    {
        private readonly double enemySpeed;

        public RunningEnemyBehavior(double enemySpeed)
        {
            this.enemySpeed = enemySpeed;
        }

        public InGamePosition Do(GameState state, GameObjectParameters enemyParameters, ulong enemyLifetimeTicks)
        {
            return enemyParameters.Position.Add(-state.Speed * enemySpeed);
        }

        public static Func<RunningEnemyBehavior> Creator(double speed = 0)
        {
            return () => new RunningEnemyBehavior(speed);
        }
    }
}