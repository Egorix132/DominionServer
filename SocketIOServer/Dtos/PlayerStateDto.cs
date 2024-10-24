using GameModel;
using GameModel.Cards;

namespace Dominion.SocketIoServer.Dtos
{
    public class PlayerStateDto
    {
        public int ActionsCount { get; set; } = 1;
        public int BuyCount { get; set; } = 1;
        public int AdditionalMoney { get; set; } = 0;
        public int TotalMoney { get; set; } = 0;

        public List<AbstractCard> AllCards { get; set; } = new();
        public List<AbstractCard> Hand = new();
        public List<AbstractCard> OnPlay = new();
        public List<AbstractCard> PublicDiscard = new();

        public PlayerStateDto()
        {

        }

        public PlayerStateDto(PlayerState playerState)
        {
            AllCards = new List<AbstractCard>(playerState.AllCards.Cast<AbstractCard>());
            PublicDiscard = new List<AbstractCard>(playerState.PublicDiscard.Cast<AbstractCard>());
            OnPlay = new List<AbstractCard>(playerState.OnPlay.Cast<AbstractCard>());
            ActionsCount = playerState.ActionsCount;
            BuyCount = playerState.BuyCount;
            AdditionalMoney = playerState.AdditionalMoney;
            TotalMoney = playerState.TotalMoney;
        }
    }
}
