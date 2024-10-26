﻿using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class ChapelCard : AbstractActionCard
{
    public override string Name { get; } = "Chapel";

    public override int Cost { get; } = 2;

    public override string Text { get; } = "Trash up to 4 cards from your hand.";

    public override CardEnum CardTypeId { get; } = CardEnum.Chapel;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var firstFour = playMessage.Args.Take(4);
        player.State.TrashFromHand(game.Kingdom, firstFour);
    }

    public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var firstFour = playMessage.Args.Take(4);
        if (!player.State.HaveInHand(firstFour))
        {
            throw new MissingCardsException(firstFour);
        }
        return true;
    }
}