import numpy as np
import pyvista as pv
from typing import List
from World import World

## file // class for genetic solver

class Solver(object):
    ## This class will be responsible for running the genetic solver, as well as generating initial routes
    ## Add object/context for visualization ?

    def __init__(self, world: World, initial_solutions: int = 2):

        self.target_point: tuple[int,int,int]
        self.start_point:  tuple[int,int,int]
        self.world = world

        pass



    def initial_generation(self):

        pass
    
    def is_valid(self, x_int, y_int):
        # world will be bound by y=0, x=0 then max values
        xMax = self.world.sizeX
        yMax = self.world.sizeY
        return (x_int >= 0 & x_int <= xMax) & (y_int >= 0 & y_int <= yMax) 

    def heuristic_route(self, step_size: float, entropy: float = 0) -> List:
        neighbors = list[Node]
        route: list[Node]
        ## step size will be how far away algo will move with each iteration of search
        open_set = list[Node]
        closed_set = list[Node]
        
        def set_hCost(node: Node):
            xdist = abs(current_location[0] - self.target_point[0])
            ydist = abs(current_location[1] - self.target_point[1])
            zdist = abs(current_location[2] - self.target_point[2])
            node.hCost = xdist + ydist + zdist
                  
        def calculate_neighbors() -> List[Node]:
            current_neighbors = List[Node]
            currentX = current_location[0]
            currentY = current_location[1]
            currentZ = current_location[2]
            
            for x in [currentX-1, currentX, currentX+1]:
                for y in [currentY-1, currentY, currentY+1]:
                    for z in [currentZ-1, currentZ, currentZ+1]:
                        
                        if (x==currentX & y==currentY & z == currentZ):
                            continue;
                        else:
                            n = Node((x,y,z))
                            n.parent = current_location
                            current_neighbors += n

            return current_neighbors
                            
        def is_destination(node: Node) -> bool:
            return node.position == self.target_point
                        
                            
        #  start by routing from the starting point
        open_set += Node(self.start_point)
            
        
        while open_set.count() > 0:
            #  From current location, find the possible steps in each direction (6 options in 3D, 4 in 2D excuding diagonals)
            #  choose best result
            #  when entropy == 0, algo will route to best & shortest option
            
            # current node will be set to next item in queue
            current_location = open_set[0]
            
            for node in open_set[1:]:
                if node.fCost <= current_location.fCost & node.hCost < current_location.hCost:
                    current_location = node
                    
            open_set.remove(current_location)
            closed_set.add(current_location)
            
            if is_destination(current_location):
                # route = get_path()
                return route;
            
            
            for neighbor in calculate_neighbors:
                pass
        
        
        

    
    
class Node:
    def __init__(self, position: tuple[int,int,int]):
        self.position = position
        self.parent: Node
        self.hCost = 0
        self.gCost = 0
        self.fCost = self.hCost + self.gCost
        
        pass
    
    