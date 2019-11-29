import numpy as np
from scipy.sparse.linalg import eigsh
from scipy.linalg import eigh


class Point:
    def __init__(self, x, y, z=None):
        self.x = x
        self.y = y
        self.z = z

    @property
    def xy(self):
        return self.x, self.y

    def __repr__(self):
        return 'Point({}, {}, {})'.format(self.x, self.y, self.z)


def interpolate(matrix, lu):
    if matrix.shape[0] != matrix.shape[1]:
        raise ValueError('Matrix not square')


    p1 = Point(0, 0, matrix[0, 0])
    p2 = Point(matrix.shape[0] - 1, matrix.shape[1] - 1, matrix[-1, -1])
    p3 = None
    if lu == 'L':
        p3 = Point(matrix.shape[0]-1, 0, matrix[-1, 0])
    elif lu == 'U':
        p3 = Point(0, matrix.shape[1]-1, matrix[0, -1])
    else:
        raise ValueError('Argument "lu" must have value "L" or "U"')

    v1 = (p2.x - p1.x, p2.y - p1.y, p2.z - p1.z)
    v2 = (p3.x - p1.x, p3.y - p1.y, p3.z - p1.z)

    A, B, C = v1[1]*v2[2] - v2[1]*v1[2], - v1[0]*v2[2] + v2[0]*v1[2], v1[0]*v2[1] - v2[0]*v1[1]
    D = -1 * (A * p1.x + B * p1.y + C * p1.z)
    z = lambda x, y: (x * A + y * B + D) / (-C)

    for i in range(matrix.shape[0]):
        r = None
        if lu == 'L':
            r = range(i+1)
        else:
            r = range(i, matrix.shape[1])

        for j in r:
            pos = (i, j)
            if pos == p1.xy or pos == p2.xy or pos == p3.xy:
                continue

            matrix[i, j] = z(i, j)

    return matrix


class Algorithm:
    def __init__(self, size, center = None):
        self.size = size
        self.triangles_in_row = 2 * (size - 1)
        self.triangles_count = 2 * (size - 1)**2
        self.triangles = []
        self.vertices_count = size**2
        self.fixed_vertices = [center[0] * self.size + center[1]]
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

    def fill_matrices(self):
        for triangle in self.triangles:
            for i in (0, 1, 2):
                for j in (0, 1, 2):
                    self.stot[triangle[i], triangle[j]] += self.se[i, j]
                    self.mtot[triangle[i], triangle[j]] += self.me[i, j]

    def solve_system(self):
        self.triangulate()
        self.fill_matrices()

        self.stot = np.delete(self.stot, self.fixed_vertices, axis=0)
        self.stot = np.delete(self.stot, self.fixed_vertices, axis=1)
        self.mtot = np.delete(self.mtot, self.fixed_vertices, axis=0)
        self.mtot = np.delete(self.mtot, self.fixed_vertices, axis=1)

        return eigh(self.stot, b=self.mtot)


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
