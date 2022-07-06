using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Theme
{
    public interface IThemeManager
    {
        Task<bool> Install(ThemeInfo theme);
        Task<bool> Uninstall(ThemeInfo theme);
        Task<bool> SetDefault(ThemeInfo theme);
        Task<IEnumerable<ThemeInfo>> GetThemes(string query, int page, int limit);
        Task<ThemeStatus> UnzipAndInstall(IFormFile modelThemeZip);
    }
}