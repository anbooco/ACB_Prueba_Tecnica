using ACB_Prueba_Tecnica.Domain.Entities;

namespace ACB_Prueba_Tecnica.Domain.Interfaces.Repositories
{
    public interface IPlayByPlayLeanRepository : IBaseRepository<MatchEvent>
    {
        /// <summary>
        /// Devuelve los datos del partido (gameId) guardados en la BBDD local
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        Task<MatchEvent?> GetByMatchName(int gameId);
    }
}
