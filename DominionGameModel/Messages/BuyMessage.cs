using GameModel.Cards;

namespace GameModel;

public class BuyMessage : BaseMessage
{
    public BuyMessage() { }

    public BuyMessage(params CardEnum[] buyCards)
    {
        Args = buyCards;
    }
}
