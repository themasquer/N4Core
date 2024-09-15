using System.Globalization;

namespace N4Core.Culture.Utils.Bases
{
    public abstract class CultureUtilBase
    {
        protected List<CultureInfo> _cultures = new List<CultureInfo>()
        {
            new CultureInfo("en-US"),
            new CultureInfo("tr-TR")
        };

        public void Set(List<CultureInfo> cultures)
        {
            _cultures = cultures.ToList();
        }

        public virtual Languages GetLanguage()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            return currentCulture.Name == _cultures[0].Name ? Languages.English : Languages.Türkçe;
        }

        public virtual Languages GetLanguage(string value)
        {
            return string.IsNullOrWhiteSpace(value) || value == "0" ? Languages.English : Languages.Türkçe;
        }

        public virtual CultureInfo GetCulture(string language)
        {
            return string.IsNullOrWhiteSpace(language) || language == ((int)Languages.English).ToString() ? _cultures[0] : _cultures[1];
        }
    }
}
