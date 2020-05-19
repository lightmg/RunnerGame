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
        private readonly Dictionary<string, Size> enemySizesByBehavior;
        private readonly MissedTextureFactory defaultTextureLoader = new MissedTextureFactory();

        private readonly Lazy<GameResource[]> loadedResources;

        public ResourcesLoader(string resourcesPath,
            Dictionary<PlayerState, GameObjectSize> playerSizesByState,
            Dictionary<string, GameObjectSize> enemySizesByBehavior)
        {
            this.playerSizesByState = playerSizesByState
                .ToDictionary(x => x.Key, x => new Size(x.Value.Width, x.Value.Height));
            this.enemySizesByBehavior = enemySizesByBehavior
                .ToDictionary(x => x.Key, x => new Size(x.Value.Width, x.Value.Height)); 

            loadedResources = new Lazy<GameResource[]>(() =>
            {
                var directoryInfo = new DirectoryInfo(resourcesPath);
                if (!directoryInfo.Exists)
                    throw new InvalidOperationException($"Resources folder [{resourcesPath}] doesn't exists");

                var resources = directoryInfo.GetFiles("*.png")
                    .Concat(directoryInfo.GetFiles("*.gif"))
                    .Concat(directoryInfo.GetFiles("*.jpg"))
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
                        ? new EnemyByBehaviorRenderer(enemyType, PrepareImage(x.Image, val))
                        : null;
                })
                .NotNull()
                .ToArray();

            return new GameRenderersSet
            {
                MissedTextureFactory = defaultTextureLoader.Get,
                ObjectsRenderers = playerRenderers.Cast<IGameObjectRenderer>()
                    .Concat(enemiesRenderer)
                    .ToArray()
            };
        }

        public TexturesRepository LoadTextures()
        {
            return new TexturesRepository(defaultTextureLoader,
                new GameObjectSize {Width = 50, Height = 50},
                loadedResources.Value.ToDictionary(x => x.FileName, x => x.Image));
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