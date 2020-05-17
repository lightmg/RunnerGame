using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Game.Helpers;
using Game.Models;
using Game.Rendering.Renderers;

namespace Game.Rendering
{
    public class GameRenderer
    {
        private const int InitialTicksPerFrame = 6;
        private readonly PointF gameOrigin;
        private readonly GameRenderersSet renderersSet;

        public GameRenderer(PointF gameOrigin, GameRenderersSet renderersSet)
        {
            this.gameOrigin = gameOrigin;
            this.renderersSet = renderersSet;
        }

        public IEnumerable<ImageRenderInfo> Render(GameModel gameModel, bool debugging = false)
        {
            return gameModel.ObjectsToRender
                .Select(obj => new
                {
                    Point = gameOrigin.Add(obj.Position.X, -obj.Position.Y - obj.ObjectParameters.Size.Height),
                    Renderer = GetRendererForObjectOrNull(obj),
                    GameObject = obj
                })
                .Select(x => new ImageRenderInfo
                {
                    Point = x.Point,
                    Image = RenderObject(gameModel.State, x.GameObject, x.Renderer)
                })
                .If(debugging, q => q.Select(x => new ImageRenderInfo
                    {
                        Point = x.Point,
                        Image = x.Image.AddBorders(Color.Red)
                    })
                    .ConcatWith(new ImageRenderInfo
                    {
                        Image = DrawingHelpers.CreateSquare(3, 3, Color.GreenYellow),
                        Point = gameOrigin
                    }))
                .ToArray();
        }

        private IGameObjectRenderer GetRendererForObjectOrNull<T>(T gameObject) where T : InGameObject
        {
            return renderersSet.ObjectsRenderers.FirstOrDefault(r => r.IsValidFor(gameObject));
        }

        private Image RenderObject(GameState state, InGameObject gameObject, IGameObjectRenderer renderer)
        {
            if (renderer == null)
                return renderersSet.MissedTextureFactory.Invoke(gameObject.ObjectParameters.Size);
            var ticksPerFrame = (int) (InitialTicksPerFrame / Math.Sqrt(Math.Sqrt(state.Speed)));
            var frameNum = (int) (gameObject.LifetimeTicks % (ulong) (renderer.Frames.Length * ticksPerFrame)) /
                           ticksPerFrame;
            return renderer.Frames[frameNum];
        }
    }
}