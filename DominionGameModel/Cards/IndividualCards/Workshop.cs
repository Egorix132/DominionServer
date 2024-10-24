namespace GameModel.Cards.IndividualCards
{
    public class WorkshopCard : IActionCard
    {
        public string Name { get; set; } = "Workshop";

        public int Cost { get; set; } = 4;

        public string Text { get; set; } = "Gain a card costing up to $4.";

        public CardEnum CardTypeId { get; set; } = CardEnum.Workshop;

        public List<CardType> Types { get; set; } = new List<CardType> { CardType.Action };

        public bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            throw new NotImplementedException();
        }

        public bool TryAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            return true;
        }
    }
}
