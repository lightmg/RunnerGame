using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Game.Helpers;
using Game.Models;
using Game.Models.Player;
using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public class ResourcesLoader
    {
        private readonly Dictionary<PlayerState, Size> playerSizesByState;
        private readonly Dictionary<string, (Type behaviorType, Size objectSize)> enemySizesByBehavior;

        private readonly Lazy<GameResource[]> loadedResources;

        public ResourcesLoader(string resourcesPath,
            Dictionary<PlayerState, GameObjectSize> playerSizesByState,
            Dictionary<Type, GameObjectSize> enemySizesByBehavior)
        {
            this.playerSizesByState = playerSizesByState
                .ToDictionary(x => x.Key, x => new Size(x.Value.Width, x.Value.Height));
            this.enemySizesByBehavior = enemySizesByBehavior
                .ToDictionary(x => x.Key.Name, x => (x.Key, new Size(x.Value.Width, x.Value.Height)));

            loadedResources = new Lazy<GameResource[]>(() =>
            {
                var directoryInfo = new DirectoryInfo(resourcesPath);
                if (!directoryInfo.Exists)
                    throw new InvalidOperationException($"Resources folder [{resourcesPath}] doesn't exists");

                var resources = directoryInfo.GetFiles("*.png")
                    .Union(directoryInfo.GetFiles("*.gif"))
                    .Union(directoryInfo.GetFiles("*.jpg"))
                    .Select(fileInfo => new GameResource
                    {
                        Image = Image.FromFile(fileInfo.FullName),
                        FileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length)
                    })
                    .ToArray();
                if (resources.Length == 0)
                    throw new InvalidOperationException($"Got 0 resources from [{resourcesPath}]");
                return resources;
            });
        }

        public GameRenderersSet LoadRenderers()
        {
            var playerRenderers = loadedResources.Value
                .Where(x => x.FileName.StartsWith("Player", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var playerStateString = string.Join(".", x.FileName.Split("_").Skip(1));
                    var state = playerStateString.ParseEnumOrNull<PlayerState>(true);
                    return state == null
                        ? null
                        : new PlayerByStateRenderer(state.Value,
                            PrepareImage(x.Image, playerSizesByState[state.Value]));
                })
                .NotNull()
                .ToArray();

            var enemiesRenderer = loadedResources.Value
                .Where(x => x.FileName.StartsWith("Enemy", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var enemyType = x.FileName.Split("_").Last();
                    var behaviorRecognized = enemySizesByBehavior.TryGetValue(enemyType, out var val);
                    return behaviorRecognized
                        ? new EnemyByBehaviorRenderer(val.behaviorType, PrepareImage(x.Image, val.objectSize))
                        : null;
                })
                .NotNull()
                .ToArray();
            var defaultRendererResource = loadedResources.Value
                .SingleOrDefault(x => x.FileName.Equals("default", StringComparison.OrdinalIgnoreCase));
            var defaultRenderer = defaultRendererResource == null
                ? null
                : new DefaultGameObjectRenderer(PrepareImage(defaultRendererResource.Image));

            return new GameRenderersSet
            {
                DefaultRenderer = defaultRenderer,
                Renderers = playerRenderers.Cast<IGameObjectRenderer>()
                    .Concat(enemiesRenderer)
                    .ToArray()
            };
        }

        private static Image[] CreateBlinkingSquare(int width, int height, params Color[] colors)
        {
            return colors.Select(color => DrawingHelpers.CreateSquare(width, height, color)).ToArray();
        }

        private static Image[] PrepareImage(Image sourceImage, Size? targetSize = null)
        {
            return sourceImage
                .ExtractImageFrames()
                .Select(frame => new Bitmap(frame))
                .Select(frame => frame
                    .AutoCrop(Color.Transparent)
                    .AutoCrop(Color.White)
                    .AutoCrop(Color.Black)
                    .Resize(targetSize ?? new Size(frame.Width, frame.Height)))
                .Cast<Image>()
                .ToArray();
        }

        private class GameResource
        {
            public string FileName { get; set; }
            public Image Image { get; set; }
        }
    }
}