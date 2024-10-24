using GameModel.Cards;

namespace GameModel
{
    public class BaseMessage
    {
        public CardEnum[] Args { get; set; } = Array.Empty<CardEnum>();
    }
}
