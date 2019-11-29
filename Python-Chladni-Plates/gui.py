from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import *
from matplotlib.backends.backend_qt5agg import FigureCanvasQTAgg
from matplotlib.figure import Figure
import matplotlib.pyplot as plt

import threading as thr
from algorithm import *


class App(QWidget):
    def __init__(self, parent=None):
        super().__init__(parent)

        self.size = 50
        self.vals = None
        self.vecs = None
        self.at_freq = None
        self.at_size = None
        self.at_center = None
        self.center_x = self.size//2
        self.center_y = self.size//2
        
        self.precision_label = QLabel('Precision', self)
        self.precision_value = QLabel('', self)
        self.precision_slider = self.make_slider(2, 7, 10, self.precision_value)
        self.precision_slider.sliderReleased.connect(self.precision_change)
        self.precision_slider.setValue(5)
        
        self.max_freq = 1
        self.frequency_label = QLabel('Eigenmode number', self)
        self.frequency_value = QLabel('', self)
        self.frequency_slider = self.make_slider(1, self.max_freq, 1, self.frequency_value)
        self.frequency_slider.sliderReleased.connect(self.generate)
        self.frequency_slider.setValue(1)
        
        self.pos_x_label = QLabel('X position', self)
        self.pos_x_value = QLabel('', self)
        self.pos_x_slider = self.make_slider(1, 100, 1, self.pos_x_value)
        self.pos_x_slider.sliderReleased.connect(lambda: self.set_center_x(self.size*self.pos_x_slider.value()//100))
        self.pos_x_slider.setValue(50)
        
        self.pos_y_label = QLabel('Y position', self)
        self.pos_y_value = QLabel('', self)
        self.pos_y_slider = self.make_slider(1, 100, 1, self.pos_y_value)
        self.pos_y_slider.sliderReleased.connect(lambda: self.set_center_y(self.size*self.pos_y_slider.value()//100))
        self.pos_y_slider.setValue(50)

        self.generate_button = QPushButton('&Generate', self)
        self.generate_button.clicked.connect(self.generate)

        self.figure = Figure(frameon=False)
        self.canvas = FigureCanvasQTAgg(self.figure)
        self.ax = self.figure.add_subplot(111)
        self.canvas_lock = thr.Lock()
        self.thread = None

        self.grid = QGridLayout()
        self.grid.addWidget(self.precision_label, 0, 0)
        self.grid.addWidget(self.precision_slider, 1, 0)
        self.grid.addWidget(self.precision_value, 2, 0)
        
        self.grid.addWidget(self.frequency_label, 3, 0)
        self.grid.addWidget(self.frequency_slider, 4, 0)
        self.grid.addWidget(self.frequency_value, 5, 0)
        
        self.grid.addWidget(self.pos_x_label, 6, 0)
        self.grid.addWidget(self.pos_x_slider, 7, 0)
        self.grid.addWidget(self.pos_x_value, 8, 0)
        
        self.grid.addWidget(self.pos_y_label, 9, 0)
        self.grid.addWidget(self.pos_y_slider, 10, 0)
        self.grid.addWidget(self.pos_y_value, 11, 0)
        
        self.grid.addWidget(self.generate_button, 12, 0)
        self.grid.addWidget(self.canvas, 0, 1, 12, 2)

        self.setLayout(self.grid)
        self.setGeometry(20, 20, 900, 500)
        self.setWindowTitle('Chladni plate simulator')
        self.show()

    def make_slider(self, minimum, maximum, step, label):
        slider = QSlider(Qt.Horizontal)
        slider.setFocusPolicy(Qt.StrongFocus)
        # slider.setSizePolicy(QSizePolicy.Expanding, QSizePolicy.Expanding)
        slider.setTickPosition(QSlider.TicksAbove)
        slider.setTickInterval(1)
        slider.setMinimum(minimum)
        slider.setMaximum(maximum)
        slider.valueChanged.connect(lambda val: self.slider_change(val*step, label))
        return slider

    def slider_change(self, val, label):
        label.setText(str(val))
        
    def precision_change(self):
        self.size = self.precision_slider.value()*10
        self.center_x = self.size*self.pos_x_slider.value()//100
        self.center_y = self.size*self.pos_y_slider.value()//100

    def set_center_y(self, val):
        self.center_y = val 
       
    def set_center_x(self, val):
        self.center_x = val 

    def generate(self):
        if self.thread is not None:
            self.thread.join()
            
        if self.vecs is None:
            self.thread = thr.Thread(target=self.gen_and_draw)
            self.thread.start()
        elif self.at_size != self.precision_slider.value() or self.at_center != (self.center_x, self.center_y):
            self.frequency_slider.setValue(1)
            self.thread = thr.Thread(target=self.gen_and_draw)
            self.thread.start()
            self.at_size = self.precision_slider.value()
            self.at_center = (self.center_x, self.center_y)
        elif self.at_freq != self.frequency_slider.value():
            print('value = {}'.format(self.vals[self.frequency_slider.value()]))
            data = self.prepare_data(self.vecs, self.frequency_slider.value())
            self.draw_plot(data, self.size)
            self.at_freq = self.frequency_slider.value()
        
    def draw_plot(self, raw_data, size):
        center = (size//2)*(size+1)
        vals = np.abs(raw_data.reshape([size, size]))
        vals *= 1 / vals.max()
        intp = np.power(increase_size(vals, 5), 1/3)
        
        self.canvas_lock.acquire()
        self.ax.clear()
        self.ax.axis('off')
        self.ax.imshow(intp, interpolation='nearest', cmap=plt.get_cmap('Greys'))
        self.canvas.draw()
        self.canvas_lock.release()
        
    def gen_and_draw(self):
        self.vals, self.vecs = Algorithm(self.size, (self.center_x-1, self.center_y-1)).solve_system()
        for i, v in enumerate(self.vals):
            if v >= 0.5:
                self.max_freq = i-1
                self.frequency_slider.setMaximum(self.max_freq)
                break
        data = self.prepare_data(self.vecs, 1)
        self.draw_plot(data, self.size)
        print('value = {}'.format(self.vals[1]))
        
    def prepare_data(self, vecs, i):
        size = len(vecs[:, i])
        center = (size//2)*(size+1)
        vec = np.array(vecs[:, i][:center])
        vec = np.append(vec, [0])
        vec = np.append(vec, np.array(vecs[:, i][center:]))
        return vec
       
