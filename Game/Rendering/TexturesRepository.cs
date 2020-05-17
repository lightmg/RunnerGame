using System.Collections.Generic;
using System.Drawing;
using Game.Models;

namespace Game.Rendering
{
    public class TexturesRepository
    {
        private readonly MissedTextureFactory missedTextureFactory;
        private readonly GameObjectSize defaultSize;
        private readonly Dictionary<string, Image> texturesCache;

        public TexturesRepository(MissedTextureFactory missedTextureFactory, GameObjectSize defaultSize,
            Dictionary<string, Image> texturesCache)
        {
            this.missedTextureFactory = missedTextureFactory;
            this.defaultSize = defaultSize;
            this.texturesCache = texturesCache;
        }

        public Image Get(string name)
        {
            if (texturesCache.TryGetValue(name, out var texture))
                return texture;
            texture = missedTextureFactory.Get(defaultSize);
            texturesCache.Add(name, texture);
            return texture;
        }
    }
}