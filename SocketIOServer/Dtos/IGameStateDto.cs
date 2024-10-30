using GameModel.Infrastructure.Exceptions;

namespace Dominion.SocketIoServer.Dtos
{
    public interface IGameStateDto
    {
        public Guid GameId { get; set; }
        public ExceptionsEnum? ExceptionType { get; set; }

        public List<PlayerDto> Players { get; set; }
        public KingdomDto Kingdom { get; set; }

        public string PlayerId { get; set; }
        public int Turn { get; set; }
        public CurrentPlayerStateDto PlayerState { get; set; }
    }
}
