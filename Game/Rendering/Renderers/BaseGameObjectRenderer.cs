using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Game.Models;

namespace Game.Rendering.Renderers
{
    public abstract class BaseGameObjectRenderer<T> : IGameObjectRenderer where T : InGameObject
    {
        public abstract Image[] Frames { get; }

        protected abstract Func<T, bool> RendererCompabilityChecker { get; }

        public bool IsValidFor(InGameObject gameObject)
        {
            return gameObject is T obj &&
                   (RendererCompabilityChecker?.Invoke(obj) ?? true);
        }
    }
}