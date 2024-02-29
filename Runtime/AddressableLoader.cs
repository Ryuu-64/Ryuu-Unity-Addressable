using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Ryuu.Unity.Addressable
{
    public class AddressableLoader<T> where T : UnityEngine.Object
    {
        // ReSharper disable once CollectionNeverQueried.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public Dictionary<string, T> Assets { get; } = new();

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Task LoadTask { get; private set; }

        // ReSharper disable once EventNeverSubscribedTo.Global
        public event Action AfterLoad;

        private AddressableLoader()
        {
        }

        public static AddressableLoader<T> Load(string key, AssetStrategy assetStrategy, LoadStrategy loadStrategy)
        {
            return assetStrategy switch
            {
                AssetStrategy.Single => LoadAsset(key, loadStrategy),
                AssetStrategy.Multiple => LoadAssets(key, loadStrategy),
                _ => throw new ArgumentOutOfRangeException(nameof(assetStrategy), assetStrategy, null)
            };
        }

        private static AddressableLoader<T> LoadAsset(string key, LoadStrategy strategy)
        {
            var assetLoader = new AddressableLoader<T>();
            var loadAssetHandle = Addressables.LoadAssetAsync<T>(key);
            assetLoader.LoadTask = loadAssetHandle.Task;
            switch (strategy)
            {
                case LoadStrategy.Sync:
                    T asset = loadAssetHandle.WaitForCompletion();
                    assetLoader.Assets.Add(key, asset);
                    break;
                case LoadStrategy.Async:
                    loadAssetHandle.Completed +=
                        handle =>
                        {
                            assetLoader.Assets.Add(key, handle.Result);
                            assetLoader.AfterLoad?.Invoke();
                        };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }

            return assetLoader;
        }

        private static AddressableLoader<T> LoadAssets(string key, LoadStrategy strategy)
        {
            var assetLoader = new AddressableLoader<T>();
            var loadAssetsHandle = Addressables.LoadAssetsAsync<T>(
                key,
                asset => assetLoader.Assets.Add(asset.name, asset)
            );
            loadAssetsHandle.Completed += _ => assetLoader.AfterLoad?.Invoke();
            assetLoader.LoadTask = loadAssetsHandle.Task;

            if (strategy == LoadStrategy.Sync)
            {
                loadAssetsHandle.WaitForCompletion();
            }

            return assetLoader;
        }
    }
}