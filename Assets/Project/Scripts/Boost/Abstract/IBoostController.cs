namespace Project.Scripts.Boost.Abstract
{
    /// <summary>
    /// Контроллер управления бустами.
    /// </summary>
    public interface IBoostController
    {
        /// <summary>
        /// Добавляет буст.
        /// </summary>
        /// <param name="boost">Буст для добавления.</param>
        void AddBoost(IBoost boost);

        /// <summary>
        /// Удаляет буст.
        /// </summary>
        /// <param name="boost">Буст для удаления.</param>
        void RemoveBoost(IBoost boost);
    }
}
