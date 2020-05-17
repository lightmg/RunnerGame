using System;
using System.Drawing;
using Game.Models;
using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public class GameRenderersSet
    {
        public IGameObjectRenderer[] ObjectsRenderers { get; set; }

        public Func<GameObjectSize, Image> MissedTextureFactory { get; set; }
    }
}