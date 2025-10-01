namespace Project.Scripts.Boost.Abstract
{
    /// <summary>
    /// Интерфейс для буста с ограниченной продолжительностью.
    /// </summary>
    public interface IBoostDurable : IBoost
    {
        /// <summary>
        /// Возвращает продолжительность действия буста в секундах.
        /// </summary>
        /// <returns>Продолжительность буста.</returns>
        float GetDuration();
    }
}
