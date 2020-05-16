using System.Drawing;
using Game.Models;

namespace Game.Rendering.Renderers
{
    public interface IGameObjectRenderer
    {
        bool IsValidFor(InGameObject gameObject);
        Image[] Frames { get; }
    }
}