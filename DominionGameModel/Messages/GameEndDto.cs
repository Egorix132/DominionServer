namespace GameModel;

public class GameEndDto
{
    public int Turn { get; set; }

    public GameEndType GameEndType { get; set; }

    public string WinnerName { get; set; }

    public int WinnerVP { get; set; }

    public List<PlayerVictoryDto> Players { get; set; }
}

