namespace N4Core.ArtificialIntelligence.Models.Bases
{
    public abstract class AiModelBase
    {
        public string Text { get; set; }

        protected AiModelBase(string text)
        {
            Text = text;
        }

        protected AiModelBase()
        {
            Text = string.Empty;
        }
    }
}
