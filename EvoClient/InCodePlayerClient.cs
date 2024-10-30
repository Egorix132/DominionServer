using Dominion.SocketIoServer.Dtos;
using DominionClient;
using GameModel;

namespace EvoClient;

internal class InCodePlayerClient : IPlayer, IGameSenderClient
{
    public string Id { get; set; }

    public string Name { get; set; }

    public PlayerState State { get; set; }


    public ISocketClientStrategy _strategy;

    public IGameState _game;

    public InCodePlayerClient(string id, string name, ISocketClientStrategy strategy)
    {
        Id = id;
        Name = name;
        State = new PlayerState();
        _strategy = strategy;
    }

    public void GameEnded(GameEndDto gameEndDto)
    {
        return;
    }

    public void SendException(Exception e)
    {
        return;
    }

    public async Task PlayTurnAsync(IGameState game)
    {
        _game = game;
        _strategy.PlayTurnAsync(game);
    }

    public async Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request)
    {
        return _strategy.ClarifyPlay(request);
    }

    public async Task<GameStateDto?> PlayCardAsync(PlayCardMessage message)
    {
        await State.PlayCard(_game, this, message);
        return null;
    }

    public void BuyCards(BuyMessage message)
    {
        State.BuyCards(_game, message, this);
    }
}


