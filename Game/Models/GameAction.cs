using System;

namespace Game.Models
{
    [Flags]
    public enum GameAction
    {
        Nothing = 1,
        Start = 2,
        PlayerCrouch = 4,
        PlayerJump = 8,
    }
}