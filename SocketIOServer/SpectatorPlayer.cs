using Dominion.SocketIoServer.Dtos;
using GameModel;

namespace Dominion.SocketIoServer
{
    public class SpectatorPlayer : IPlayer
    {
        private SocketIoClient _socket;

        public SpectatorPlayer(SocketIoClient socket, string name)
        {
            _socket = socket;
            Name = name;
            State = new PlayerState();
        }

        public string Id => _socket.Id.ToString();

        public string Name { get; init; }

        public PlayerState State { get; init; }

        public async Task PlayTurnAsync(IGameState game)
        {
            try
            {
                _socket.SendMessage("playTurn", new GameStateDto(game));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request)
        {
            ClarificationResponseMessage playClarificationResponse = null;
            try
            {
                var response = await _socket.AskAsync(
                    $"clarificatePlay",
                    request);

                if (response.Length < 1 || !response[0].TryDeserializeObject(out playClarificationResponse))
                {
                    throw new ArgumentException("BadRequest");
                }
            }
            catch
            {
                throw;
            }

            return playClarificationResponse;
        }

        public void GameEnded(GameEndDto gameEndDto)
        {
            _socket.SendMessage("endGame", gameEndDto);
        }

        public void SendException(Exception e)
        {
            _socket.SendMessage("exception", e.Message);
        }
    }
}
