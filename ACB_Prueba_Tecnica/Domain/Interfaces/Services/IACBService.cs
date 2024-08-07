namespace ACB_Prueba_Tecnica.Domain.Interfaces.Services
{
    public interface IACBService
    {
        Task<List<Dictionary<string, object>>> GetPlayByPlayLean(int gameId);
        Task<Dictionary<string, object>> GetGameLeaders(int gameId);
        Task<Dictionary<string, int>> GetGameBiggestLeaders(int gameId);
    }
}
