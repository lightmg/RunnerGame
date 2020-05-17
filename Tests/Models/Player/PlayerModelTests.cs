using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using Game.Models.Player;
using NUnit.Framework;

namespace Tests.Models.Player
{
    public class PlayerModelTests
    {
        private const int InitialHealth = 3;
        private PlayerModel playerModel;

        private static readonly Dictionary<PlayerState, Dictionary<PlayerAction, PlayerState>>
            validActionReactionsByInitialState = new Dictionary<PlayerState, Dictionary<PlayerAction, PlayerState>>
            {
                {
                    PlayerState.Crouching,
                    new Dictionary<PlayerAction, PlayerState>
                    {
                        {PlayerAction.Crouch, PlayerState.Crouching},
                        {PlayerAction.Jump, PlayerState.Jumping},
                        {PlayerAction.Nothing, PlayerState.OnGround}
                    }
                },

                {
                    PlayerState.Jumping,
                    new Dictionary<PlayerAction, PlayerState>
                    {
                        {PlayerAction.Crouch, PlayerState.Jumping},
                        {PlayerAction.Jump, PlayerState.Jumping},
                        {PlayerAction.Nothing, PlayerState.Jumping}
                    }
                },
                {
                    PlayerState.OnGround,
                    new Dictionary<PlayerAction, PlayerState>
                    {
                        {PlayerAction.Crouch, PlayerState.Crouching},
                        {PlayerAction.Jump, PlayerState.Jumping},
                        {PlayerAction.Nothing, PlayerState.OnGround}
                    }
                }
            };

        [SetUp]
        public void Setup()
        {
            playerModel = CreatePlayer(PlayerState.OnGround);
        }

        [Test]
        public void DoNothing()
        {
            playerModel.Tick(PlayerAction.Nothing);
            Assert.That(playerModel.Position.Y, Is.EqualTo(0d));
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth));
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.OnGround));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(0));
        }

        [Test]
        public void TakeDamage_CorrectlyHandleDamage()
        {
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(0));
            playerModel.Tick(PlayerAction.Nothing, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth - 1));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(1));
            playerModel.Tick(PlayerAction.Nothing, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth - 2));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(2));
            playerModel.Tick(PlayerAction.Nothing, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth - 3));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(3));
        }

        [Test]
        public void TakeDamage_CantMoveBelowZero()
        {
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(0));
            for (var i = 0;
                i < InitialHealth;
                i++)
                playerModel.Tick(PlayerAction.Nothing, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(0));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(3));
            playerModel.Tick(PlayerAction.Nothing, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(0));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(3));
        }

        [Test]
        public void TakeDamage_CanTakeOnEachPlayerAction([Values] PlayerAction playerAction)
        {
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(0));
            playerModel.Tick(playerAction, true);
            Assert.That(playerModel.CurrentHealth, Is.EqualTo(InitialHealth - 1));
            Assert.That(playerModel.TakenDamage, Is.EqualTo(1));
        }

        [Test]
        public void Crouching()
        {
            playerModel.Tick(PlayerAction.Crouch);
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.Crouching));
            Assert.That(playerModel.Position.Y, Is.EqualTo(0d));
            playerModel.Tick(PlayerAction.Nothing);
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.OnGround));
            Assert.That(playerModel.Position.Y, Is.EqualTo(0d));
        }

        [Test]
        public void Jump()
        {
            playerModel.Tick(PlayerAction.Jump);
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.Jumping));
            Assert.That(playerModel.Position.Y, Is.Not.EqualTo(0d));
            playerModel.Tick(PlayerAction.Nothing);
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.Jumping));
            Assert.That(playerModel.Position.Y, Is.Not.EqualTo(0d));
            Console.WriteLine(playerModel.Position.Y);
            Waiter.Wait(() =>
            {
                playerModel.Tick(PlayerAction.Nothing);
                Console.WriteLine(playerModel.Position.Y);
                return playerModel.State;
            }, PlayerState.OnGround, 0, TimeSpan.FromSeconds(2));
            Assert.That(playerModel.State, Is.EqualTo(PlayerState.OnGround));
            Assert.That(playerModel.Position.Y, Is.EqualTo(0d));
        }

        [TestCaseSource(nameof(StateChanged_TestCases))]
        public void StateChanges(PlayerState previousState, PlayerAction action,
            PlayerState expectedState)
        {
            playerModel = CreatePlayer(previousState);
            Assert.That(playerModel.State, Is.EqualTo(previousState));
            playerModel.Tick(action);
            Assert.That(playerModel.State, Is.EqualTo(expectedState));
        }

        [Test]
        public void OppositeActionsDoesNothing([Values] PlayerState initialState)
        {
            playerModel = CreatePlayer(initialState);
            Assert.That(playerModel.State, Is.EqualTo(initialState));
            playerModel.Tick(PlayerAction.Crouch | PlayerAction.Jump);
            Assert.That(playerModel.State,
                Is.EqualTo(validActionReactionsByInitialState[initialState][PlayerAction.Nothing]));
        }

        private static IEnumerable StateChanged_TestCases()
        {
            return validActionReactionsByInitialState.SelectMany(x =>
                x.Value.Select(y => new object[] {x.Key, y.Key, y.Value}));
        }

        private PlayerModel CreatePlayer(PlayerState initialState)
        {
            return new PlayerModel(initialHealth: InitialHealth,
                position: new InGamePosition
                {
                    X = 10, Y = 0
                },
                sizesByState: new Dictionary<PlayerState, GameObjectSize>
                {
                    {
                        PlayerState.Crouching, new GameObjectSize {Width = 20, Height = 20}
                    },
                    {
                        PlayerState.Jumping, new GameObjectSize {Width = 20, Height = 20}
                    },
                    {
                        PlayerState.OnGround, new GameObjectSize {Width = 20, Height = 20}
                    }
                }, initialState: initialState);
        }
    }
}