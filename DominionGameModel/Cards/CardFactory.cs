using GameModel.Cards.IndividualCards;

namespace GameModel.Cards
{
    public static class CardFactory
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
                CardEnum.Bandit => new BanditCard(),
                CardEnum.Bureaucrat => new BureaucratCard(),
                CardEnum.Cellar => new CellarCard(),
                CardEnum.Chapel => new ChapelCard(),
                CardEnum.CouncilRoom => new CouncilRoomCard(),
                CardEnum.Festival => new FestivalCard(),
                CardEnum.Gardens => new GardensCard(),
                CardEnum.Harbinger => new HarbingerCard(),
                CardEnum.Laboratory => new LaboratoryCard(),
                CardEnum.Library => new LibraryCard(),
                CardEnum.Market => new MarketCard(),
                CardEnum.Merchant => new MerchantCard(),
                CardEnum.Militia => new MilitiaCard(),
                CardEnum.Mine => new MineCard(),
                CardEnum.Moat => new MoatCard(),
                CardEnum.Moneylender => new MoneylenderCard(),
                CardEnum.Poacher => new PoacherCard(),
                CardEnum.Remodel => new RemodelCard(),
                CardEnum.Sentry => new SentryCard(),
                CardEnum.Smithy => new SmithyCard(),
                CardEnum.ThroneRoom => new ThroneRoomCard(),
                CardEnum.Vassal => new VassalCard(),
                CardEnum.Village => new VillageCard(),
                CardEnum.Witch => new WitchCard(),
                CardEnum.Workshop => new WorkshopCard(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
