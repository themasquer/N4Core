namespace N4Core.Views.Models
{
    public class TextBoxModel
    {
        public string Value { get; } = string.Empty;
        public string Name { get; } = string.Empty;
        public string PlaceHolder { get; } = string.Empty;

        public TextBoxModel(string name)
        {
            Name = name;
        }

        public TextBoxModel(string name, string placeHolder) : this(name)
        {
            PlaceHolder = placeHolder;
        }

        public TextBoxModel(string name, string placeHolder, string value) : this(name, placeHolder)
        {
            Value = value;
        }
    }
}
