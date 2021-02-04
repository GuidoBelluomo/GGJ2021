# Buzz Limb-O
##### [A Global Game Jam 2021 Project](https://globalgamejam.org/2021/games/buzz-limb-o-3)

## Credits
* Guido Belluomo - Code, Zombie Voice
* Daniele Coppola - Music, SFX
* Riccardo Fiorini - Pixel Art, Level Design
* Walter Samperi - Pixel Art, Level Design

## Overview
This is a game project made in five days for GGJ2021. I didn't pay a lot of attention to the architecture and design patterns to save on upfront programming cost, but it didn't really help with long-term cost. In fact, most of it became spaghetti code by the end, increasing the cost and resulting in a particularly sleepless night by the second last day.

## Gameplay
You're Buzz, a zombie head with no limbs. Play two levels where you'll find limbs laying around which you can use to your advantage: the limbs have abilities, as well as an "ultimate ability" for traversing big obstacles. This results in losing the limb, but performing very significant actions towards the end goal: **finding a juicy brain.**

## Rules
* Without limbs you'll roll, you'll behave just like a physical ball and that's exactly what you are under the hood.
* With at least a limb you'll walk
* Legs allow you to jump. Pressing up (or W) and Spacebar will make you jump very high and leave the leg behind.
* Arms allow you to grab onto ceilings to swing off of them, as well as making you stop rolling. Jumping while swinging will sacrifice the limb.
* Levers and switches can only be interacted with by throwing an arm at them. (Though actually I think legs work too, but it's pointless.)

## Controls
Controls are explained at the beginning of the first level, but I feel like I need to explain the Swing/Grab action. When you found an arm, you can press Z to grab onto ceilings, which will make buzz Swing. Use the movement keys to gain momentum and Jump to sacrifice the limb and perform a big leap, or simply press Z again to let go (I may have accidentally given the character too much momentum when not sacrificing the limb, it probably works just as well as the jump)

## Known Bugs
* When you have a leg, arms are hard to pickup. Literally goomba-stomp the arm to pick it up, it may take a few attempts the first time but it'll work.
* Animations desync. I never bothered fixing this because it's still somewhat okay looking, I may do this in the future.
* If you get stuck on a fire hydrant, just stop doing anything and wait until it lifts you high enough, then move in the desired direction.
* When you have a leg and get lifted by a fire hydrant, sometimes when partially touching ledges (possibly other cases too) sometimes you'll fall down crazy fast to near teleportation speeds.
