using GameModel.Cards;

namespace GameModel;

public class BaseMessage
{
    public IList<CardEnum> Args { get; set; } = Array.Empty<CardEnum>();
}

