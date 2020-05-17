using System;

namespace Game.Models.Enemies.Behavior
{
    /// <summary>
    /// Defines enemy behavior. Implementors must have parameter-less constructor
    /// </summary>
    public interface IEnemyBehavior
    {
        /// <summary>
        /// Executes enemy move
        /// </summary>
        /// <param name="state"></param>
        /// <param name="enemyParameters">Current coordinate of enemy</param>
        /// <returns>Enemy possition offset</returns>
        public InGamePosition Do(GameState state, GameObjectParameters enemyParameters);

        public static Func<T> CreatorOf<T>() where T : class, IEnemyBehavior, new() => () => new T();
    }
}