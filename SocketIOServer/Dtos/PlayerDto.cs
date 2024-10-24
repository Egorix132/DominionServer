using GameModel;

namespace Dominion.SocketIoServer.Dtos
{
    public class PlayerDto
    {
        public string Name { get; set; }

        public PlayerStateDto PublicState { get; set; }

        public PlayerDto() { }

        public PlayerDto(IPlayer player)
        {
            Name = player.Name;
            PublicState = new PlayerStateDto(player.State);
        }
    }
}
