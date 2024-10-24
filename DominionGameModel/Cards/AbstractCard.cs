namespace GameModel.Cards
{
    public abstract class AbstractCard : ICard
    {
        public abstract string Name { get; }

        public abstract int Cost { get; }

        public abstract string Text { get; }

        public abstract CardEnum CardTypeId { get; }

        public abstract List<CardType> Types { get; }
    }
}
