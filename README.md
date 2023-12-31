# BoidSimulator: Unity project to experiment with boids

Welcome to my boid simulator project! While reading through some game development articles, the concept of boids caught my eye 
and I thought thought to myself "Woah this is so cool, I want to try this". This project is to learn more about boids and 
their potential applications in game development.

## Boids? What is that? 
![Animated GIF](https://media2.giphy.com/media/5t9wJjyHAOxvnxcPNk/giphy.gif?cid=ecf05e47wcfr411dqf5p8ov9z8pz0j2pw0044uey4o5wgwj0&ep=v1_gifs_search&rid=giphy.gif&ct=g)
<br>
Boids are autonomous agents that exhibit collective behavior by following a set of simple rules. They were introduced by 
Craig Reynolds in 1986 as a means to simulate the behavior of flocks of birds. 
The name "boids" is a portmanteau of "bird-oid objects," reflecting their original inspiration. 
Boids are characterized by three fundamental rules:
* **Separation**: Each boid tries to maintain a minimum distance from its neighbors, preventing overcrowding and collisions.
* **Alignment**: Each boid tries to align its velocity with the average velocity of its neighbors, resulting in a cohesive group movement.
* **Cohesion**: Each boid tries to move toward the center of mass of its neighbors, aiming for a tight formation.

<br> You can access the paper on boids by clicking [here.](https://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/) 

## Project Overview
In this Unity project, I have implemented a boid simulation that demonstrates the flocking behavior of boids. The simulation consists of two types of boids:
* **Flocking Boids**: These boids follow the three basic rules of separation, alignment, and cohesion. They exhibit flocking behavior,
  maintaining cohesion and avoiding collisions as they navigate the virtual environment.
* **Predatory Boids**: These boids are designed as predators to the flocking boids. They actively seek out and pursue the flocking boids,
  disrupting their formation. The flocking boids, in turn, try to evade the predator boids, adding an element of tension and challenge to the simulation.

The primary objective of this project is to showcase the interaction between the flocking boids and the predator boids, emphasizing the dynamic nature of their relationship.

https://github.com/Dom17Pham/BoidSimulator/assets/71042283/ca5bba57-eaa4-433b-adee-0b7fd4f4dc80

https://github.com/Dom17Pham/BoidSimulator/assets/71042283/5e367939-f5ee-4c0e-886e-50940bb9c2a5

## Further Development
* Add additional types of boids with unique behaviors, such as leaders or scavengers.
* Introduce more complex predator-prey dynamics and explore how they influence the flocking behavior.
* Experimenting with different parameters and configurations to improve natural and organic flow of the boids.

## Acknowledgements 
* Thanks to Craig Reynolds for his pioneering work on boids and making the concept widely accessible.
