# 1.2.0 (7/19/2026)

### Additions
* Added Git Unity Package Manager support for easy auto-updates. This is now the recommended installation method.
* Added the "IntSyncer" component, allowing for storage of integer values for use in maths and logic.
* Added the "InputTrigger" component, allowing for interactions with inputs (button and axis inputs).
* Added the "MathActivator" component, allowing for algebra calculations and Boolean logic.
* Added the "HostActivator" component, allowing for special host actions (currently, ending the game with custom win logic).
* Added the "ValueSource" type, which allows certain components to grab an integer (whole number) value from a certain source. This includes a constant value, HealthSyncer, IntSyncer, current room info, or trigger source info.

### Changes
* HealthSyncer now functions locally when "Sync" is disabled.
* Activators now have a "Player Filter", allowing for improved control and logic (options are NoFilter, NonPlayerOnly, PlayerOnly, LocalPlayerOnly, HostPlayerOnly).
* Activators now have a "Conditional Logic" section, which allows you to gate Activator execution using ValueSources.
* TargetActivators have a new "Teleport to Player Camera" option, available when "Teleport to Original Target" is true.
* GenericActivators can now trigger a random activator from the list "Random Activator to Trigger".
* RigidbodySyncer teleport has been reworked. Now, a RigidbodySyncer with "Sync" ON and "Host Only" OFF can be teleported by non-host players.

### Bugfixes
* Fixed HealthSyncer always triggering "OnDamaged" activators whenever health is changed.
* Fixed HealthSyncer never triggering "OnHealed" activators when health is increased.

# 1.1.2 (1/25/2026)
* Added the MapLoadedTrigger, TimeTrialStartedTrigger, and TimeTrialFinishedTrigger.
* Added SMG Damage, Double Barrel Damage, and Knockback Multiplier to the match setting overrides.
* Added many new components to the whitelist.

# 1.1.1 (9/2/2023)
* Added Time Trials to custom maps.
* Upgrade Cabinets can now be parented.

# 1.1.0 (8/31/2023)
* You can now build and test a map without a thumbnail.
* Added a maximum size of 200mb to custom maps.
* You can now set spawnpoints for infected players.
* Triggers now show what their target is, making it easier to build TargetActivator setups.
* Added the PlayerDiedTrigger and PlayerGotKillTrigger.
* Added player trigger filtering to Activators.
* The editor will no longer let you test a map if you haven't built it yet.
* The editor now does workshop upload checks for the thumbnail and screenshots itself.

# 1.0.1 (8/30/2023)
* You can now create a new map using the Redmatch 2 > New Map option.
* Maps now require a PostProcessingVolume.
* Match setting overrides will no longer carry on to future maps.
* The editor now automatically installs the required packages.
* Fixed some errors with maps not loading in-game. IDK if all of them are fixed because some are hard to reproduce.
* Stay Triggers have frame independence now via a rolling timer.
* You can now unsubscribe or view the workshop page for custom maps in-game by right clicking the map button.
* You will no longer automatically subscribe to custom maps when loading a match with one. Instead the game will temporarily download it and delete it after. This prevents excessive file bloating over time.
* Replaced the Relative Force toggle with 4 options for a Relative Force Mode: World, Self, Target, and Orbital, which allow you to apply force based on different coordinate systems.
* Relative Force now respects the player's vertical look rotation.
* Local custom maps will now start at 60 minutes by default.
* Added hot-reloading. If the game is open and you build the map with F4, it will reload in-game.
* Press F6 inside Redmatch 2 while on a local custom map to reload it.
* Renamed the "Authority Requirement" setting to "Execute On" and changed "Anyone" to "Everyone" to try to make it less confusing.
* The editor now pings objects that have errors during an error check.

# 1.0.0 (8/25/2023)
* Created a new Demo Map to better showcase all the new features.
* IDs now automatically assign.
* The TriggerActivator has been separated into Triggers and Activators. You reference the Activators using Triggers. The Activator itself does not contain any networking logic. Think of it as a bridge between * Triggers and actions taken on Syncer components.
* Added more actions including dealing damage to HealthSyncers and setting Animator parameters, which allows dynamic Animator logic.
* Added DamageableTrigger, EnterTrigger, ExitTrigger, StayTrigger, GameEndedTrigger, and RoundStartedTrigger.
* Added HealthSyncer, RigidbodySyncer, and GameObjectSyncer.
* Added FilledImageValueDisplay and TextValueDisplay.
* The scene will no longer reset the window layout if you try to build and get an error.
* Layers will now automatically be set up when you import the SDK.
* Added a "Ground" tool for spawnpoints which aligns them to the ground.
* Added sweet icons to all the components.
* Added the CameraFacingBillboard utility script.
* You can no longer teleport/spawn players at weird angles.
* Inactive Upgrade Cabinets will no longer spawn.
* Added consistency to error popups and console messages.
* When using Build and Test, the map will no longer launch if there are build errors.
* Added an error checking feature because wozzA wanted it.
* Added new shortcuts for Check For Errors (F3), Build Map (F4), and Build and Test Map (F5).
* Whitelisted additional components.

# 0.1.0 (8/17/2023)
* Added the TriggerActivator.
* Removed the DamageTrigger as the same functionality is now possible with the TriggerActivator.
* Fixed a bug where you couldn't have more than 1 Upgrade Cabinet.
* Added Text as a usable component.
* The Map folder now deletes itself before building to avoid unwanted contamination from misnaming.
* The editor now asks for confirmation to save your scene before building to prevent the loss of unsaved changes.