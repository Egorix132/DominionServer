using GameModel.Cards;

namespace GameModel;

public class LogEntry
{
    public int Turn { get; set; }

    public string PlayerName { get; set; }

    public CardEnum? PlayedCard { get; set; }

    public MessageType MessageType { get; set; }

    public IList<CardEnum> Args { get; set; } = Array.Empty<CardEnum>();
}

