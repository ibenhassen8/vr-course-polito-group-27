# vr-course-polito-group-27

# Ritual — VR Experience

## Project Overview
**Ritual** is a first-person immersive VR experience in which the player takes on the role of a young apprentice witch alone in an ancient forest, brewing her first spells to complete her initiation trial.

Blending a fairytale, mysterious, and slightly eerie tone, the experience alternates between contemplative exploration and the satisfaction of crafting spells.

### Group Members
- Bazan	Stefano
- Ben hassen	Ines
- Carlini	Nicolo' 
- Carolillo	Ivan
- Restuccia Giuseppe

---

## Concept & Gameplay Loop
Each spell follows a core three-step loop:
1. **Read the Grimoire:** Check the recipe page at the altar.
2. **Explore & Gather:** Search the forest for the right magical ingredients while avoiding useless or harmful ones.
3. **Brew at the Cauldron:** Return to the central altar and perform the preparation sequence (adding ingredients, crushing, and adjusting the fire).

### Evolving Forest Progression
The player always starts from the central clearing and returns to the altar. Every completed spell transforms the same environment and unlocks access to the next area:
- **Part 1 — Firefly Light Spell:** Ignites the cauldron and fills the forest with fireflies, revealing new paths.
- **Part 2 — Root Bridge Spell:** Grows roots across the stream to form a bridge.
- **Part 3 — Rain Spell:** Summons rain, causing new ingredients to sprout.
- **Final Part — Initiation Portal:** With the last page of the Grimoire torn into riddles and guided by the crow, the player gathers the final ingredients across the fully unlocked forest, completes the brew, clears the fog, and steps through the glowing portal to become a witch.

---

## Environment, Lighting & Atmosphere
- **Visual Style:** Stylized night/dusk forest wrapped in fog.
- **Star-Shaped Layout:** A central clearing (altar and cauldron) branching out into distinct micro-zones — the **stream**, the **stone ruins**, the **hollow tree** *(+ 4th zone TBD)* — bounded naturally by dense trees instead of an open world.
- **Modular Kit Optimization:** A compact set of trees, rocks, and bushes reused with scale, rotation, and color variations, plus flat silhouettes for distant trees.
- **Diegetic Guidance:** Magical ingredients emit a subtle glow and distinct sound cues so players can navigate in the dark without an intrusive UI.
- **3D Spatial Audio:** Layered ambient sounds (rustling leaves, wind, creaking wood, owls, flowing water), magical feedback (crystalline chimes, dark hisses from cauldron smoke), and an evolving soundscape (fireflies, rain).

---

## Current Milestone & Repository Structure
This milestone replaces the **A1 blockout** with real, textured modular assets configured as prefabs and lit with **baked lighting** across two dedicated scenes:

- **Environment Scene** (`Assets/Scenes/...`): Star-shaped forest layout assembled using textured modular prefabs.
- **Lighting Scene** (`Assets/Scenes/...`): Night/dusk atmosphere, fog, and baked lighting setup.
- **Prefabs** (`Assets/Prefabs/...`): Reusable modular prefabs (trees, rocks, bushes, props, and landmarks).

---

## Asset Sources & Credits

wait...
---

## Technical Notes
- **Baked Lighting:** *(Add details on lightmap resolution, static flags, and baked lights used in the Lighting scene)*
- **Modular Variations:** *(Note how prefabs are varied by scale, rotation, and material/color tints to avoid repetition)*

## Documento A0
[LINK Concept] (https://docs.google.com/document/d/1ZH_OmsGo6RZMYgo-krfmaInTpPxGHdJSctzhfD9dx5Y/edit?usp=sharing)

