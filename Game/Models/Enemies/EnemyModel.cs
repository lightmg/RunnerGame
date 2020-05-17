using Game.Models.Enemies.Behavior;

namespace Game.Models.Enemies
{
    public class EnemyModel : InGameObject
    {
        public EnemyModel(IEnemyBehavior enemyBehavior, GameObjectParameters gameObjectParameters)
        {
            ObjectParameters = gameObjectParameters;
            EnemyBehavior = enemyBehavior;
        }

        public IEnemyBehavior EnemyBehavior { get; }

        public void Tick(GameState gameState, InGamePosition baseOffset)
        {
            ObjectParameters.Position = EnemyBehavior.Do(gameState, ObjectParameters)
                .Add(baseOffset)
                .Add(Position);
            LifetimeTicks++;
        }
    }
}