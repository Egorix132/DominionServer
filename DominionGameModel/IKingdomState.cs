using GameModel.Cards;

namespace GameModel
{
    public interface IKingdomState
    {
        public Dictionary<CardEnum, Pile> Piles { get; }
        public List<ICard> Trash { get; }

        public bool IsPileEmpty(CardEnum type);

        public GameEndType? IsGameOver();

        public void ToTrash(ICard card);
    }
}
