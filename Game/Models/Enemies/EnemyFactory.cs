using System;
using System.Collections.Generic;
using Game.Models.Enemies.Behavior;

namespace Game.Models.Enemies
{
    public class EnemyFactory
    {
        private readonly Dictionary<Type, GameObjectSize> enemiesSizesByBehavior;
        private readonly Func<IEnemyBehavior>[] existingBehaviors;
        private readonly Random random;

        public EnemyFactory(Dictionary<Type, GameObjectSize> enemiesSizesByBehavior,
            params Func<IEnemyBehavior>[] existingBehaviorFactories)
        {
            this.enemiesSizesByBehavior = enemiesSizesByBehavior;
            existingBehaviors = existingBehaviorFactories;
            random = new Random();
        }

        public EnemyModel CreateWithRandomBehavior(InGamePosition position)
        {
            var behaviorIndex = random.Next(existingBehaviors.Length);
            var enemyBehavior = existingBehaviors[behaviorIndex].Invoke();
            if (!(enemyBehavior is FlyingEnemyBehavior))
                position.Y = 0;

            return new EnemyModel(enemyBehavior,
                new GameObjectParameters
                {
                    Position = position.Clone(),
                    Size = enemiesSizesByBehavior[enemyBehavior.GetType()].Clone()
                });
        }
    }
}