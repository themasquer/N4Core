namespace N4Core.Views.Models
{
    public class TextAreaModel
    {
        public string Value { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public int MaxLength { get; private set; } = 100;
        public int Rows { get; private set; } = 2;
        public string PlaceHolder { get; private set; } = string.Empty;
        public int RemainingCharacterCount { get; private set; }

        public TextAreaModel(string value, string name, int maxLength)
        {
            Value = value ?? string.Empty;
            Name = name;
            MaxLength = maxLength;
            RemainingCharacterCount = Value.Length == 0 ? MaxLength : MaxLength - Value.Length;
        }

        public TextAreaModel(string value, string name, int maxLength, int rows) : this(value, name, maxLength)
        {
            Rows = rows;
        }

        public TextAreaModel(string value, string name, int maxLength, int rows, string placeHolder) : this(value, name, maxLength, rows)
        {
            PlaceHolder = placeHolder;
        }
    }
}
