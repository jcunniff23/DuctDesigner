import pyvista as pv

class World:
    def __init__(self, size_x: int, size_y: int, size_z: int, in_jupyter: bool = True):
        self._plotter = pv.Plotter()
        self.sizeX, self.sizeY, self.sizeZ = size_x, size_y, size_z


    def add_line(self, start, end):
        line = pv.Line(start, end)
        self._plotter.add_mesh(line)

    def add_point(self, location):
        point = pv.Sphere(0.5, location)
        self._plotter.add_mesh(point)

    def show(self):
        self._plotter.show()


class Node:
    def __init__(self, x, y):
        pass


