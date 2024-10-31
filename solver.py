import math
import random
import numpy as np
import matplotlib.pyplot as plt


class TerminalNode:
    def __init__(self, terminal_id: int, coordinate: tuple[int, int], airflow: int = None):
        self.terminal_id = terminal_id
        self.coordinate = coordinate
        self.airflow: int = airflow


class DuctNode:
    def __init__(self, node_id: int, coordinate: tuple[int, int]):
        self.node_id = node_id
        self.coordinate = coordinate
        self.exposed_sides = []  # sides where ducts can still be attached

    def update_exposed_sides(self, duct_system_geometry: set[tuple[int, int]]):
        """
        Update which sides of the duct are exposed and open for connection.
        """
        directions = [(1, 0), (-1, 0), (0, 1), (0, -1)]
        self.exposed_sides = [
            (dx, dy) for dx, dy in directions
            if (self.coordinate[0] + dx, self.coordinate[1] + dy) not in duct_system_geometry
        ]


class DuctMain:
    def __init__(self, geometry: list[DuctNode]):
        self.geometry = geometry
        # self.connections = {i for i in self.nodes}
        self.duct_system_geometry = set()

    def unattached_nodes(self) -> list[DuctNode]:
        """
        Getter method to expose which duct nodes are still available for branch connection.
        Prevents overlap (faster search).
        """

        # update exposed sides for each node
        for node in self.geometry:
            node.update_exposed_sides(self.duct_system_geometry)

        return [x for x in self.geometry if x.exposed_sides]

    def initial_node_open_sides(self, node: int) -> list[tuple[int, int]]:
        vector = []
        coord = self.geometry[node].coordinate
        all_coords = [x.coordinate for x in self.geometry]
        directions = [(1, 0), (-1, 0), (0, 1), (0, -1)]
        """
        check which direction (up down left right) does not have other main ducts        
        (1,0) for open left
        (-1,0) for open right
        (0,1) for open up
        (0,-1) for open down
        """

        for dx, dy in directions:
            neighbor = (coord[0] + dx, coord[1] + dy)
            if neighbor not in all_coords:
                vector.append((dx, dy))

        return vector


class DuctBranch:

    def __init__(self, geometry: list[DuctNode]):
        self.geometry = geometry
        self.length = self.calculate_length()

    def calculate_length(self) -> float:
        """
        Calculate the length of the branch based on its geometry.
        The branch starts at the first provided coordinate and ends at the final coordinate.
        """
        length = 0
        # Iterate through each pair of consecutive nodes in the branch
        for i in range(1, len(self.geometry)):
            # Get the coordinates of the current node and the previous node
            current_node = self.geometry[i].coordinate
            previous_node = self.geometry[i - 1].coordinate
            # Calculate the Manhattan distance between the nodes (L1 norm)
            length += abs(current_node[0] - previous_node[0]) + abs(current_node[1] - previous_node[1])
        return length


class DuctSystem:
    def __init__(self, terminals: list[TerminalNode], main_duct: DuctMain):

        self.main_duct = main_duct
        self.terminal_locations = [terminal.coordinate for terminal in terminals]
        self.main_duct_nodes: list[DuctNode] = main_duct.geometry
        self.branches: list[DuctBranch] = []

    def generate_connection(self, current_state):

        new_state = current_state.copy()
        new_terminal, new_terminal_coordinates = random.choice(self.terminal_locations)
        connect_to_node: DuctNode = random.choice(self.main_duct.unattached_nodes())
        new_path = self.generate_path_to_terminal(connect_to_node, new_terminal)
        new_branch = self.generate_branch_from_path(new_path)

    def generate_path_to_terminal(self, starting_node: DuctNode, objective_terminal: TerminalNode) -> list[
        tuple[int, int]]:
        """
        Generates a valid path from the starting node to the terminal node.
        Constrained to 90-degree turns.

        TODO: implement 90deg turns sooner for random approaches, integrate with temperature function
        """
        path = [starting_node.coordinate]  #  path will look like a series of coords from start, elbow then finish that will create the travel vector
        x, y = starting_node.coordinate
        tx, ty = objective_terminal.coordinate

        # Step along the X-axis first, then the Y-axis, making 90-degree turns

        if random.choice([0, 1]) == 0:
            if x != tx:
                path.append((tx, y))  #  horizontally
            if y != ty:
                path.append((tx, ty))  #  vertically
        else:
            if y != ty:
                path.append((x, ty))  #  vertically
            if x != tx:
                path.append((tx, ty))  #  horizontally

        return path

    def generate_branch_from_path(self, path_coordinates: list[tuple[int, int]]) -> DuctBranch:
        """
        Generates a DuctBranch object based on a given path of coordinates.
        Fills in any skipped coordinates along straight lines to ensure all points are covered.
        :param path_coordinates: A list of coordinates representing the path for the duct branch.
        """
        full_path = []

        # Fill in skipped coordinates along straight lines
        for i in range(len(path_coordinates) - 1):
            current = path_coordinates[i]
            next_point = path_coordinates[i + 1]

            # Add the current point to the full path
            full_path.append(current)

            # Fill in all intermediate points between current and next_point
            cx, cy = current
            nx, ny = next_point

            # If moving along the x-axis, add all intermediate x points
            if cx != nx:
                step = 1 if nx > cx else -1
                for x in range(cx + step, nx, step):
                    full_path.append((x, cy))

            # If moving along the y-axis, add all intermediate y points
            if cy != ny:
                step = 1 if ny > cy else -1
                for y in range(cy + step, ny, step):
                    full_path.append((cx, y))

        # Add the last point (the terminal)
        full_path.append(path_coordinates[-1])

        # Create DuctNode objects for each point in the full path
        duct_nodes = [DuctNode(node_id=i, coordinate=coord) for i, coord in enumerate(full_path)]

        # Create and return the DuctBranch object
        return DuctBranch(geometry=duct_nodes)

    def evaluate_branch(self, proposed_branch: DuctBranch):
        """
        TODO check for overlap on main duct
        TODO check for overlap on terminals that are not objective
        TODO check for overlap on other ducts that are already in self
        """
        current_branches = self.branches  #  list of current branches commited (should NOT have collision errors within set)
        branch_check: bool = True
        proposed_coords = [x.coordinate for x in proposed_branch.geometry]

        #  Check for any overlap in proposed new branch with existing branches
        for b in current_branches:
            if proposed_coords in [node.coordinate for node in b.geometry]:
                return False

        #  Check for any overlap in proposed new branch against existing terminals

        return branch_check

    def all_geometry_coordinates(self):
        all_duct_coordinates: list[tuple[int, int]] = []

        # Collect coordinates from main duct geometry (assuming it's a list of DuctNode objects)
        for node in self.main_duct.geometry:
            all_duct_coordinates.append(node.coordinate)

        # Collect coordinates from each branch's geometry (assuming branches is a list of DuctBranch objects)
        for branch in self.branches:
            for node in branch.geometry:
                all_duct_coordinates.append(node.coordinate)

        return all_duct_coordinates

    def plot_system(self):
        """
        Plot the duct system including the main duct, branches, and terminal nodes.
        """
        plt.figure(figsize=(10, 8))

        # Plot the main duct
        main_x = [node.coordinate[0] for node in self.main_duct.geometry]
        main_y = [node.coordinate[1] for node in self.main_duct.geometry]
        plt.plot(main_x, main_y, 'bo-', label='Main Duct')  # Blue circles for main duct

        # Plot each branch
        for branch in self.branches:
            branch_x = [node.coordinate[0] for node in branch.geometry]
            branch_y = [node.coordinate[1] for node in branch.geometry]
            plt.plot(branch_x, branch_y, 'ro-', label='Duct Branch')  # Red circles for branches

        # Plot terminal nodes
        terminal_x = [terminal[0] for terminal in self.terminal_locations]
        terminal_y = [terminal[1] for terminal in self.terminal_locations]
        plt.scatter(terminal_x, terminal_y, color='green', label='Terminals', zorder=5)  # Green for terminals

        # Adding labels and legend
        plt.title('Duct System Layout')
        plt.xlabel('X Coordinate')
        plt.ylabel('Y Coordinate')
        plt.grid(True)
        plt.axhline(0, color='black', linewidth=0.5, ls='--')
        plt.axvline(0, color='black', linewidth=0.5, ls='--')
        plt.legend()
        plt.axis('equal')  # Equal scaling of x and y axes
        plt.show()


class SimulatedAnnealing:
    def __init__(self, initial_state: dict[int, tuple[int, int]], terminal_locations: dict[str, tuple[int, int]],
                 main_duct: DuctMain):
        self.current_state = initial_state  # The current configuration of ducts
        self.terminal_locations = terminal_locations
        self.main_duct = main_duct
        self.temperature = 100  # Initial temperature for annealing
        self.alpha = 0.95  # Cooling rate

    def cost_function(self, state: dict[int, tuple[int, int]]) -> float:
        """
        Calculate the cost of the current state. This could be based on duct length, number of turns, etc.
        """
        total_length = 0
        for node_id, node_coord in state.items():
            # Calculate the cost of the branch to each terminal
            for terminal_id, terminal_coord in self.terminal_locations.items():
                total_length += abs(node_coord[0] - terminal_coord[0]) + abs(node_coord[1] - terminal_coord[1])

        return total_length  # The lower the total length, the better

    def generate_neighbor(self) -> dict[int, tuple[int, int]]:
        """
        Generate a neighboring state by randomly changing a connection.
        """
        new_state = self.current_state.copy()
        # Pick a random terminal and move it to a different node in the main duct
        random_terminal = random.choice(list(self.terminal_locations.keys()))
        new_connection_node = random.choice(list(self.main_duct.unattached_nodes()))

        # Move the terminal to connect to a new main duct node
        new_state[random_terminal] = self.main_duct.geometry[new_connection_node]

        return new_state

    def acceptance_probability(self, old_cost: float, new_cost: float) -> float:
        """
        Calculate the acceptance probability. If the new solution is better, accept it.
        If it is worse, accept it with a probability based on the temperature.
        """
        if new_cost < old_cost:
            return 1.0
        else:
            return math.exp((old_cost - new_cost) / self.temperature)

    def anneal(self):
        """
        Run the simulated annealing process.
        """
        while self.temperature > 1:
            # Generate a neighboring state
            neighbor = self.generate_neighbor()

            # Calculate the costs
            old_cost = self.cost_function(self.current_state)
            new_cost = self.cost_function(neighbor)

            # Decide whether to accept the neighbor state
            if self.acceptance_probability(old_cost, new_cost) > random.random():
                self.current_state = neighbor  # Accept the neighbor

            # Decrease the temperature
            self.temperature *= self.alpha

        return self.current_state  # Return the best found state
