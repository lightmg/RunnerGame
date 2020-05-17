using System;
using System.Collections.Generic;
using Game.Helpers;

namespace Game.Models.Player
{
    public class PlayerModel : InGameObject
    {
        //Jump maximal height = playerHeight * JumpPeakByPlayerHeight;
        private const double JumpPeakByPlayerHeight = 1.5;
        private const double GravityAcceleration = 0.981d;
        private readonly double initialJumpSpeed;
        private readonly int initialHealth;
        private readonly Dictionary<PlayerState, GameObjectSize> sizesByState;

        public PlayerModel(int initialHealth, InGamePosition position,
            Dictionary<PlayerState, GameObjectSize> sizesByState, PlayerState initialState = PlayerState.OnGround)
        {
            this.initialHealth = initialHealth;
            this.sizesByState = sizesByState;
            State = initialState;
            ObjectParameters = new GameObjectParameters
            {
                Position = position,
                Size = sizesByState[PlayerState.OnGround]
            };
            initialJumpSpeed =
                Math.Sqrt(ObjectParameters.Size.Height * GravityAcceleration * (1 + JumpPeakByPlayerHeight));
            MaxJumpHeight = ObjectParameters.Size.Height * JumpPeakByPlayerHeight;
        }

        public int CurrentHealth => initialHealth - TakenDamage;
        public bool IsAlive => CurrentHealth > 0;
        public int TakenDamage { get; private set; }
        public PlayerState State { get; private set; }
        public double MaxJumpHeight { get; private set; }
        private int TicksAfterJump { get; set; }

        public void Tick(PlayerAction action, bool isDamageTaken = false)
        {
            var previousState = State;
            if (action == (PlayerAction.Crouch | PlayerAction.Jump))
                action = PlayerAction.Nothing;
            State = action switch
            {
                PlayerAction.Jump when State.In(PlayerState.OnGround, PlayerState.Crouching) => PlayerState.Jumping,
                PlayerAction.Crouch when State == PlayerState.OnGround => PlayerState.Crouching,
                PlayerAction.Nothing when State == PlayerState.Crouching => PlayerState.OnGround,
                _ => State
            };
            if (State != previousState)
                Console.WriteLine(
                    $"Changed player state from {previousState.ToString()} to {State.ToString()}");

            if (isDamageTaken && IsAlive)
                TakenDamage++;

            if (State == PlayerState.Jumping)
            {
                ObjectParameters.Position.Y = CalculateHeight(TicksAfterJump++);
                if (ObjectParameters.Position.Y <= 0)
                {
                    TicksAfterJump = 0;
                    State = PlayerState.OnGround;
                }
            }

            UpdateSize();
            LifetimeTicks++;
        }

        private double CalculateHeight(int ticksAfterJump)
        {
            if (ticksAfterJump == 0)
                ticksAfterJump = 1;
            //https://lampa.io/p/00000000aec967390a34cd4976d5458f
            var calculatedHeight = initialJumpSpeed * ticksAfterJump -
                                   GravityAcceleration * ticksAfterJump * ticksAfterJump / 2;
            return calculatedHeight > 0 ? calculatedHeight : 0;
        }

        private void UpdateSize()
        {
            ObjectParameters.Size = sizesByState.TryGetValue(State, out var size)
                ? size
                : throw new InvalidOperationException($"Can't get size for state [{State.ToString()}]");
        }
    }
}