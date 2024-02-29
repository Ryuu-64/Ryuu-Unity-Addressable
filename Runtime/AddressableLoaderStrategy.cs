namespace Ryuu.Unity.Addressable
{
    public class AddressableLoaderStrategy
    {
        public string Key { get; }
        public AssetStrategy AssetStrategy { get; }
        public LoadStrategy LoadStrategy { get; set; }

        public AddressableLoaderStrategy(string key, AssetStrategy assetStrategy, LoadStrategy loadStrategy)
        {
            Key = key;
            AssetStrategy = assetStrategy;
            LoadStrategy = loadStrategy;
        }
    }
}