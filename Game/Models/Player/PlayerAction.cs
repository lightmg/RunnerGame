using System;

namespace Game.Models.Player
{
    [Flags]
    public enum PlayerAction
    {
        Nothing,
        Crouch,
        Jump,
    }
}