using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Game.Helpers;
using Game.Models.Player;
using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public class GameRendererSettingsLoader
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
                .Select(fileInfo =>
                {
                    return new GameResource
                    {
                        Image = Image.FromFile(fileInfo.FullName),
                        FileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length)
                    };
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
                        : new PlayerByStateRenderer(state.Value, SplitImageByFrames(x.Image, new Size(100, 100)));
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
                        : new EnemyByBehaviorRenderer(behavior, SplitImageByFrames(x.Image));
                })
                .NotNull()
                .ToArray();
            var defaultRendererResource = loadedResources.Value
                .SingleOrDefault(x => x.FileName.Equals("default", StringComparison.OrdinalIgnoreCase));
            var defaultRenderer = defaultRendererResource == null
                ? null
                : new DefaultGameObjectRenderer(SplitImageByFrames(defaultRendererResource.Image));

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

        private static Image[] SplitImageByFrames(Image image, Size? targetSize = null)
        {
            if (image.RawFormat.Guid != ImageFormat.Gif.Guid)
                return new[] {image};

            var framesCount = image.GetFrameCount(FrameDimension.Time);

            var rawFrames = new Image[framesCount];
            for (var i = 0; i < framesCount; i++)
            {
                image.SelectActiveFrame(FrameDimension.Time, i);
                rawFrames[i] = (Image) image.Clone();
            }

            var capturedFrames = new List<Image>();
            foreach (var frame in rawFrames)
            {
                var newImageSize = targetSize ?? new Size(frame.Width, frame.Height);
                var tempBitmap = new Bitmap(newImageSize.Width, newImageSize.Height);

                using var graphics = Graphics.FromImage(tempBitmap);
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0);
                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, new Rectangle(0, 0, newImageSize.Width, newImageSize.Height), 0,
                    0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

                capturedFrames.Add(tempBitmap);
            }

            return capturedFrames.ToArray();
        }
    }

    public class GameResource
    {
        public string FileName { get; set; }
        public Image Image { get; set; }
    }
}