using Dominion.SocketIoServer.Dtos;
using GameModel;
using Newtonsoft.Json.Linq;

namespace DominionClient
{
    internal interface ISocketClientStrategy
    {
        public Task PlayTurnAsync(IGameState game);

        public ClarificationResponseMessage ClarifyPlay(ClarificationRequestMessage message);

        public void HandleException(string exceptionMessage);

        public void HandleDisconnect(JToken[] data);
    }
}