namespace GameModel.Cards
{
    public interface ICard
    {
        string Name { get; }
        int Cost { get; }
        string Text { get; }

        CardEnum CardTypeId { get; }

        List<CardType> Types { get; }
    }
}
