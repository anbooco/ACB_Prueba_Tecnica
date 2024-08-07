using ACB_Prueba_Tecnica.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ACB_Prueba_Tecnica.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "TokenAuthentication")]
    [Route("api/[controller]")]
    public class ACBController : ControllerBase
    {
        private const int _idPartido = 103789;
        private readonly IACBService _acbService;

        public ACBController(IACBService acbService)
        {
            _acbService = acbService;
        }

        /// <summary>
        /// Devuelve una versión reducida de la información del play by play para el partido especificado por su ID
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpGet("pbp-lean/{gameId}")]
        public async Task<IActionResult> GetPlayByPlayLean(int gameId)
        {
            try {
                var idPartido = gameId == 0 ? _idPartido : gameId;
                var playByPlays = await _acbService.GetPlayByPlayLean(idPartido);

                return Ok(playByPlays);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve los líderes de ambos equipos en puntos y rebotes para el partido especificado por su ID(game_id). Los datos están ordenados por puntos en orden descendente y luego por rebotes en orden descendente.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpGet("game-leaders/{gameId}")]
        public async Task<IActionResult> GetGameLeaders(int gameId)
        {
            try {
                var idPartido = gameId == 0 ? _idPartido : gameId;
                var gameLeaders = await _acbService.GetGameLeaders(idPartido);

                return Ok(gameLeaders);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve la mayor diferencia de puntos entre ambos equipos durante el partido especificado por su ID
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpGet("game-biggest_lead/{gameId}")]
        public async Task<IActionResult> GetBiggestLead(int gameId)
        {
            try {
                var idPartido = gameId == 0 ? _idPartido : gameId;
                var biggestLeaders = await _acbService.GetGameBiggestLeaders(idPartido);

                return Ok(biggestLeaders);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
