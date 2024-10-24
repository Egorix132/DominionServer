using DominionServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameModel;

namespace DominionServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private IGameService _gameService;

        public SessionController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("StartVs1Bot")]
        public async Task<GameDto> StartGameVs1Bot(string playerName)
        {
            var game = _gameService.StartGame(playerName);

            var playerId = game.Players.FirstOrDefault(p => p.Name == playerName)!.Id;
            var obfuscatedGame = new GameDto(game, playerId.ToString());

            await Authenticate(obfuscatedGame.PlayerId.ToString());

            return obfuscatedGame;
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}