2025-01-25 02:09:57

Tags: #draft 

Links: [[Atlanta GroupGameDev - project Cyberpowers]] 

---

Shaman - nature, elements.
Power.
Mountain - hidden volcano. Maybe built over with metal imitation of a pyramid.

Elemental power - Earth, Water, Air, Fire, Lightning.
Power comes from journey, it is means to reach a destination, not the destination itself.
Power helps gather more power.
Synergies between elements create effect bigger than the sum of their parts.

4 areas, one for each basic element.
5th area - boss challenges - lightning.

4 types of gameplay challenges:
- Farming (Earth)
- Puzzles (Water)
- Traversal (Air)
- Combat (Fire)

Start at a bottom of a mountain, go through each area towards the peak.

2-4 minutes of gameplay per area!

## Elements
### Earth - farming
Jungle area at the bottom of mountain.

##### Earth ability - grow vines
- **Farming - old vines can be composted into fertilizer**
- Puzzles - vines can be navigation markers, ???
- Traversal - grappling vine-hook
- Combat - vines entangle enemies (slow)

##### Earth essence pickup boosts
- **Farming - planting cooldown reduction**
- Puzzles - ???
- Traversal - Grappling vine-hook length
- Combat - ???

##### Earth challenges
1. **Farming - grow a bunch of vines**
2. Puzzle - maze, use vines to help navigate?
3. Traversal - move through jungle with a grappling vine-hook, tarzan style
4. Combat - at night, robot centipedes attack, have to entangle them with vines. (melee enemies)
5. Final combined - centipedes are overwhelming - grow vines near steep incline, climb it. (farming, puzzle, traversal, combat)


### Water - puzzles
Should it be raining, or an underwater area? or swimming upstream dodging incoming stuff?
Yes. (if have time for all)

##### Water ability - spray water
- Farming - boosts growth rate
- **Puzzles - ???**
- Traversal - ???
- Combat - short bursts of water short circuits robots (stun), long exposure rusts (vulnerability)

##### Water essence pickup boosts
- Farming - ???
- **Puzzles - ???**
- Traversal - movespeed? (need speed to swim upstream?)
- Combat - ???

##### Water challenges
1. Farming - grow trees?
2. **Puzzle - high-pressure spray wall to reveal, then ???**
3. Traversal - geysers, water currents???
4. Combat - Turrets shooting water jets, after second, electrifies (ranged stationary enemies)
5. Final combined - ???


### Air - traversal
Open air area around middle of the mountain.

##### Air ability - play songs
- Farming - buff plants to grow faster
- Puzzles - ???
- **Traversal - ???**
- Combat - hack robots with music, disorienting? drones drop from sky?

##### Air essence pickup boosts
- Farming - ???
- Puzzles - ???
- **Traversal - jump height/gliding duration/song effect**
- Combat - ???

##### Air challenges
1. Farming - grow bouncy shrooms - jump pads?
2. Puzzle - ???
3. **Traversal - to reach pickups, need altitude - gain with pads, keep by gliding. Adjust to wind, use air currents**
4. Combat - Drones - buzzing, annoying - disrupts music with static? Shoots missiles? create gusts of wind that disrupt you? (ranged, mobile enemies). Have to hack with music and then knock out by colliding.
5. Final combined - ???


### Fire - combat
Inside the mountain - tunnels with magma veins for lighting.

##### Fire ability - fireball
- Farming - warmth and light for plants, fertilizer from ash
- Puzzles - hold fireball to light up hidden parts
- Traversal - double jump explosion?
- **Combat - Fireball**

##### Fire essence pickup boosts
- Farming - ???
- Puzzles - ???
- Traversal - ???
- **Combat - ???**

##### Fire challenges
1. Farming - Grow grass, burn it (cycle of life and death, fertilizing new life)
2. Puzzle - fire burns ropes, drops bridge, then ???
3. Traversal - cave where floor is lava?
4. **Combat - Enemies that also shoot fireballs?**
5. Final combined - ???


### Lightning - final combined
Mountain peak plateau. Cold, windy. 

##### Lightning ability - lightning strike
- Farming - ???
- Puzzles - create electricity to power something, ???
- Traversal - dash?
- Combat - Combine elements into lightning - shoot fireballs into sky to create thunderclouds. If a lot of small fireballs - lots of small lightning strikes to hit waves of mobs. If big fireball, big lightning strike.

##### Lightning essence pickup boosts
- Farming - ???
- Puzzles - ???
- Traversal - ???
- Combat - ???

##### Lightning challenges
Will need to warm up air to use other abilities effectively.
- Plants dont grow in cold.
- Water turns to ice armor if used on robots.
- Songs drowned out by wind - bubble of warm air to make wind quiet?

1. Farming - ???
2. Puzzle - ???
3. Traversal - ???
4. Combat - Waves + boss, ???
5. Final combined
  - Vines holding boss, it is drenched in water, music disorients? (music is electric, hehe)
  - Fireball into sky to create thunderclouds
  - Zap with super lightning strike, which not only hits it, but also ground beneath, which triggers volcanic eruption, melting the boss and lair (and also a lot of surrounding area, but once again, from ash will come new life)



## Design considerations
Want to let player use all the skills combined for longer, in 4 more areas like the first ones, just with more mobility to have fun, tools to do cool stuff.
Maybe can be after boss is beaten? Could have to go down and quickly grow out new life after lava cools off.
After getting down, can use up all gained power to shed the metal shell of the pyramid, revealing the volcano beneath.
Or leave it as a cautionary monument.


Want to add power generators that power the robots and need to be destroyed. Not sure where to slot them in?

Some pickups obvious, some hidden. Need only some to beat the game, but can do a 100% on the way down the mountain, or in new game.

Some areas enemies come at night, others triggered by picking up big power essence, or solving the puzzle, or something else.


Elements work better in their area

Uzturbint planta su cristalais


In home farm, centipedes attack to drive player forward, also show that game has enemies

## Setting
Post-war AI mama wakes up, starts overtaking nature.



## Lore delivery
Through quotes.
"Power is a tool to help reach the destination, not destination itself"
"Power begets power"
"Different powers create more than the sum of their parts"



## Technique focus
Smooth movement.
Good lighting.
Maybe procedural animations? https://www.youtube.com/watch?v=e6Gjhr1IP6w&ab_channel=Codeer

## Enemy design
Procedural animation is closer to how things animate in reality.

Spider will jump, and in air will be floater.
Depending on distance to ground, will adjust leg positions.

Brain notices prey in jumping distance.
(Cone raycast)
Sends signal to legs. (Event)
Legs move towards center while body rotates and lowers towards prey.
(Raycast is pointing closer to center due to body being lower)
Body rapidly ascends, spider gets pushed up/starts moving in arc towards target.
()
During ascent arc, body lowers to lowest, legs go to center.
(Raycast targets are outside range of ground - target moved to center)

During descent, body rises to max, legs extend toward target.
(Leg target moves on target)
On hit, body lowers to default.
After hitting legs go back to default
(Raycast reaches ground)

If not reaching ground, target self or ground/prey where will land.


Need to see where raycasts hit - either upgrade debugdrawray or drawline.

Need to see where target is.

When legs need to move, move snap target in an arc towards hit.point.
Can just do on fixed update - move the target where it should be this frame - calc arc based on distance from last ground contact and target - rise, if more than half way, lower.

If leg in motion, either go towards the set hit.point or always try to reach current hit.point.
Try both, see difference for both quadruped and bipedal movements.

Spider rises on two feet to attack.

No need for perfection - good enough and move on

### spider vs floater vs centipede
##### Common to all
Senses - shared with AI, always see you? sends signals when in distance to do more that walk towards
current leg positions, etc.
- may have different distance

attacks and walking are same stuff - target to move towards, current fixed position, arc based on distance.

when attacking, leg goes not to default position raycast, but to prey marker

##### Floater
Doesnt have Raycasts for leg targets
Has simple script to wiggle tentacles


## Next tasks
### MA
Find wine 3d asset
add ability to quickly grow bonded (actual objects, not part of terrain) plants around you
 - grow garden plants
add ability to conjure vines
   (add ability to bond with plant (change from terrain to actual object))

add rest of colliders to trees (now only dragon tree has collision)

terrain template - only added all trees, not spawned in scene yet.
also same all terrain layers

try to paint a good looking area. take references from red dead redemption2, horizon zero dawn??
Start with full jungle, remove trees to make clearing

go back to checkpoint button

TENTACLE WIGGLING FOR FLOATER
(leg target moves and rotates every frame)
snake?

tiki torches?

## Done
Basic procedural animation
terrain creation pipeline
basic lighting
character controller


## Log
Saturday 17:00, we fixed last critical issue with logic, have most prefabs, can focus on building levels.
Have some terrains to place stuff in, just need to place more stuff, then remove some.
Want to have a brush for placing prefabs

Sunday 05:53 sleep deprivation is detrimental.

TRIAL BY FIRE IS COMPLETE! Was difficult, but very much worth it.

---
#### References

Entry for BIGMODE 2025 Game Jam by Mindaugas and Laimonas.

[itch.io](https://nonumbersgames.itch.io/atlanta-2173-flowers-vs-robot-spiders)

[github](https://github.com/mandrulevicius/elemental-ascension-BIGMODE2025)

[youtube](https://youtu.be/soauF7iegT0)