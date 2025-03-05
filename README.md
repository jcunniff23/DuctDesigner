### DuctDesigner

This is a repository focused on abstracting the routing for 2D (and eventually 3D) pathfinding problems on a continuous space.

#### References and Resources used
- Kochnderfer, M. J., & Wheeler, T. A. (2019). Algorithms for optimization. The Mit Press.
- Leigh, R., Louis, S. J., & Miles, C. (2007, April 1). Using a Genetic Algorithm to Explore A*-like Pathfinding Algorithms. IEEE Xplore. https://doi.org/10.1109/CIG.2007.368081
- Mowery, J. D. (n.d.). A Genetic Algorithm Using Changing Environments for Pathfinding in Continuous Domains.
- Deb, K., Pratap, A., Agarwal, S., & Meyarivan, T. (2002). A fast and elitist multiobjective genetic algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), 182–197. https://doi.org/10.1109/4235.996017
- Asiedu, Y., Besant, R. W., & Gu, P. (2000). HVAC Duct System Design Using Genetic Algorithms. HVAC&R Research, 6(2), 149–173. https://doi.org/10.1080/10789669.2000.10391255
- Differential evolution - Cornell University Computational Optimization Open Textbook - Optimization Wiki. (n.d.). Retrieved February 1, 2025, from https://optimization.cbe.cornell.edu/index.php?title=Differential_evolution#Initialization







#### Folder Structure 
`~/GenDesign/assets` Jupyter Notebook location for path analysis, imgaes of plots & where csv data is dumped

`~/GenDesign/Program.cs` Entry point method for running GA with hyper parameters (Number of turns, generations, population, mutation rate, crossover rate)

`~/GenDesign/GeneticEvolution.cs` Main class for running GA algorithm

`~/GenDesign/Utils.cs` supporting class/methods for geometry and point manipulation

`~/TerminalDesign/` WIP C# project for applying GA to terminal placement

#### Methodology

