namespace Game.Models.Enemies
{
    public interface IEnemyFactory
    {
        EnemyModel Create(InGamePosition position);
    }
}