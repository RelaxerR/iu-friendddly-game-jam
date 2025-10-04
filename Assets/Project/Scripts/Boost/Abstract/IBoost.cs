namespace Project.Scripts.Boost.Abstract
{
    /// <summary>
    /// Интерфейс для буста.
    /// </summary>
    public interface IBoost
    {
        /// <summary>
        /// Применяет эффект буста.
        /// </summary>
        void Apply();

        /// <summary>
        /// Отменяет эффект буста.
        /// </summary>
        void Remove();
    }
}
