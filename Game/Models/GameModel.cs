using System;
using System.Collections.Generic;
using Game.Helpers;
using Game.Models.Enemies;
using Game.Models.Player;

namespace Game.Models
{
    public class GameModel
    {
        private const double InitialMinimalDistanceBetweenEnemies = 100d;
        private const int MaxEnemiesOnScreen = 2;

        private readonly List<EnemyModel> enemies = new List<EnemyModel>();
        private readonly Random random = new Random();
        private readonly EnemyFactory enemyFactory;
        private readonly GameFieldSize gameFieldSize;
        private readonly Dictionary<PlayerState, GameObjectSize> playerSizesByStates;
        private double MinDistanceBetweenEnemies => Math.Sqrt(State.Speed) * InitialMinimalDistanceBetweenEnemies;

        public GameModel(EnemyFactory enemyFactory, GameFieldSize gameFieldSize,
            Dictionary<PlayerState, GameObjectSize> playerSizesByStates)
        {
            Console.WriteLine($"Starting game with field {gameFieldSize.Width}x{gameFieldSize.Height}");
            this.enemyFactory = enemyFactory;
            this.gameFieldSize = gameFieldSize;
            this.playerSizesByStates = playerSizesByStates;
            Reset();
            State.PlayerAlive = false;
        }

        public IEnumerable<InGameObject> ObjectsToRender => enemies.ConcatWith(Player as InGameObject);
        public GameState State { get; private set; }
        private PlayerModel Player { get; set; }

        public void Tick(GameAction action)
        {
            if (!State.PlayerAlive)
            {
                if (action.HasFlag(GameAction.Start))
                    Reset();
                return;
            }

            var enemiesToRemove = new List<EnemyModel>();
            var playerShouldTakeDamage = false;
            var maxEnemyXCoordinate = 0d;
            foreach (var enemy in enemies)
                if (enemy.ObjectParameters.HasCollision(Player.ObjectParameters))
                {
                    enemiesToRemove.Add(enemy);
                    playerShouldTakeDamage = true;
                }
                else
                {
                    if (enemy.Position.X + enemy.ObjectParameters.Size.Width <= 0 ||
                        enemy.Position.X >= gameFieldSize.Width + 5)
                        enemiesToRemove.Add(enemy);
                    else
                    {
                        enemy.Tick(State, new InGamePosition {X = -State.Speed});
                        maxEnemyXCoordinate = Math.Max(enemy.Position.X, maxEnemyXCoordinate);
                    }
                }

            foreach (var enemy in enemiesToRemove)
            {
                enemies.Remove(enemy);
                Console.WriteLine($"To remove: ({enemy.Position.X}, {enemy.Position.Y})");
            }

            if ((enemies.Count == 0 || maxEnemyXCoordinate <= gameFieldSize.Width - MinDistanceBetweenEnemies) &&
                random.Next(0, 10) == 0 && enemies.Count < MaxEnemiesOnScreen)
            {
                var enemy = CreateEnemy();
                enemies.Add(enemy);
                enemy.Tick(State, new InGamePosition {X = -State.Speed});
            }

            Player.Tick(GetPlayerAction(action), playerShouldTakeDamage);
            State.Tick();
            State.PlayerAlive = Player.IsAlive;
            State.PlayerHealth = Player.CurrentHealth;
        }

        private void Reset()
        {
            enemies.Clear();

            Player = new PlayerModel(3, new InGamePosition
            {
                X = 30,
                Y = 0,
            }, playerSizesByStates, State);
            State = new GameState(gameFieldSize, 1, 1)
            {
                PlayerHealth = Player.CurrentHealth
            };
        }

        private EnemyModel CreateEnemy()
        {
            var height = random.Next(0, Player.ObjectParameters.Size.Height * 3);
            var createdEnemy = enemyFactory.CreateWithRandomBehavior(new InGamePosition
            {
                X = gameFieldSize.Width - 1,
                Y = height
            });
            Console.WriteLine($"Spawned enemy at {createdEnemy.Position}");
            return createdEnemy;
        }

        private PlayerAction GetPlayerAction(GameAction gameAction)
        {
            var playerAction = PlayerAction.Nothing;
            if (gameAction.HasFlag(GameAction.PlayerCrouch))
                playerAction |= PlayerAction.Crouch;
            if (gameAction.HasFlag(GameAction.PlayerJump))
                playerAction |= PlayerAction.Jump;
            return playerAction;
        }
    }
}