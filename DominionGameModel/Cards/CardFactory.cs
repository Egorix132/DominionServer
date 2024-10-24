using GameModel.Cards.IndividualCards;

namespace GameModel.Cards
{
    internal static class CardFactory
    {
        public static ICard CreateCard(CardEnum cardType)
        {
            return cardType switch
            {
                CardEnum.Copper => new CopperCard(),
                CardEnum.Silver => new SilverCard(),
                CardEnum.Gold => new GoldCard(),
                CardEnum.Estate => new EstateCard(),
                CardEnum.Duchy => new DuchyCard(),
                CardEnum.Province => new ProvinceCard(),
                CardEnum.Curse => new CurseCard(),
                CardEnum.Artisan => new ArtisanCard(),
                CardEnum.Cellar => new CellarCard(),
                CardEnum.Market => new MarketCard(),
                CardEnum.Merchant => new MerchantCard(),
                CardEnum.Mine => new MineCard(),
                CardEnum.Moat => new MoatCard(),
                CardEnum.Moneylender => new MoneylenderCard(),
                CardEnum.Poacher => new PoacherCard(),
                CardEnum.Remodel => new RemodelCard(),
                CardEnum.Witch => new WitchCard(),
                _ => throw new NotImplementedException()
            };;
        }
    }
}
