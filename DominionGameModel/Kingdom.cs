using GameModel.Cards;

namespace GameModel
{
    public class Kingdom
    {
        public Dictionary<CardEnum, Pile> Piles { get; set; } = new();
        public List<ICard> Trash { get; set; } = new();

        public Kingdom() { }

        public Kingdom(List<CardEnum> cards, int playerCount)
        {
            Piles.Add(CardEnum.Copper, new Pile(CardEnum.Copper, 60 - playerCount * 7));
            Piles.Add(CardEnum.Silver, new Pile(CardEnum.Silver, 40));
            Piles.Add(CardEnum.Gold, new Pile(CardEnum.Gold, 30));

            Piles.Add(CardEnum.Estate, new Pile(CardEnum.Estate, playerCount < 3 ? 8 : 12));
            Piles.Add(CardEnum.Duchy, new Pile(CardEnum.Duchy, playerCount < 3 ? 8 : 12));
            Piles.Add(CardEnum.Province, new Pile(CardEnum.Province, playerCount < 3 ? 8 : 12));

            Piles.Add(CardEnum.Curse, new Pile(CardEnum.Curse, (playerCount - 1) * 10));

            foreach (var card in cards)
            {
                Piles.Add(card, new Pile(card));
            }
        }

        public bool IsPileEmpty(CardEnum type)
        {
            return !Piles[type].Cards.Any();
        }

        public bool IsGameOver()
        {
            return Piles.Where(p => !p.Value.Cards.Any()).Count() >= 3
                || !Piles[CardEnum.Province].Cards.Any();
        }
    }
}
