using System;
using System.Drawing;
using Game.Models.Player;

namespace Game.Rendering.Renderers
{
    public class PlayerByStateRenderer : BaseGameObjectRenderer<PlayerModel>
    {
        public PlayerByStateRenderer(PlayerState state, params Image[] frames)
        {
            State = state;
            Frames = frames;
        }

        public PlayerState State { get; }

        public override Image[] Frames { get; }

        protected override Func<PlayerModel, bool> RendererCompabilityChecker =>
            player => player.State == State;
    }
}