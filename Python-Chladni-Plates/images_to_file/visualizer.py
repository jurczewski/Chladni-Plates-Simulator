import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from algorithm import *

# dir = sys.argv[1]
#
# df_vec = pd.read_csv('{}/eigenvectors.csv'.format(dir), header=None)
# df_val = pd.read_csv('{}/eigenvalues.csv'.format(dir), header=None)
size = 50
a = Algorithm(size)
df_val, df_vec = a.solve_system()


center = (size//2)*(size+1)
for i in range(20):
    # vals = np.array(df_vec.iloc[:, i][:center])
    # vals = np.append(vals, [0])
    # vals = np.append(vals, np.array(df_vec.iloc[:, i][center:]))
    vals = np.array(df_vec[:, i][:center])
    vals = np.append(vals, [0])
    vals = np.append(vals, np.array(df_vec[:, i][center:]))
    print('min: {}, max: {}'.format(vals.min(), vals.max()))
    vals = np.abs(vals.reshape([size, size]))
    vals *= 1 / vals.max()
    intp = np.power(increase_size(vals, 5), 1/2)

    plt.figure()
    plt.imshow(intp, interpolation='nearest', cmap=plt.get_cmap('Greys'))
    plt.savefig('chladni{}.png'.format(i))
