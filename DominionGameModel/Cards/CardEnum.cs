using GameModel.Infrastructure.Attributes;

namespace GameModel.Cards
{
    public enum CardEnum
    {

        [CardCost(0), CardTypes(CardType.Treasure)] Copper = 0,
        [CardCost(3), CardTypes(CardType.Treasure)] Silver = 1,
        [CardCost(6), CardTypes(CardType.Treasure)] Gold = 2,
        [CardCost(2), CardTypes(CardType.Victory)] Estate = 3,
        [CardCost(5), CardTypes(CardType.Victory)] Duchy = 4,
        [CardCost(8), CardTypes(CardType.Victory)] Province = 5,
        [CardCost(0), CardTypes(CardType.Curse)] Curse = 6,

        [CardCost(6), CardTypes(CardType.Action)] Artisan = 7,
        [CardCost(5), CardTypes(CardType.Action, CardType.Attack)] Bandit = 8,
        [CardCost(4), CardTypes(CardType.Action, CardType.Attack)] Bureaucrat = 9,
        [CardCost(2), CardTypes(CardType.Action)] Cellar = 10,
        [CardCost(2), CardTypes(CardType.Action)] Chapel = 11,
        [CardCost(5), CardTypes(CardType.Action)] CouncilRoom = 12,
        [CardCost(5), CardTypes(CardType.Action)] Festival = 13,
        [CardCost(4), CardTypes(CardType.Action, CardType.Victory)] Gardens = 14,
        [CardCost(3), CardTypes(CardType.Action)] Harbinger = 15,
        [CardCost(5), CardTypes(CardType.Action)] Laboratory = 16,
        [CardCost(5), CardTypes(CardType.Action)] Library = 17,
        [CardCost(5), CardTypes(CardType.Action)] Market = 18,
        [CardCost(3), CardTypes(CardType.Action)] Merchant = 19,
        [CardCost(4), CardTypes(CardType.Action, CardType.Attack)] Militia = 20,
        [CardCost(5), CardTypes(CardType.Action)] Mine = 21,
        [CardCost(2), CardTypes(CardType.Action, CardType.Reaction)] Moat = 22,
        [CardCost(4), CardTypes(CardType.Action)] Moneylender = 23,
        [CardCost(4), CardTypes(CardType.Action)] Poacher = 24,
        [CardCost(4), CardTypes(CardType.Action)] Remodel = 25,
        [CardCost(5), CardTypes(CardType.Action)] Sentry = 26,
        [CardCost(4), CardTypes(CardType.Action)] Smithy = 27,
        [CardCost(4), CardTypes(CardType.Action)] ThroneRoom = 28,
        [CardCost(3), CardTypes(CardType.Action)] Vassal = 29,
        [CardCost(3), CardTypes(CardType.Action)] Village = 30,
        [CardCost(5), CardTypes(CardType.Action, CardType.Attack)] Witch = 31,
        [CardCost(3), CardTypes(CardType.Action)] Workshop = 32,
    }
}
