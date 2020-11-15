using System.Threading.Tasks;

namespace Dex.Uwp.Services
{
    public interface IAppLifecycleManager
    {
        Task<bool> IsFirstRun();

        Task InitializeAppForFirstRun();
    }

    public class AppLifecycleManager : IAppLifecycleManager
    {

        public AppLifecycleManager()
        {
        }
        public async Task<bool> IsFirstRun()
        {
            return false;
        }

        public async Task InitializeAppForFirstRun()
        {
            
        }

        //private async Task encryptFile()
        //{
        //    var localFolder = ApplicationData.Current.LocalFolder;
        //    await (await localFolder.GetFolderAsync("pks")).DeleteAsync();

        //    var appInstalledFolder = Package.Current.InstalledLocation;
        //    var zipFile = await appInstalledFolder.GetFileAsync(@"Assets\pks.zip");

        //    var zipBuffer = await FileIO.ReadBufferAsync(zipFile);
        //    var encryptedBuffer = _encryptionService.Encrypt(zipBuffer, "123456");

        //    var newfile = await localFolder.CreateFileAsync("pks.pks");

        //    await FileIO.WriteBufferAsync(newfile, encryptedBuffer);
        //}

        //private async Task decryptFile()
        //{
        //    var localFolder = ApplicationData.Current.LocalFolder;
        //    var appInstalledFolder = Package.Current.InstalledLocation;
        //    var encryptedZipFile = await appInstalledFolder.GetFileAsync(@"Assets\pks.pks");

        //    var encryptedZipBuffer = await FileIO.ReadBufferAsync(encryptedZipFile);
        //    var decryptedBuffer = _encryptionService.Decrypt(encryptedZipBuffer, "123456");

        //    var newfile = await localFolder.CreateFileAsync("pks.zip");
        //    await FileIO.WriteBufferAsync(newfile, decryptedBuffer);

        //    await Task.Run(() => ZipFile.ExtractToDirectory(newfile.Path, localFolder.Path));
        //    await newfile.DeleteAsync();
        //}
    }
}
