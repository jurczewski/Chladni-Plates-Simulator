import sys
import numpy as np


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
