using GameModel.Cards;

namespace GameModel;

public class PlayCardMessage : BaseMessage
{
    public PlayCardMessage() { }

    public PlayCardMessage(CardEnum playedCard, params CardEnum[] args)
    {
        PlayedCard = playedCard;
        Args = args;
    }

    public CardEnum PlayedCard { get; set; }
}
