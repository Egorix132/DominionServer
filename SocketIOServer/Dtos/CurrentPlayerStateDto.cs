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
        public int VictoryPoints { get; set; } = 0;

        public List<CardEnum> AllCards;
        public List<CardEnum> Hand;
        public List<CardEnum> Deck;
        public List<CardEnum> OnPlay;
        public List<CardEnum> Discard;

        public CurrentPlayerStateDto()
        {

        }

        public CurrentPlayerStateDto(PlayerState playerState)
        {
            AllCards = playerState.AllCards.Select(c => c.CardTypeId).ToList();
            Discard = playerState.Discard.Select(c => c.CardTypeId).ToList();
            OnPlay = playerState.OnPlay.Select(c => c.CardTypeId).ToList();
            Hand = playerState.Hand.Select(c => c.CardTypeId).ToList();
            Deck = playerState.Deck.Select(c => c.CardTypeId).ToList();

            Deck.Shuffle();

            ActionsCount = playerState.ActionsCount;
            BuyCount = playerState.BuyCount;
            AdditionalMoney = playerState.AdditionalMoney;
            TotalMoney = playerState.TotalMoney;
            VictoryPoints = playerState.VictoryPoints;
        }
    }
}
