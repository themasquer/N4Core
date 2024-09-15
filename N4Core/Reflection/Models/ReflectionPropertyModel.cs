namespace N4Core.Reflection.Models
{
    public class ReflectionPropertyModel
    {
        public string Name { get; } = null!;
        public string DisplayName { get; } = null!;

        public ReflectionPropertyModel(string name, string displayName = "")
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
