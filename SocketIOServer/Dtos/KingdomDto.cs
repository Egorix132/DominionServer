using GameModel;
using GameModel.Cards;

namespace Dominion.SocketIoServer.Dtos
{
    public class KingdomDto
    {
        public Dictionary<CardEnum, Pile> Piles { get; set; } = new();
        public List<CardEnum> Trash { get; set; } = new();

        public KingdomDto() { }

        public KingdomDto(Kingdom kingdom)
        {
            Piles = kingdom.Piles;
            Trash = kingdom.Trash.Select(c => c.CardTypeId).ToList();
        }
    }
}
