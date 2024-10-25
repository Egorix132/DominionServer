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
        public int VictoryPoints { get; set; } = 0;

        public List<CardEnum> AllCards { get; set; } = new();
        public List<CardEnum> OnPlay = new();
        public List<CardEnum> PublicDiscard = new();

        public PlayerStateDto()
        {

        }

        public PlayerStateDto(PlayerState playerState)
        {
            AllCards = playerState.AllCards.Select(c => c.CardTypeId).ToList();
            PublicDiscard = playerState.PublicDiscard.Select(c => c.CardTypeId).ToList();
            OnPlay = playerState.OnPlay.Select(c => c.CardTypeId).ToList();
            ActionsCount = playerState.ActionsCount;
            BuyCount = playerState.BuyCount;
            AdditionalMoney = playerState.AdditionalMoney;
            TotalMoney = playerState.TotalMoney;
            VictoryPoints = playerState.VictoryPoints;
        }
    }
}
