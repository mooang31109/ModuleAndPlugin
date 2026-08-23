// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class ModuleAndPlugin : ModuleRules
{
	public ModuleAndPlugin(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[] {
			"Core",
			"CoreUObject",
			"Engine",
			"InputCore",
			"EnhancedInput",
			"AIModule",
			"StateTreeModule",
			"GameplayStateTreeModule",
			"UMG",
			"Slate"
		});

		PrivateDependencyModuleNames.AddRange(new string[]
		{
			"Test",
			"Temporary"
		});

		PublicIncludePaths.AddRange(new string[] {
			"ModuleAndPlugin",
			"ModuleAndPlugin/Variant_Platforming",
			"ModuleAndPlugin/Variant_Platforming/Animation",
			"ModuleAndPlugin/Variant_Combat",
			"ModuleAndPlugin/Variant_Combat/AI",
			"ModuleAndPlugin/Variant_Combat/Animation",
			"ModuleAndPlugin/Variant_Combat/Gameplay",
			"ModuleAndPlugin/Variant_Combat/Interfaces",
			"ModuleAndPlugin/Variant_Combat/UI",
			"ModuleAndPlugin/Variant_SideScrolling",
			"ModuleAndPlugin/Variant_SideScrolling/AI",
			"ModuleAndPlugin/Variant_SideScrolling/Gameplay",
			"ModuleAndPlugin/Variant_SideScrolling/Interfaces",
			"ModuleAndPlugin/Variant_SideScrolling/UI"
		});

		// Uncomment if you are using Slate UI
		// PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

		// Uncomment if you are using online features
		// PrivateDependencyModuleNames.Add("OnlineSubsystem");

		// To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
	}
}
