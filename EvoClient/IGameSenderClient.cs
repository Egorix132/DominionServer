using Dominion.SocketIoServer.Dtos;
using GameModel;

namespace DominionClient;

internal interface IGameSenderClient
{
    public Task<GameStateDto?> PlayCardAsync(PlayCardMessage message);

    public void BuyCards(BuyMessage message);

}


