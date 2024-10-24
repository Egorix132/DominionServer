using GameModel.Cards;

namespace GameModel
{
    public class PlayCardMessage : BaseMessage
    {
        public CardEnum PlayedCard { get; set; }
    }
}
