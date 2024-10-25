using GameModel;
using GameModel.Cards;
using GameModel.Infrastructure;

namespace Dominion.SocketIoServer.Dtos
{
    public class CurrentPlayerStateDto
    {
        public int ActionsCount { get; set; } = 1;
        public int BuyCount { get; set; } = 1;
        public int AdditionalMoney { get; set; } = 0;
        public int TotalMoney { get; set; } = 0;

        public List<ICard> AllCards;
        public List<ICard> Hand;
        public List<ICard> Deck;
        public List<ICard> OnPlay;
        public List<ICard> Discard;

        public CurrentPlayerStateDto()
        {

        }

        public CurrentPlayerStateDto(PlayerState playerState)
        {
            AllCards = new List<ICard>(playerState.AllCards);
            Discard = new List<ICard>(playerState.Discard);
            OnPlay = new List<ICard>(playerState.OnPlay);
            Hand = new List<ICard>(playerState.Hand);
            Deck = new List<ICard>(playerState.Deck);
            Deck.Shuffle();

            ActionsCount = playerState.ActionsCount;
            BuyCount = playerState.BuyCount;
            AdditionalMoney = playerState.AdditionalMoney;
            TotalMoney = playerState.TotalMoney;
        }
    }
}
