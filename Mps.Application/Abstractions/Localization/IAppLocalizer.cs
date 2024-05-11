using Microsoft.Extensions.Localization;

namespace Mps.Application.Abstractions.Localization
{
    public interface IAppLocalizer
    {
        string this[string key] { get; }
        string this[string key, params object[] arguments] { get; }
        IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);
        LocalizedString GetString(string name);
        LocalizedString GetString(string name, params object[] arguments);
    }
}
