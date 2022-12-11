# Project 2: NPC World

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Lucas Corey
-   Section: 03

## Simulation Design

My simulation is going to be a farm with several types of animals. These animals will mostly react to each other as they would in the real world.

### Controls

Player can click anywhere to place an obstacle

## Farm Animal

This agent encompases my 2 different farm animals: chickens and sheep

### Wandering

**Objective:** If the animal is in no danger, then it wanders around the farm.

#### Steering Behaviors

This state uses Wander, Stay In bounds, and Flocking

For now, the only obstacle is a hay bale.

This state will cause the animal to separate from anything other than its own type (i.e. a chicken will stay with chickens and not cows or sheep)
   
#### State Transistions
When this agent is out of a predator's range
   
### Panicking

**Objective:** Flee the cause of the state

#### Steering Behaviors

This state uses Evade and Stay In Bounds

This state also avoids haybales

### State Transistions

When the agent comes within range of a predator

## Wolf

This agent is a predator, and it will seek farm animals in order to kill them (although it cant kill, just chase).

### Wandering

Search for an animal to pursue

#### Steering Behaviors

Wandering, Staying In bounds, and separation

This state avoids haybales

#### State Transistions

This state will ocurr if the predator loses a prey in a chase
   
### Attacking

**Objective:** Pursue a trageted farm animal

#### Steering Behaviors

This state will use Pursue, Separation, and Staying in bounds

This state also uses obstacle avoidance
   
#### State Transistions

The predator will enter this state when it spots a farm animal (is within a certain distance)

## Sources

No External Sources

## Make it Your Own

For now, this is what I plan on doing:
    Custom assets
    3 Agents (3rd hasn't been thought of enough yet to include here)
    More states if appropriate
    Menu System (I'll try and look up how to get the UI to align right on WebGL)
    I might want to do more, but this is what I think I have time for. 
    
Unfortunately, this is all I had time to do:
    Custom Assets
    3 Agents (sheep, chicken, wolf)
    

## Known Issues

I think that the only issue is sometimes the obstacle avoidance causes animals to get trapped in a breakdance, but this has been reduced after editing some values

### Requirements not completed

I think the movement could be a bit more fine-tuned, but I did what I had time for.

