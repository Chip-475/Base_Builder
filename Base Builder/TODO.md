# Base Builder — project completion checklist

Ordered from highest priority to lowest. A task is complete only when its acceptance criteria pass in a clean build and in a fresh playthrough.

## P0 — make a complete, playable vertical slice

- [ ] **Define the release target and victory condition.** Write down the minimum shippable loop: crash site → gather ore → refine/craft → build power and production → automate logistics → repair the ship → victory screen. Decide exactly what “ship repaired” requires and which features are out of scope.
- [ ] **Fix the startup/new-game flow.** Put `MainMenu.unity` and `Main.unity` in Build Settings in the correct order; wire Play, Options, Quit, New Game, Continue, and return-to-menu. A new game must start cleanly, while Continue must load exactly one save.
- [ ] **Remove runtime crash blockers.** Resolve every `NotImplementedException` and null-sensitive path in the core loop, especially bot interaction, machine workplace output, spaceship destruction/workplace methods, player movement on empty cells, and generator fuel/status access.
- [ ] **Fix the machine and bot UI control flow.** Eliminate collection mutation during `MachineUI.BuildBotList`, make recipe selection/amount changes actually start work, and ensure the correct bot type can be assigned and released without duplicated jobs or stuck occupied states.
- [ ] **Initialize progression data at startup.** Call `RecipeManager.Initialize`, verify default recipes are available, and ensure building/recipe IDs are unique and resolve correctly from the scene and save data.
- [ ] **Prove the core economy loop end-to-end.** In a fresh game, verify: mineral node → miner → crate/depot → carrier → smelter/assembler → output → inventory/build cost. Include insufficient inventory, full inventory, missing path, missing bot, and missing power cases with recoverable UI feedback.
- [ ] **Make construction and power reliable.** Test placement, construction-site reservation, builder completion, deletion/refunds policy, pole connectivity, generator fuel consumption, network merging/splitting, machine power requests, and recovery after destroying/rebuilding a connected building.
- [ ] **Implement the ship-repair ending.** Assign the navigation/core controls in the scene, implement their costs and persistent progress, prevent double-spending, show clear progress, and transition to a victory state when all required components are complete.

## P1 — finish the major gameplay systems

- [ ] **Complete the tech tree.** Enable tech entry icons/names/buttons, implement prerequisite checks, spend costs once, apply effects once, lock/unlock buildings and recipes in the UI, and restore unlocks correctly after loading.
- [ ] **Finish save/load as a real game feature.** Add New Game/Continue/Save/Load behavior, clear or isolate existing runtime objects before loading, and serialize all state that affects progress: buildings, construction sites, bots/tasks/inventories, depot inventory, machine inputs/outputs/progress, generator fuel/state, techs, and ship-repair progress.
- [ ] **Finish the missing UI.** Replace generator placeholders with real per-generator production/consumption, complete spaceship inventory/repair/tech menus, refresh lists safely while open, show unavailable reasons, and provide consistent close/back behavior and input-mode restoration.
- [ ] **Add player guidance.** Provide a short onboarding/tutorial or objective panel that explains gathering, building, power, bot assignment, research, and the ship objective; surface pathing, power, and resource failures where the player can act on them.
- [ ] **Stabilize scene and prefab wiring.** Remove duplicate/obsolete scene variants or clearly mark the authoritative one; validate all serialized references, prefab data IDs, UI buttons, bot prefabs, resource lists, and save databases before every build.

## P2 — content, balance, and usability

- [ ] **Author the intended progression.** Complete the first-era recipe chain and tech unlock path, set sensible costs/times/power values, and ensure every required resource has a source, recipe, sprite, and UI entry.
- [ ] **Balance bots and automation.** Tune bot costs, speed, capacity, mining, power, carrier priorities, worker throughput, construction speed, and failure recovery so manual play is possible but automation is rewarding.
- [ ] **Improve feedback and readability.** Add clear build previews, blocked-cell reasons, power-network status, machine progress/output states, bot task states, inventory capacity, and success/failure notifications.
- [ ] **Finish audio/visual polish.** Wire master/music/SFX settings, add missing sound cues, replace placeholder art/text, and add animations for mining, construction, crafting, power state, ship repair, and victory.

## P3 — release hardening

- [ ] **Add repeatable tests.** Cover inventory arithmetic, pathfinding, construction reservation/release, logistics requests, power allocation, tech unlocks, save serialization, and ship completion with edit-mode or play-mode tests where practical.
- [ ] **Run a full QA matrix.** Test fresh start, save/quit/load, scene reload, destroying/rebuilding, insufficient resources, full inventories, disconnected power, bot pickup/drop, blocked paths, and every machine/bot/building type without Console errors.
- [ ] **Profile and optimize.** Remove per-frame debug logging, avoid unnecessary network rebuilds/UI instantiation, check pathfinding and bot update costs, and verify acceptable performance on the target hardware.
- [ ] **Create the release build.** Set application metadata, resolution/input defaults, save location behavior, build scenes, platform settings, credits/license information, and a short player-facing README; produce and smoke-test a clean build from a fresh checkout.

## Current baseline

- The C# solution currently builds successfully, with four `AudioManager` field-assignment warnings.
- The repository README still describes the game as early development and not yet playable.
- The most urgent known gaps are in startup flow, runtime safety, ship repair, tech-tree interaction, save/load completeness, and unfinished UI.
