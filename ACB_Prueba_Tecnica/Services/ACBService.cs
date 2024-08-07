using ACB_Prueba_Tecnica.Domain.Entities;
using ACB_Prueba_Tecnica.Domain.Interfaces.Repositories;
using ACB_Prueba_Tecnica.Domain.Interfaces.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ACB_Prueba_Tecnica.Services
{
    public class ACBService : IACBService
    {
        private readonly IConfiguration _config;
        private readonly IPlayByPlayLeanRepository _repo;
        private readonly IHttpClientFactory _httpClientFactory;

        public ACBService(IConfiguration config, IHttpClientFactory httpClientFactory, IPlayByPlayLeanRepository repo)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _repo = repo;
        }

        /// <summary>
        /// Devuelve una versión reducida de la información del play by play para el partido especificado por su ID. 
        /// Si los datos se consultan por primera vez o en BBDD no hay datos, la respuesta de la API se guarda en BBDD
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<List<Dictionary<string, object>>> GetPlayByPlayLean(int gameId)
        {
            try {
                var matchEvent = await _repo.GetByMatchName(gameId);
                if (matchEvent is not null && !string.IsNullOrEmpty(matchEvent.PlayByPlayData)) {
                    return GetListFromResponse(matchEvent.PlayByPlayData);
                }

                var response = await GetDataFromACB_API(gameId);
                var playByPlaysLean = GetListFromResponse(response);

                var matchDB = new MatchEvent() {
                    Id = 0,
                    IdGame = gameId,
                    PlayByPlayData = response,
                    LastUpdated = DateTime.Now
                };

                await _repo.Add(matchDB);

                return playByPlaysLean;
            }
            catch (Exception ex) {
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Devuelve los líderes de ambos equipos en puntos y rebotes para el partido especificado por su ID(game_id). 
        /// Los datos están ordenados por puntos en orden descendente y luego por rebotes en orden descendente.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetGameLeaders(int gameId)
        {
            string response;
            List<Dictionary<string, object>> data;

            try {
                var matchEvent = await _repo.GetByMatchName(gameId);
                if (matchEvent is not null && !string.IsNullOrEmpty(matchEvent.PlayByPlayData)) {
                    response = matchEvent.PlayByPlayData;
                }
                else {
                    response = await GetDataFromACB_API(gameId);
                }

                var playersStats = GetPlayersStatsFromResponse(response);
                var orderedStats = playersStats
                   .OrderByDescending(player => player["points"])
                   .ThenByDescending(player => player["total_rebound"])
                   .ToList();

                var homeTeam = orderedStats.Where(player => player["local"].Equals(true)).ToList();
                var awayTeam = orderedStats.Where(player => player["local"].Equals(false)).ToList();

                return new Dictionary<string, object>()
                {
                    { "home_team_leaders", homeTeam },
                    { "away_team_leaders", awayTeam }
                };
            }
            catch (Exception ex) {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Devuelve la mayor diferencia de puntos entre ambos equipos durante el partido especificado por su ID
        /// </summary>
        /// <param name="gameId">Id de partido proporcionado</param>
        /// <returns></returns>
        public async Task<Dictionary<string, int>> GetGameBiggestLeaders(int gameId)
        {
            try {
                var response = await GetDataFromACB_API(gameId);
                var playByPlayData = GetPlayByPlayFromResponse(response);

                var maxLeadHome = 0;
                var maxLeadAway = 0;

                foreach (var play in playByPlayData) {
                    var scoreLocal = play.ContainsKey("score_local") ? Convert.ToInt32(play["score_local"]) : 0;
                    var scoreVisitor = play.ContainsKey("score_visitor") ? Convert.ToInt32(play["score_visitor"]) : 0;
                    var leadDifference = scoreLocal - scoreVisitor;

                    if (leadDifference > maxLeadHome)
                        maxLeadHome = leadDifference;
                    else if (-leadDifference > maxLeadAway)
                        maxLeadAway = -leadDifference;
                }

                var result = new Dictionary<string, int>
                {
                    { "home_team", maxLeadHome },
                    { "away_team", maxLeadAway }
                };

                return result;
            }
            catch (Exception ex) {
                return new Dictionary<string, int>
                {
                    { "home_team", 0 },
                    { "away_team", 0 }
                };
            }
        }


        #region Private Methods
        /// <summary>
        /// Hace la llamada a la API de ACB con la Url y Token del AppSettings
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        private async Task<string> GetDataFromACB_API(int gameId)
        {
            var endpoint = _config["ACBApiUrl"];
            var action = $"PlayByPlay/matchevents?idMatch={gameId}";
            var url = $"{endpoint}/{action}";
            var token = _config["ACBApiToken"];

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetStringAsync(url);
            return response;
        }

        /// <summary>
        /// Devuelve el listado reducido de los datos de la API de ACB. Solo con id_team_denomination, id_license, crono y id_playbyplaytype  
        /// </summary>
        /// <param name="response">Respuesta de la llamada a la API de ACB hecha</param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetListFromResponse(string response)
        {
            var playByPlaysLean = new List<Dictionary<string, object>>();

            using (JsonDocument doc = JsonDocument.Parse(response)) {
                foreach (var element in doc.RootElement.EnumerateArray()) {
                    var playByPlay = new Dictionary<string, object>();

                    if (element.TryGetProperty("team", out JsonElement teamElement) && teamElement.ValueKind != JsonValueKind.Null) {
                        playByPlay["id_team_denomination"] = (teamElement.TryGetProperty("id_team_denomination", out JsonElement idTeamDenominationElement) && idTeamDenominationElement.ValueKind != JsonValueKind.Null)
                            ? idTeamDenominationElement.GetInt32()
                            : "";
                    }
                    else {
                        Console.WriteLine("Warning: Obviamos item debida a que 'team' es NULL");
                        continue;
                    }

                    if (element.TryGetProperty("id_license", out JsonElement idLicenseElement) && idLicenseElement.ValueKind != JsonValueKind.Null) {
                        playByPlay["id_license"] = idLicenseElement.GetInt32();
                    }
                    else {
                        Console.WriteLine("Warning: Obviamos item debida a que 'id_license' es NULL");
                        continue;
                    }

                    playByPlay["crono"] = (element.TryGetProperty("crono", out JsonElement cronoElement) && cronoElement.ValueKind != JsonValueKind.Null)
                        ? cronoElement.GetString()
                        : "";

                    playByPlay["id_playbyplaytype"] = (element.TryGetProperty("id_playbyplaytype", out JsonElement idPlayByPlayTypeElement) && idPlayByPlayTypeElement.ValueKind != JsonValueKind.Null)
                        ? idPlayByPlayTypeElement.GetInt32()
                        : "";


                    playByPlaysLean.Add(playByPlay);
                }
            }

            return playByPlaysLean;
        }

        /// <summary>
        /// Devuelve el listado reducido de los datos de la API de ACB. Con los datos de id_license, points, total_rebound y local 
        /// </summary>
        /// <param name="response">Respuesta de la llamada a la API de ACB hecha</param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetPlayersStatsFromResponse(string response)
        {
            var playersStats = new List<Dictionary<string, object>>();

            using (var doc = JsonDocument.Parse(response)) {
                foreach (var element in doc.RootElement.EnumerateArray()) {
                    var playerStats = new Dictionary<string, object>();

                    // Verificar id_license
                    if (element.TryGetProperty("id_license", out JsonElement idLicenseElement) && idLicenseElement.ValueKind != JsonValueKind.Null) {
                        playerStats["id_license"] = idLicenseElement.GetInt32();
                    }
                    else {
                        Console.WriteLine("Warning: Obviamos item debida a que 'id_license' es NULL");
                        continue;
                    }

                    if (element.TryGetProperty("statistics", out JsonElement statisticsElement) && statisticsElement.ValueKind != JsonValueKind.Null) {
                        playerStats["points"] = statisticsElement.GetProperty("points").GetInt32();
                        playerStats["total_rebound"] = statisticsElement.GetProperty("total_rebound").GetInt32();
                    }
                    else {
                        Console.WriteLine("Warning: Obviamos item debida a que 'statistics' es NULL");
                        continue;
                    }

                    playerStats["local"] = element.GetProperty("local").GetBoolean();

                    playersStats.Add(playerStats);
                }
            }

            return playersStats;
        }

        /// <summary>
        /// Devuelve úniocamente los campos score_local y score_visitor
        /// </summary>
        /// <param name="response">Respuesta de la llamada a la API de ACB hecha</param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetPlayByPlayFromResponse(string response)
        {
            var playByPlayData = new List<Dictionary<string, object>>();

            using (var doc = JsonDocument.Parse(response)) {
                foreach (var element in doc.RootElement.EnumerateArray()) {
                    var play = new Dictionary<string, object>();

                    play["score_local"] = (element.TryGetProperty("score_local", out JsonElement scoreLocalElement) && scoreLocalElement.ValueKind != JsonValueKind.Null)
                        ? scoreLocalElement.GetInt32()
                        : 0;

                    play["score_visitor"] = (element.TryGetProperty("score_visitor", out JsonElement scoreVisitorElement) && scoreVisitorElement.ValueKind != JsonValueKind.Null)
                        ? scoreVisitorElement.GetInt32()
                        : 0;

                    playByPlayData.Add(play);
                }
            }

            return playByPlayData;
        }
        #endregion
    }
}
