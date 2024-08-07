using ACB_Prueba_Tecnica.DAL.Context;
using ACB_Prueba_Tecnica.Domain.Entities;
using ACB_Prueba_Tecnica.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ACB_Prueba_Tecnica.DAL.Repositories
{
    public class PlayByPlayLeanRepository : BaseRepository<MatchEvent>, IPlayByPlayLeanRepository
    {
        public PlayByPlayLeanRepository(ACBContext context) : base(context)
        {
        }

        public async Task<MatchEvent?> GetByMatchName(int gameId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.IdGame == gameId);
        }
    }
}
