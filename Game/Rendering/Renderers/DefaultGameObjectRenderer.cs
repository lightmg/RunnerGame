using System;
using System.Drawing;
using Game.Models;

namespace Game.Rendering.Renderers
{
    public class DefaultGameObjectRenderer : BaseGameObjectRenderer<InGameObject>
    {
        public DefaultGameObjectRenderer(params Image[] frames)
        {
            Frames = frames;
        }

        public override Image[] Frames { get; }
        protected override Func<InGameObject, bool> RendererCompabilityChecker => null;
    }
}