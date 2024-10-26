using EvoClient.Utils;
using GameModel.Cards;
using GameModel.Cards.IndividualCards;
using GameModel.Infrastructure;
using GameModel.Infrastructure.Attributes;
using GameModel.Infrastructure.Exceptions;
using System.Linq;
using System.Text.Json.Serialization;

namespace GameModel
{
    public class PlayerState
    {
        public int ActionsCount { get; set; } = 1;
        public int BuyCount { get; set; } = 1;
        public int AdditionalMoney { get; set; } = 0;
        public int TotalMoney => Hand.OfType<ITreasureCard>().Sum(c => c.Money) + AdditionalMoney
            + (Hand.Any(c => c.CardTypeId == CardEnum.Silver) ? OnPlay.Where(c => c.CardTypeId == CardEnum.Merchant).Count() : 0);
        public int VictoryPoints => AllCards.OfType<IVictoryCard>().Sum(c => c.GetVictoryPoints(this));

        public List<ICard> AllCards { get; set; } = new();

        [JsonIgnore]
        public List<ICard> Deck = new();
        public List<ICard> Hand = new();
        public List<ICard> OnPlay = new();

        [JsonIgnore]
        private List<ICard> _privateDiscard = new();

        public List<ICard> PublicDiscard = new();

        [JsonIgnore]
        public List<ICard> Discard => _privateDiscard.Concat(PublicDiscard).ToList();

        public PlayerState()
        {
            SetDefaultState();
        }

        public void SetDefaultState()
        {
            ActionsCount = 1;
            BuyCount = 1;
            AdditionalMoney = 0;

            AllCards.Clear();
            _privateDiscard.Clear();
            PublicDiscard.Clear();
            OnPlay.Clear();
            Hand.Clear();

            Deck = new List<ICard>()
            {
                new CopperCard(), new CopperCard(), new CopperCard(), new CopperCard(), new CopperCard(),
                new CopperCard(), new CopperCard(), new EstateCard(), new EstateCard(), new EstateCard(),
            };
            AllCards.AddRange(Deck);

            Deck.Shuffle();

            DrawToHand();
        }

        public List<ICard> Draw(int size = 5)
        {
            var drawedCards = new List<ICard>();
            for (int i = 0; i < size; i++)
            {
                if (Deck.Count == 0)
                {
                    if (Discard.Count == 0)
                    {
                        break;
                    }
                    Deck.AddRange(Discard);
                    Deck.Shuffle();
                    _privateDiscard.Clear();
                    PublicDiscard.Clear();
                }
                var card = Deck.Pop();
                drawedCards.Add(card!);
            }

            return drawedCards;
        }

        public List<ICard> DrawToHand(int size = 5)
        {
            var drawedCards = Draw(size);
            Hand.AddRange(drawedCards);

            return drawedCards;
        }

        public async Task PlayCard(Game game, IPlayer player, PlayCardMessage playCardMessage)
        {
            if (game.CurrentPlayer.Id != player.Id)
            {
                throw new BaseDominionException(ExceptionsEnum.NotYourTurn);
            }

            if (ActionsCount == 0)
            {
                throw new BaseDominionException(ExceptionsEnum.DontHaveActions);
            }

            var cardInHand = Hand.FirstOrDefault(c => c.CardTypeId == playCardMessage.PlayedCard);
            if (cardInHand == null || cardInHand is not IActionCard actionCard)
            {
                throw new MissingCardsException(playCardMessage.PlayedCard);
            }

            OnPlay.Add(cardInHand);
            Hand.Remove(cardInHand);

            try
            {
                var isPlayed = await actionCard.TryAct(game, player, playCardMessage);
                ActionsCount--;
            }
            catch (Exception)
            {
                OnPlay.Remove(cardInHand);
                Hand.Add(cardInHand);
                throw;
            }
        }

        public bool CanPlayCard(Game game, PlayCardMessage playCardMessage, IPlayer player)
        {
            if (game.CurrentPlayer.Id != player.Id)
            {
                throw new BaseDominionException(ExceptionsEnum.NotYourTurn);
            }

            if (ActionsCount == 0)
            {
                throw new BaseDominionException(ExceptionsEnum.DontHaveActions);
            }

            var cardInHand = Hand.FirstOrDefault(c => c.CardTypeId == playCardMessage.PlayedCard);
            if (cardInHand == null || cardInHand is not IActionCard actionCard)
            {
                throw new MissingCardsException(playCardMessage.PlayedCard);
            }

            OnPlay.Add(cardInHand);
            Hand.Remove(cardInHand);

            try
            {
                return actionCard.CanAct(game, player, playCardMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                OnPlay.Remove(cardInHand);
                Hand.Add(cardInHand);
            }
        }

        public void AddCardsOntoDeck(params ICard[] cards)
        {
            AllCards.AddRange(cards);

            foreach (var card in cards)
            {
                Deck = Deck.Prepend(card).ToList();
            }
        }

        public void AddCardsToDiscard(params ICard[] cards)
        {
            AddCardsToDiscard(cards as IEnumerable<ICard>);
        }

        public void AddCardsToDiscard(IEnumerable<ICard> cards)
        {
            AllCards.AddRange(cards);
            PublicDiscard.AddRange(cards);
        }

        public void MoveCardsToDiscard(params ICard[] cards)
        {
            MoveCardsToDiscard(cards as IEnumerable<ICard>);
        }

        public void MoveCardsToDiscard(IEnumerable<ICard> cards)
        {
            PublicDiscard.AddRange(cards);
        }

        public void AddCardsToHand(params ICard[] cards)
        {
            AllCards.AddRange(cards);
            Hand.AddRange(cards);
        }

        public void BuyCards(Game game, BuyMessage buyMessage, IPlayer player)
        {
            if (game.CurrentPlayer.Id != player.Id)
            {
                throw new BaseDominionException(ExceptionsEnum.NotYourTurn);
            }

            if (BuyCount < buyMessage.Args.Length)
            {
                throw new BaseDominionException(ExceptionsEnum.DontHaveBuy);
            }

            var emptyPiles = buyMessage.Args
                .GroupBy(t => t)
                .Where(g => game.Kingdom.Piles[g.Key].Cards.Count < g.Count());

            if (emptyPiles.Any())
            {
                throw new PileIsEmptyException(emptyPiles
                    .Select(g => g.FirstOrDefault())
                    .ToArray());
            }
            var requiredMoney = buyMessage.Args.Sum(t => CardEnumDict.GetCard(t).Cost);
            var treasureCards = new List<ITreasureCard>(Hand.OfType<ITreasureCard>());
            
            if (requiredMoney > TotalMoney)
            {
                throw new NotEnoughMoneyException(requiredMoney, TotalMoney);
            }

            requiredMoney -= AdditionalMoney;
            foreach (var item in treasureCards)
            {
                requiredMoney -= item.Money;
                DiscardFromHand(DiscardType.AllToPublic, item.CardTypeId);

                if (requiredMoney <= 0)
                {
                    break;
                }
            }

            foreach (var cardType in buyMessage.Args)
            {
                var card = game.Kingdom.Piles[cardType].Pop();
                AllCards.Add(card);
                PublicDiscard.Add(card);
            }
        }

        public bool DiscardFromHand(DiscardType discardType, IEnumerable<CardEnum> cards)
        {
            if (!HaveInHand(cards))
            {
                return false;
            }

            for (int i = 0; i < cards.Count(); i++)
            {
                var cardType = cards.ElementAt(i);
                var discardedCard = Hand.FirstOrDefault(c => c.CardTypeId == cardType);

                if (discardType == DiscardType.AllToPublic
                    || discardType == DiscardType.LastToPublic && i == cards.Count() - 1)
                {
                    PublicDiscard.Add(discardedCard!);
                }
                else if (discardType == DiscardType.AllToPrivate)
                {
                    _privateDiscard.Add(discardedCard!);
                }

                Hand.Remove(discardedCard!);
            }
            return true;
        }

        public bool DiscardFromHand(DiscardType discardType, params CardEnum[] cards)
        {
            return DiscardFromHand(discardType, cards as IEnumerable<CardEnum>);
        }

        public static bool MoveFromTo(ICollection<ICard> from, ICollection<ICard> to, params CardEnum[] cards)
        {
            if (cards.GroupBy(t => t).Any(group => from.Where(c => c.CardTypeId == group.FirstOrDefault()).Count() < group.Count()))
            {
                return false;
            }

            for (int i = 0; i < cards.Length; i++)
            {
                var cardType = cards[i];
                var discardedCard = from.FirstOrDefault(c => c.CardTypeId == cardType);

                to.Add(discardedCard!);
                from.Remove(discardedCard!);
            }
            return true;
        }

        public bool TrashFromHand(Kingdom kingdom, IEnumerable<CardEnum> cards)
        {
            if (cards.GroupBy(t => t).Any(group => Hand.Where(c => c.CardTypeId == group.FirstOrDefault()).Count() < group.Count()))
            {
                return false;
            }

            for (int i = 0; i < cards.Count(); i++)
            {
                var cardType = cards.ElementAt(i);
                var discardedCard = Hand.FirstOrDefault(c => c.CardTypeId == cardType);

                kingdom.Trash.Add(discardedCard!);
                Hand.Remove(discardedCard!);
                AllCards.Remove(discardedCard!);
            }
            return true;
        }


        public bool TrashFromHand(Kingdom kingdom, params CardEnum[] cards)
        {
            return TrashFromHand(kingdom, cards as IEnumerable<CardEnum>);
        }
        public bool OnDeckFromHand(CardEnum? type)
        {
            var discardedCard = Hand.FirstOrDefault(c => c.CardTypeId == type);

            if (discardedCard == null)
            {
                return false;
            }

            Deck = Deck.Prepend(discardedCard).ToList();
            Hand.Remove(discardedCard);

            return true;
        }

        public bool MoveOnDeck(ICard? card)
        {
            if(card == null)
            {
                return false;
            }
            Deck = Deck.Prepend(card).ToList();

            return true;
        }

        public bool HaveInHand(params CardEnum[] cards)
        {
            if (cards.GroupBy(t => t)
                .Any(group => Hand.Where(c => c.CardTypeId == group.Key).Count() < group.Count()))
            {
                return false;
            }

            return true;
        }

        public bool HaveInHand(IEnumerable<CardEnum> cards)
        {
            if (cards.GroupBy(t => t)
                .Any(group => Hand.Where(c => c.CardTypeId == group.FirstOrDefault()).Count() < group.Count()))
            {
                return false;
            }

            return true;
        }

        public void EndTurn()
        {
            DiscardFromHand(DiscardType.AllToPrivate, Hand.Select(c => c.CardTypeId).ToArray());

            _privateDiscard.AddRange(OnPlay);
            OnPlay.Clear();

            AdditionalMoney = 0;
            ActionsCount = 1;
            BuyCount = 1;

            DrawToHand();
        }

        public void RemoveFromDiscard(ICard card)
        {
            if(PublicDiscard.Contains(card))
            {
                PublicDiscard.Remove(card);
            }
            else
            {
                _privateDiscard.Remove(card);
            }
        }
    }
}
