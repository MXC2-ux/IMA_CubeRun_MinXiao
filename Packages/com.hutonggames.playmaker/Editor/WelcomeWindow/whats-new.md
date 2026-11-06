# :star: What's New

For a full list of changes, see the ChangeLog.

## [2.0.0 beta 92] - 2026-10-24

### IMPROVED

- Improved tooltips in Interactable system actions and components.
- Allow selection of nodes in prefab instance locked mode. Note, inspecting an outdated prefab may update its instance data and create an override; update the prefab asset to resolve this.
- Optimized first time opening of Type selector.

### ADDED

- Added custom Node Colors to [Settings](menu:PlayMaker/Help/Settings) with an option to show names.
- Added an auto-tint setting for node titles so bright node colors use dark text.
- Added Node Descriptions [settings](menu:PlayMaker/Help/Settings) for FSM and region frames, states, and global transitions in the graph view. Node descriptions show text up to the first --- divider; use this to show less text on the canvas than in the inspector.
- Double-click state descriptions in the graph view to edit them in the inspector.
- Added [WaitForAnimatorEnterState](action:WaitForAnimatorEnterState) and [WaitForAnimatorExitState](action:WaitForAnimatorExitState) actions.
- Added optional Rotation Target to [GameObjectDockWithInteractable](action:GameObjectDockWithInteractable) action. This allows you to translate one object and rotate a different one. For example if you have a character setup where the root translates, and a child model rotates.
- Added interaction state to [ThirdPerson Samples](addon:samples.third-person) AnimatedCharacter sample.
- Added Delete Unused Events to Events settings menu.

### FIXED

- Fixed [Localization](addon:localization) and [Networking](addon:networking) addons missing their custom editors.
- Fixed Localize Property to work with FSM Components and GameObjectLocalizer.
- Fixed runtime-only variable types, including Scene, missing a value row. It now shows a read-only value while playing.
- Fixed FSM description edits sometimes not updating the Graph View.
- Fixed Markdown edits not committing when focus leaves the text field.
- Fixed [DataTableWidgetGetSelection](action:DataTableWidgetGetSelection) row key with Use Name as Key.
- Fixed Markdown checklists so they only show the checkbox (not the bullet).
- Fixed graph view description checkboxes not updating the source Markdown.
- Fixed state and transition highlights missing from the graph view when entering Play Mode.
