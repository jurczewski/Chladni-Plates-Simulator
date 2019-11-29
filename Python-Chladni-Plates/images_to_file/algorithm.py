import numpy as np
from scipy.sparse.linalg import eigsh
from scipy.linalg import eigh
from plane import *


class Algorithm:
    def __init__(self, size):
        self.size = size
        self.triangles_in_row = 2 * (size - 1)
        self.triangles_count = 2 * (size - 1)**2
        self.triangles = []
        self.vertices_count = size**2
        self.fixed_vertices = []
        self.se = 0.5 * np.matrix([[2, -1, -1],
                                   [-1, 1, 0],
                                   [-1, 0, 1]])
        self.me = 1/24 * np.matrix([[2, 1, 1],
                                    [1, 2, 1],
                                    [1, 1, 2]])
        self.stot = np.zeros([self.vertices_count, self.vertices_count])
        self.mtot = np.zeros([self.vertices_count, self.vertices_count])

    def triangulate(self):
        """Divide plate into triangular mesh, mark fixed vertices"""
        for i in range(self.triangles_count):
            is_upper = i % 2
            row = i // self.triangles_in_row
            column = (i % self.triangles_in_row) // 2

            if is_upper:
                self.triangles.append((column + 1 + self.size * row,
                       column + self.size * row,
                       column + 1 + self.size * (row + 1)))
            else:
                self.triangles.append((column + self.size * (row + 1),
                       column + 1 + self.size * (row + 1),
                       column + self.size * row))

        center = self.size // 2
        self.fixed_vertices.append(center * self.size + center)

    def fill_matrices(self):
        for triangle in self.triangles:
            for i in (0, 1, 2):
                for j in (0, 1, 2):
                    self.stot[triangle[i], triangle[j]] += self.se[i, j]
                    self.mtot[triangle[i], triangle[j]] += self.me[i, j]

    def solve_system(self, n=-1):
        self.triangulate()
        self.fill_matrices()

        self.stot = np.delete(self.stot, self.fixed_vertices, axis=0)
        self.stot = np.delete(self.stot, self.fixed_vertices, axis=1)
        self.mtot = np.delete(self.mtot, self.fixed_vertices, axis=0)
        self.mtot = np.delete(self.mtot, self.fixed_vertices, axis=1)

        if n == -1:
            return eigh(self.stot, b=self.mtot)
        else:    
            return eigsh(self.stot, M=self.mtot, k=n, tol=0.01)


def set_submatrix(matrix, idx, subm):
    for i in range(subm.shape[0]):
        for j in range(subm.shape[1]):
            matrix[i+idx[0], j+idx[1]] = subm[i, j]
    return matrix


def increase_size(matrix, factor):
    if matrix.shape[0] != matrix.shape[1]:
        raise ValueError('Current version does not handle non-square matrices')
        
    s = matrix.shape[0]
    new_s = s + (s - 1) * factor
    out = np.zeros([new_s, new_s])
    element_shape = (factor + 2, factor + 2)
    for i in range(matrix.shape[0]-1):
        for j in range(matrix.shape[1]-1):
            e = np.zeros(element_shape)
            e[0, 0] = matrix[i, j]
            e[-1, 0] = matrix[i+1, j]
            e[0, -1] = matrix[i, j+1]
            e[-1, -1] = matrix[i+1, j+1]
            e = interpolate(e, lu='L')
            e = interpolate(e, lu='U')
            out = set_submatrix(out, (i*(factor+1), j*(factor+1)), e)
            
    return out
