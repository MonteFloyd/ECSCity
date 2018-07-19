# ECSCity
High population city simulation in Unity ECS. The purpose of this project is to get more familiar with new ECS and push the performance and flexibility limits of the new features.

The project currently runs on 2018.2.0b7, newer versions of ECS/Entities/Burst package or Unity versions might have troubles as the code needs to be refactored to fit new changes.

ECS is currently is not production ready and have many shortcomings but still, performance wise it is far superior to the old style of OO based design.


## Features

### General outline

ECSCity people(tiny yellow things) currently only have homes and works, depending on the time they either go work or go home. they just follow their schedule.

### Local avoidance

Masses(people) in ECSCity uses a modified version of Unities Boid system, except they all have their own targets(home or work) and their schedule to follow. Currently there are only workers but there can be more features added in the future as ECS grows. Currently this **LocalAvoidanceSystem** puts people into cells depending on their positions(X and Z in my version) and forces them to split depending on the values given in Settings.

System is quite fast but far from optimized currently, the most obvious problem is that it runs every single fram. One future possible improvement is to split this simulation system into another world and force it to run at a a lower fps compared to rendering world.

### Physics Systems

By default entities have no gravity, force attached to them unless needed. When something related to physics happen to an entitiy, like if player uses a bomb near it, then **BlowUpSystem** attaches relevant physics components the to entity and those systems start doing their job on the entity. Gravity system pulls down entity, force/velocity system applies force/velocity and collision system checks for collision.

If a system is done with an entity, other relevant systems can also strip those components away and entity can revert back to their default state not affected by that physics system. This kind of flexibility is most apparent in Infection System.

### Infection System

Infection system works by attaching a Infection component to an entity. This component keeps track of its own clock which kills the entity when it reaches zero. The clock is managed and monitored by the relevant **InfectionKillingSystem** while how Infection in general spreads is handled by **InfectionSystem** .

The feature can be further improved with the possibility of cures and such. 

### Future Work

Possible NavMesh introduction, better local avoidance, more flexible scheduling options, better models and possible animations with more advanced physic systems as ECS develops further.

