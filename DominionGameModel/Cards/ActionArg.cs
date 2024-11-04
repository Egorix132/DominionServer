namespace GameModel.Cards
{
    public struct ActionArg
    {
        public ActionArgSourceType Source = ActionArgSourceType.Any;

        public bool IsOptional = false;

        public ActionArg(ActionArgSourceType sourceType = ActionArgSourceType.Any, bool isOptional = false)
        {
            Source = sourceType;
            IsOptional = isOptional;
        }

        public ActionArg(bool isOptional)
        {
            IsOptional = isOptional;
        }
    }
}
