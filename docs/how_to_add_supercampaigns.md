# How to add Supercampaigns prefabs as Addressables

Once you have created a new Supercampaigns prefab, you need to add it as an Addressable so that it can be loaded at runtime. Here's how you can do that:

0. Make sure you have the Addressables package installed. If you don't, you can install it from the Package Manager.
1. Create a new prefab in the `Assets/Resources/Supercampaigns` folder. You will find a template prefab in that folder that you can use as a base.
2. Go to Window -> Asset Management -> Addressables -> Groups and add your new prefab to the `Supercampaigns` group.
3. Now you can load the prefab at runtime using the AddressableInstantiator and the Addressables API. You can find an example of how to do this in the `Assets/Scripts/CampaignsMap/SupercampaignsMapManager.cs` script.

More information on what Addressables are and how to use them can be found in the [official documentation](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html).
