# How to add Supercampaigns prefabs as Addressables

Once you have created a new Supercampaigns prefab, you need to add it as an Addressable so that it can be loaded at runtime. Here's how you can do that:

0. Make sure you have the Addressables package installed. If you don't, you can install it from the Package Manager.
1. Create a new prefab in the `Assets/Prefabs/Supercampaigns` folder. You will find a template prefab in that folder that you can use as a base.
2. Go to Window -> Asset Management -> Addressables -> Groups and add your new prefab to the `Supercampaigns` group.
3. Now you can load the prefab at runtime using the AddressableInstantiator and the Addressables API. You can find an example of how to do this in the `Assets/Scripts/CampaignsMap/SupercampaignsMapManager.cs` script.
    3.1. In the `AddressableInstantiator` component, write a new method to instantiate the new supercampaign. Use `InstantiateAsync()` to load the prefab and instantiate it at runtime.
    3.2. In the `SupercampaignsMapManager` script, write a new clause in the `InstantiateSupercampaign` method to instantiate the new supercampaign. Use the method you created in the `AddressableInstantiator` component to do this.
    3.3. Go to the `SupercampaignsMap` scene and assign the new supercampaign prefab to the serializable field in the `SupercampaignsInstantiator` component.
    3.4. In the previous scene to your supercampaign, add a button that calls the `SceneNavigator` to load the `SupercampaignsMap` scene with your new supercampaign's name (as you added it in the `SupercampaignsMapManager` script).

More information on what Addressables are and how to use them can be found in the [official documentation](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html).
