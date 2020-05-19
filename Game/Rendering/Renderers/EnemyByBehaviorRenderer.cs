using System;
using System.Drawing;
using Game.Models.Enemies;

namespace Game.Rendering.Renderers
{
    public class EnemyByBehaviorRenderer : BaseGameObjectRenderer<EnemyModel>
    {
        public EnemyByBehaviorRenderer(string behaviorType, params Image[] frames)
        {
            Behavior = behaviorType;
            Frames = frames;
        }

        public string Behavior { get; }
        public override Image[] Frames { get; }

        protected override Func<EnemyModel, bool> RendererCompabilityChecker =>
            enemy => enemy.EnemyBehavior?.GetType().Name == Behavior;
    }
}