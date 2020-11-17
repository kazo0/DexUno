using Dex.Uwp.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Dex.Uwp.DataAccess
{

    public interface IPokePicturesSource
    {
        string GetPath(uint dexNumber);

        string Name { get; }
    }

    public interface INeedInitialisation
    {
        Task Initialize();

        Task<bool> IsInitialized();
    }

    public class DefaultPokePicturesSource : IPokePicturesSource
    {
        //TODO: Localize this
        public string Name => "Default";

        private const string pathTemplate = @"ms-appx:///Assets/Flat/{0}.png";
		private readonly ILogger _log;

		public DefaultPokePicturesSource(ILogger log)
		{
			_log = log;
		}

        public string GetPath(uint dexNumber)
        {
            _log.LogDebug($"Getting image for path: {string.Format(pathTemplate, dexNumber.ToString())}");
            return string.Format(pathTemplate, dexNumber.ToString());
        }
    }
}