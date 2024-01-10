# How to create a new campaign

Campaigns in our game are predefined sets of levels arranged in defined positions on-screen with their assets and names. Through a campaign screen, a player can either choose the current level to battle or go back. Campaigns also have a background.

To create a campaign, you must create a variant of the SimpleCampaign prefab, and then modify it as pleased (see Campaign1 and Campaign2 prefabs). SimpleCampaign works as a template on which others are based. In the future we may have other campaign types.

To display the changes graphically, you can add your variant prefab on the Campaign scene as a child to the canvas. This will allow you to see the campaign in the scene tab. Remember to delete it afterwards (if you didn't touch your Unity config, prefab changes should auto-save) as the render is handled by the CampaignManager component of the canvas.

Levels inside your campaign must be under a GameObject that has a CampaignLevelManager component. This takes care of showing the button on click. An error will be logged if this is not met.

Another thing to take into account is that the names of the "CampaignToComplete"/"CampaignToUnlock" field in Levels and the name of the Campaign object in the CampaignsMap scene must match.
