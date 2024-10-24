namespace GameModel
{
    public class GameDto
    {
        public GameDto(Game game, string playerId)
        {
            Id = game.Id;
            PlayerId = playerId;
            YourState = game.Players.FirstOrDefault(p => p.Id.ToString() == playerId).State;
            Players = game.Players.Where(p => p.Id.ToString() != playerId).Select(p =>p.State).ToList();
        }

        public string PlayerId { get; set; }

        public Guid Id { get; set; }

        public List<PlayerState> Players { get; set; }

        public PlayerState? YourState { get; set; }
    }
}
