using System;
using Game.Models;
using Game.Models.Enemies;
using Game.Models.Enemies.Behavior;
using NUnit.Framework;

namespace Tests.Models.Enemies
{
    public class EnemyModelTests
    {
        [Test]
        public void MoveBaseOffset()
        {
            var gameState = new GameState(new GameFieldSize(100, 300), 23, 31);
            var enemyPosition = new InGamePosition {X = 3, Y = 5};
            var enemySize = new GameObjectSize {Height = 137, Width = 253};

            var enemyBehavior = CreateBehavior((state, parameters) =>
            {
                Assert.That(state.Score, Is.EqualTo(gameState.Score));
                Assert.That(state.Speed, Is.EqualTo(gameState.Speed));
                Assert.That(state.FieldSize.Height, Is.EqualTo(gameState.FieldSize.Height));
                Assert.That(state.FieldSize.Width, Is.EqualTo(gameState.FieldSize.Width));
                Assert.That(parameters.Position, Is.EqualTo(enemyPosition));
                Assert.That(parameters.Size, Is.EqualTo(enemySize));

                return new InGamePosition {X = 0, Y = 0};
            });

            var enemyModel = CreateEnemyModel(position: enemyPosition, enemyBehavior: enemyBehavior, size: enemySize);
            Assert.That(enemyModel.Position.X, Is.EqualTo(3));
            Assert.That(enemyModel.Position.Y, Is.EqualTo(5));
            Assert.That(enemyModel.EnemyBehavior, Is.InstanceOf<TestingEnemyBehavior>());
            Assert.That(enemyBehavior.InvocationsCount, Is.Zero);

            enemyModel.Tick(gameState, new InGamePosition {X = 1000, Y = -1000});
            Assert.That(enemyBehavior.InvocationsCount, Is.EqualTo(1));
            Assert.That(enemyModel.Position.X, Is.EqualTo(1000d));
            Assert.That(enemyModel.Position.Y, Is.EqualTo(-1000d));
        }

        [Test]
        public void AddBehaviorMovementToBaseOffset()
        {
            var behavior = CreateBehavior((_, gameObjParams) => gameObjParams.Position.Add(2000, -7000));
            var enemyModel = CreateEnemyModel(position: new InGamePosition {X = 0, Y = 0}, enemyBehavior: behavior);

            Assert.That(enemyModel.Position.X, Is.EqualTo(0));
            Assert.That(enemyModel.Position.Y, Is.EqualTo(0));
            Assert.That(behavior.InvocationsCount, Is.Zero);

            enemyModel.Tick(new GameState(new GameFieldSize(200, 100), 1, 1),
                new InGamePosition {X = 1000, Y = -1000});
            
            Assert.That(behavior.InvocationsCount, Is.EqualTo(1));
            Assert.That(enemyModel.Position.X, Is.EqualTo(3000));
            Assert.That(enemyModel.Position.Y, Is.EqualTo(-8000));
        }

        private static EnemyModel CreateEnemyModel(GameObjectSize size = null, InGamePosition position = null,
            IEnemyBehavior enemyBehavior = null)
        {
            return new EnemyModel(enemyBehavior, new GameObjectParameters
            {
                Position = position ?? new InGamePosition {X = 3, Y = 5},
                Size = size ?? new GameObjectSize {Width = 17, Height = 13}
            });
        }

        private static TestingEnemyBehavior CreateBehavior(
            Func<GameState, GameObjectParameters, InGamePosition> callback = null)
        {
            var behavior = new TestingEnemyBehavior
            {
                InvocationsCount = 0
            };
            behavior.OnDo += callback ?? ((_, gameObjectParameters) => new InGamePosition {X = 0, Y = 0});
            return behavior;
        }

        private class TestingEnemyBehavior : IEnemyBehavior
        {
            public event Func<GameState, GameObjectParameters, InGamePosition> OnDo;
            public int InvocationsCount { get; set; } = 0;

            public InGamePosition Do(GameState state, GameObjectParameters enemyParameters)
            {
                InvocationsCount++;
                return OnDo?.Invoke(state, enemyParameters);
            }
        }
    }
}