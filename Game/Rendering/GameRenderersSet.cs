using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public class GameRenderersSet
    {
        public IGameObjectRenderer[] Renderers { get; set; }
        public IGameObjectRenderer DefaultRenderer { get; set; }
    }
}