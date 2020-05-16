using System;
using System.Drawing;
using Game.Models.Enemies;

namespace Game.Rendering.Renderers
{
    public class EnemyByBehaviorRenderer : BaseGameObjectRenderer<EnemyModel>
    {
        public EnemyByBehaviorRenderer(Type behaviorType, params Image[] frames)
        {
            BehaviorType = behaviorType;
            Frames = frames;
        }

        public Type BehaviorType { get; }
        public override Image[] Frames { get; }

        protected override Func<EnemyModel, bool> RendererCompabilityChecker =>
            enemy => (enemy.EnemyBehavior == null
                ? null
                : enemy.EnemyBehavior.GetType()) == BehaviorType;
    }
}