# Project 2: NPC World

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Lucas Corey
-   Section: 03

## Simulation Design

My simulation is going to be a farm with several types of animals. These animals will mostly react to each other as they would in the real world.

### Controls

-   _List all of the actions the player can have in your simulation_
    -   _Include how to preform each action ( keyboard, mouse, UI Input )_
    -   _Include what impact an action has in the simulation ( if is could be unclear )_

## Farm Animal

This agent encompases my 3 different farm animals: cows, chickens, and sheep

### Safe

**Objective:** If the animal is in no danger, then it wanders around the farm.

#### Steering Behaviors

This state uses Roam and Seek

Roam will tell the animal to periodically wander to a random point on the map
Input for Roam is a random point on the map and path efficiency

Seek will cause the animal to sometimes seek a fellow farm animal (of any type) 
until it reaches it. Why? I don't know, but it seems funny. This will use a reference
to a random farm animal.

For now, the only obstacle is a hay bale.

This state will cause the animal to separate from anything other than its own type (i.e. a chicken will stay with chickens and not cows or sheep)
   
#### State Transistions

When this agent is out of range of a predator for enough time.
   
### Panic

**Objective:** Flee the cause of the state

#### Steering Behaviors

This state will use a combination of Roam and Evade
The animal will Roam at high speed to a position that evades the enemy position

This state also avoids haybales

No separation.

#### State Transistions

When the agent comes within range of a predator

## Predator

This agent is a predator, and it will seek farm animals in order to kill them.

### Hunt

Search for an animal to pursue

#### Steering Behaviors

The predator will use Wander movement to chance upon a farm animal

This state avoids haybales

This state separates from other predators and Cows (cows are too big for any of the predators im including)

#### State Transistions

This state will ocurr if the predator loses a prey in a chase
   
### Attack

**Objective:** Pursue a trageted farm animal (other than a cow) to kill

#### Steering Behaviors

This state will use Pursue

This state will not avoid haybales (just imagine them jumping over it for now).

This state will separate from other predators (with less weight than the pursuit)
   
#### State Transistions

The predator will enter this state when it spots a fitting farm animal (is within a certain distance)

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

For now, this is what I plan on doing:
    Custom assets
    3 Agents (3rd hasn't been thought of enough yet to include here)
    More states if appropriate
    Menu System (I'll try and look up how to get the UI to align right on WebGL)
    I might want to do more, but this is what I think I have time for. 

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

