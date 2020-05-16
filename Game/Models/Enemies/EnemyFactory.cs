using System;
using Game.Models.Enemies.Behavior;

namespace Game.Models.Enemies
{
    public class EnemyFactory
    {
        public GameObjectSize DefaultEnemySize { get; set; }
        private readonly Func<IEnemyBehavior>[] existingBehaviors;
        private readonly Random random;

        public EnemyFactory(GameObjectSize defaultEnemySize, params Func<IEnemyBehavior>[] existingBehaviorFactories)
        {
            DefaultEnemySize = defaultEnemySize;
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
                    Size = DefaultEnemySize.Clone()
                });
        }
    }
}