using GameModel.Cards;

namespace GameModel;

public class ClarificationResponseMessage
{
    public CardEnum[] Args { get; set; } = Array.Empty<CardEnum>();
    public CardEnum[] SecondArgs { get; set; } = Array.Empty<CardEnum>();
    public CardEnum[] ThirdArgs { get; set; } = Array.Empty<CardEnum>();

    public ClarificationResponseMessage() { }

    public ClarificationResponseMessage(params CardEnum[] buyCards)
    {
        Args = buyCards;
    }
}
