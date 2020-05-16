using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Game.Helpers;
using Game.Models.Player;
using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public static class GameRendererSettingsLoader
    {
        private static readonly Lazy<GameResource[]> loadedResources = new Lazy<GameResource[]>(() =>
        {
            var resourcesFolderPath = Path.Combine(PathHelpers.RootPath, "Resources");
            var directoryInfo = new DirectoryInfo(resourcesFolderPath);
            if (!directoryInfo.Exists)
                throw new InvalidOperationException($"Resources folder [{resourcesFolderPath}] doesn't exists");

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
                throw new InvalidOperationException($"Got 0 resources from [{resourcesFolderPath}]");
            return resources;
        });

        public static GameRenderersSet Load(params Type[] knownBehaviors)
        {
            var playerRenderers = loadedResources.Value
                .Where(x => x.FileName.StartsWith("Player", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var playerStateString = string.Join(".", x.FileName.Split("_").Skip(1));
                    var state = playerStateString.ParseEnumOrNull<PlayerState>(true);
                    Console.WriteLine($"{x.FileName} parsed to player state {state?.ToString() ?? "<null>"}");
                    return state == null
                        ? null
                        : new PlayerByStateRenderer(state.Value, x.Image.PrepareImage(new Size(100, 100)));
                })
                .NotNull()
                .ToArray();
            var behaviorTypesByNames = knownBehaviors.ToDictionary(x => x.Name);
            Console.WriteLine(behaviorTypesByNames);
            var enemiesRenderer = loadedResources.Value
                .Where(x => x.FileName.StartsWith("Enemy", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var enemyType = x.FileName.Split("_").Last();
                    var behavior = behaviorTypesByNames.GetValueOrDefault(enemyType, null);
                    Console.WriteLine($"{enemyType} parsed to behavior {behavior?.Name ?? "<null>"}");
                    return behavior == null
                        ? null
                        : new EnemyByBehaviorRenderer(behavior, x.Image.PrepareImage());
                })
                .NotNull()
                .ToArray();
            var defaultRendererResource = loadedResources.Value
                .SingleOrDefault(x => x.FileName.Equals("default", StringComparison.OrdinalIgnoreCase));
            var defaultRenderer = defaultRendererResource == null
                ? null
                : new DefaultGameObjectRenderer(defaultRendererResource.Image.PrepareImage());

            return new GameRenderersSet
            {
                DefaultRenderer = defaultRenderer,
                Renderers = playerRenderers.Cast<IGameObjectRenderer>()
                    .Concat(enemiesRenderer)
                    .ToArray()
            };
        }

        public static GameRenderersSet CreateDebugRenderersSet()
        {
            var playerRenderers = new IGameObjectRenderer[]
            {
                new PlayerByStateRenderer(PlayerState.OnGround,
                    CreateBlinkingSquare(20, 20, Color.Yellow, Color.Chartreuse, Color.Aqua, Color.Chartreuse)),
                new PlayerByStateRenderer(PlayerState.Crouching,
                    CreateBlinkingSquare(20, 10, Color.Aqua, Color.Aquamarine, Color.Azure, Color.Aquamarine)),
                new PlayerByStateRenderer(PlayerState.Jumping,
                    CreateBlinkingSquare(20, 22, Color.Red, Color.DarkRed, Color.IndianRed, Color.DarkRed)),
            };

            var defaultRenderer = new DefaultGameObjectRenderer(DrawingHelpers.CreateSquare(20, 20, Color.Orange));
            return new GameRenderersSet
            {
                Renderers = playerRenderers,
                DefaultRenderer = defaultRenderer
            };
        }

        private static Image[] CreateBlinkingSquare(int width, int height, params Color[] colors)
        {
            return colors.Select(color => DrawingHelpers.CreateSquare(width, height, color)).ToArray();
        }

        private static Image[] PrepareImage(this Image sourceImage, Size? targetSize = null)
        {
            return sourceImage
                .ExtractImageFrames()
                .Select(frame => frame
                    .Resize(targetSize ?? new Size(frame.Width, frame.Height))
                    .AutoCrop(Color.Transparent)
                    .AutoCrop(Color.White)
                    .AutoCrop(Color.Black))
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