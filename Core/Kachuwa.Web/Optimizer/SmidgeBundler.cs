using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Html;
using Smidge;
using Smidge.Models;

namespace Kachuwa.Web.Optimizer
{
    public class SmidgeBundler : IKachuwaBundler
    {
        private readonly SmidgeHelper _smidgeHelper;
        private readonly IBundleManager _bundleManager;

        public SmidgeBundler(SmidgeHelper smidgeHelper, IBundleManager bundleManager)
        {
            _smidgeHelper = smidgeHelper;
            _bundleManager = bundleManager;
        }
        public async Task<HtmlString> BundleCss(string[] files)
        {
            string bundleName = Guid.NewGuid().ToString();
            var cssFiles = files.Select(x => new CssFile
            {
                FilePath = x
            }).ToArray();
            var bundle = _bundleManager.Create(bundleName, cssFiles);
            var urls = await _smidgeHelper.GenerateCssUrlsAsync(bundleName);
            var result = new StringBuilder();

            foreach (var url in urls)
            {
                result.AppendFormat("<link href='{0}' rel='stylesheet' type='text/css'/>", url);
            }
            return new HtmlString(result.ToString());
        }

        public async Task<HtmlString> BundleCss(string name, string[] files)
        {
            // string bundleName = Guid.NewGuid().ToString();
            var cssFiles = files.Select(x => new CssFile
            {
                FilePath = x
            }).ToArray();
            if (!_bundleManager.Exists(name))
                _bundleManager.Create(name, cssFiles);
            var urls = await _smidgeHelper.GenerateCssUrlsAsync(name);
            var result = new StringBuilder();

            foreach (var url in urls)
            {
                result.AppendFormat("<link href='{0}' rel='stylesheet' type='text/css'/>", url);
            }
            return new HtmlString(result.ToString());
        }

        public async Task<HtmlString> BundleJs(string[] files)
        {

            string bundleName = Guid.NewGuid().ToString();
            var cssFiles = files.Select(x => new JavaScriptFile()
            {
                FilePath = x
            }).ToArray();
            var bundle = _bundleManager.Create(bundleName, cssFiles);
            var urls = await _smidgeHelper.GenerateJsUrlsAsync(bundleName);
            var result = new StringBuilder();

            foreach (var url in urls)
            {
                result.AppendFormat("<script asp-add-nonce='true' src='{0}' type='text/javascript'></script>", url);
            }
            return new HtmlString(result.ToString());
        }

        public async Task<HtmlString> BundleJs(string name, string[] files)
        {

            var cssFiles = files.Select(x => new JavaScriptFile()
            {
                FilePath = x
            }).ToArray();
            if (!_bundleManager.Exists(name))
                _bundleManager.Create(name, cssFiles);
            var urls = await _smidgeHelper.GenerateJsUrlsAsync(name);
            var result = new StringBuilder();

            foreach (var url in urls)
            {
                result.AppendFormat("<script asp-add-nonce='true' src='{0}' type='text/javascript'></script>", url);
            }
            return new HtmlString(result.ToString());
        }

    }
}