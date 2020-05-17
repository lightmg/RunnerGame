using System;

namespace Game.Models.Enemies.Behavior
{
    public class StandingEnemyBehavior : IEnemyBehavior
    {
        public InGamePosition Do(GameState state, GameObjectParameters enemyParameters)
        {
            return new InGamePosition();
        }

        public static Func<IEnemyBehavior> Creator()
        {
            return () => new StandingEnemyBehavior();
        }
    }
}