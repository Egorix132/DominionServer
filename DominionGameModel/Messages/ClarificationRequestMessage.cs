using GameModel.Cards;

namespace GameModel;

public class ClarificationRequestMessage : BaseMessage
{
    public CardEnum PlayedCard { get; set; }
}
