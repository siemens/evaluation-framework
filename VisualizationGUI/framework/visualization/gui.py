# -*- coding: utf-8 -*-
"""
© Siemens AG, 2020
Author: Michael Dyck (m.dyck@gmx.net)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
"""

from framework.data.storage import DataStorage
from framework.data import parser
from framework.visualization.GUIhelpers.helpers import (EvaluationParameterSlider,
                                                        VerticalScrolledFrame, 
                                                        ValueEntry, 
                                                        NestedAxisDropdown)

import tkinter as tk
from matplotlib.backends.backend_tkagg import (
    FigureCanvasTkAgg, NavigationToolbar2Tk)
from matplotlib.figure import Figure
from matplotlib.collections import PathCollection
# This import registers the 3D projection, but is otherwise unused.
from mpl_toolkits.mplot3d import Axes3D  # noqa: F401 unused import

import subprocess

"""
GUI application 
"""
class GUIRoot:
    def __init__(self, master):
        """
        MainWindow Config
        """
        self.master = master
        self.master.geometry("{}x{}".format(1400, 1000))
        self.master.title("Evaluation Framework Visualization")
        background_col = 'slate gray' #'#4D4D4D' 
        self.master.configure(background=background_col)
        self.center()           
        
        """
        Get Data
        """
        self.get_data()
        self.default = [self.df.columns[0], self.df.columns[1],
                        self.df.columns[1]] # default axis
        self.x_axis = self.df[self.default[0]]
        self.y_axis = self.df[self.default[1]]
        self.z_axis = self.df[self.default[2]]
        self.color_axis = self.df[self.default[2]]
        self.selected = set(range(0, len(self.x_axis)))
        self.selected_vals_dict = dict(zip(self.evalparam_names, 
                                           [self.selected]*len(self.evalparam_names)))
        self.axis_names_dict = {"x": self.default[0], "y": self.default[1],
                                "z": self.default[2],
                                "color": self.default[2]}

        """
        Setup Main Window Frames
        """
        self.setup_window_frames()
        
        """
        Dropdown Components
        """
        self.setup_dropdown_components()
        
        """
        Slider Components
        """
        self.setup_slider_components()
        
        """ 
        Add solution launch button
        """
        b = tk.Button(self.launch_frame, text="Launch Current Solution", 
                      command=self.launch_solution, font='Helvetica 12 bold', 
                      bg="white", bd=2)
        b.pack()
        
        self.check_var = tk.StringVar(value="2D")
        check = tk.Checkbutton(self.launch_frame, text="3D Plot",
                               font="Helvetica 12 bold", bg="white", bd=2,
                               onvalue="3D", offvalue="2D", 
                               variable=self.check_var, 
                               command=self.update_plot_shape)
        check.pack()
        
        """ 
        Draw Matplotlib Plot
        """
        self.draw_figure()
                
    def setup_window_frames(self):
        tk.Grid.rowconfigure(self.master, 0, weight=1)
        tk.Grid.columnconfigure(self.master, 0, weight=1)
        
        """
        Main PanedWindow
        """
        self.pw = tk.PanedWindow(self.master, orient=tk.VERTICAL, 
                                 width=self.master.winfo_width(), 
                                 height=self.master.winfo_height(), 
                                 bg="white")
        self.pw.grid(row=0, column=0, sticky=tk.N+tk.W+tk.E+tk.S)

        """
        MAIN FRAMES
        """
        self.top_frame = tk.Frame(self.master, bg="white", 
                                  width=self.master.winfo_width(), 
                                  height=0.6*self.master.winfo_height())
        self.bot_frame = tk.Frame(self.master, bg="slate gray", 
                                  width=self.master.winfo_width(), 
                                  height=0.4*self.master.winfo_height())
        
        tk.Grid.rowconfigure(self.top_frame, 0, weight=1)
        tk.Grid.columnconfigure(self.top_frame, 0, weight=1)
        
        tk.Grid.rowconfigure(self.bot_frame, 0, weight=1)
        tk.Grid.columnconfigure(self.bot_frame, 0, weight=1)
        
        self.top_frame.update_idletasks() 
        self.bot_frame.update_idletasks() 
        self.pw.add(self.top_frame)
        self.pw.add(self.bot_frame)
        self.top_frame.update_idletasks() 
        self.bot_frame.update_idletasks() 
        
        self._after_id = None
        self.top_frame.bind('<Configure>', self.resize_window)
        
        """
        FIGURE FRAME
        """
        self.figure_frame2D = tk.Frame(self.top_frame, bg="white", 
                                     width=self.top_frame.winfo_width(), 
                                     height=self.top_frame.winfo_height(),
                                     bd=2, relief=tk.RIDGE)
        self.figure_frame2D.grid(row=0, column=0, sticky=tk.W+tk.N+tk.S+tk.E)
        self.figure_frame3D = tk.Frame(self.top_frame, bg="white", 
                                       width=self.top_frame.winfo_width(), 
                                       height=self.top_frame.winfo_height(),
                                       bd=2, relief=tk.RIDGE)
        self.figure_frame3D.grid(row=0, column=0, sticky=tk.W+tk.N+tk.S+tk.E)
        self.figure_frame3D.grid_forget()
        
        """
        SLIDER FRAME
        """
        self.slider_frame = VerticalScrolledFrame(self.bot_frame, bg="slate gray", 
                                                  width=self.bot_frame.winfo_width(), 
                                                  height=self.bot_frame.winfo_height(),
                                                  bd=2, relief=tk.RIDGE)
        self.slider_frame.grid(row=0, column=0, sticky=tk.W+tk.N+tk.S+tk.E)
         
        """
        DROPDOWN FRAMES
        """
        self.xdropdown_frame = tk.Frame(self.top_frame, bg="white", 
                                        width=0.1*self.top_frame.winfo_width(), 
                                        height=0.05*self.top_frame.winfo_height())
        self.ydropdown_frame = tk.Frame(self.top_frame, bg="white", 
                                        width=0.1*self.top_frame.winfo_width(), 
                                        height=0.05*self.top_frame.winfo_height())
        self.zdropdown_frame = tk.Frame(self.top_frame, bg="white", 
                                        width=0.1*self.top_frame.winfo_width(), 
                                        height=0.05*self.top_frame.winfo_height())
        self.color_frame = tk.Frame(self.top_frame, bg="white", 
                                    width=0.1*self.top_frame.winfo_width(), 
                                    height=0.05*self.top_frame.winfo_height())
        
        self.xdropdown_frame.grid(row=0, column=0, sticky=tk.S+tk.E, 
                                  padx=4, pady=4)
        self.ydropdown_frame.grid(row=0, column=0, sticky=tk.W+tk.N, 
                                  padx=4, pady=4)
        self.zdropdown_frame.grid(row=0, column=0, sticky=tk.S, 
                                  padx=4, pady=4)
        self.zdropdown_frame.grid_forget()
        self.color_frame.grid(row=0, column=0, sticky=tk.N+tk.E, 
                              padx=4, pady=4)
        
        """ 
        TOOLBAR FRAME
        """
        self.toolbar_frame2D = tk.Frame(self.top_frame, bg="white",
                                          width=0.2*self.top_frame.winfo_width(), 
                                          height=0.1*self.top_frame.winfo_height())
        self.toolbar_frame2D.grid(row=0, column=0, sticky=tk.W+tk.S, 
                                  padx=4, pady=4)
        self.toolbar_frame3D = tk.Frame(self.top_frame, bg="white",
                                          width=0.2*self.top_frame.winfo_width(), 
                                          height=0.1*self.top_frame.winfo_height())
        self.toolbar_frame3D.grid(row=0, column=0, sticky=tk.W+tk.S, 
                                  padx=4, pady=4)
        self.toolbar_frame3D.grid_forget()
        
        """
        BUTTON FRAME
        """
        self.launch_frame = tk.Frame(self.master, bg="white", 
                                      width=0.2*self.figure_frame2D.winfo_width(), 
                                      height=0.05*self.figure_frame2D.winfo_height())
        self.launch_frame.grid(row=0, column=0, sticky=tk.N, 
                                  padx=4, pady=4)
        
    def resize_window(self, event):
        self.plot.cla()
        self.slider_frame.grid_remove()
        if self._after_id:
            self.top_frame.after_cancel(self._after_id)
        self._after_id = self.top_frame.after(200, self.redraw_content)
        
    def redraw_content(self):
        self.update_current_solution()
        self.slider_frame.grid()
        
    def get_data(self):
        self.df = DataStorage.get_data()
        self.evalparam_names = DataStorage.get_parameter_names()
        self.objval_names = DataStorage.get_objective_names()
        self.array_names = DataStorage.get_array_names()
        
    def center(self):
        self.master.update_idletasks()

        # Tkinter way to find the screen resolution
        screen_width = self.master.winfo_screenwidth()
        screen_height = self.master.winfo_screenheight()

        size = tuple(int(_) for _ in self.master.geometry().split('+')[0].split('x'))
        x = screen_width/2 - size[0]/2
        y = screen_height/2 - size[1]/2

        self.master.geometry("+%d+%d" % (x, y)) 
        
    def draw_figure(self):
        self.fig2D = Figure(figsize=(self.figure_frame2D.winfo_width()/100, 
                                     self.figure_frame2D.winfo_height()/100), 
                            dpi=100)
        self.fig3D = Figure(figsize=(self.figure_frame2D.winfo_width()/100, 
                                     self.figure_frame2D.winfo_height()/100), 
                            dpi=100)
        
        self.canvas2D = FigureCanvasTkAgg(self.fig2D, self.figure_frame2D) 
        self.canvas3D = FigureCanvasTkAgg(self.fig3D, self.figure_frame3D)
        
        self.plot2D = self.fig2D.add_subplot(111)
        self.plot3D = self.fig3D.add_subplot(111, projection='3d')
        
        self.colorbar = None
        
        self.current_solution = self.df.iloc[0]
        # second time necessary, since slicing a single row in pandas corrupts
        # the datatype, if they are different within the row (ints -> floats)
        self.current_solution_dt = [self.df[col][0] for col in self.df.columns]
        self.current_solution_ind = 0
        
        self.canvas2D.mpl_connect('pick_event', self.onpick)
        self.canvas3D.mpl_connect('pick_event', self.onpick)
        
        self.update_current_solution()
                
        self.toolbar2D = NavigationToolbar2Tk(self.canvas2D, 
                                              self.toolbar_frame2D)
        self.toolbar2D.update()
        self.canvas2D.get_tk_widget().pack(side=tk.TOP, fill=tk.BOTH, expand=True)
        
        self.toolbar3D = NavigationToolbar2Tk(self.canvas3D, 
                                              self.toolbar_frame3D)
        self.toolbar3D.update()
        self.canvas3D.get_tk_widget().pack(side=tk.TOP, fill=tk.BOTH, expand=True)
        
    def plot_data(self):
        # colormaps: https://matplotlib.org/examples/color/colormaps_reference.html
        self.deselected = list(set(self.df.index.values).difference(self.selected))
        
        if self.check_var.get() == "3D":
            self.plot.scatter(self.x_axis[self.deselected], 
                              self.y_axis[self.deselected],
                              self.z_axis[self.deselected],
                              c='grey', s=100, alpha=0.1, picker=1)
            im = self.plot.scatter(self.x_axis[list(self.selected)], 
                                   self.y_axis[list(self.selected)], 
                                   self.z_axis[list(self.selected)], 
                                    # alpha=1, s=100, picker=1)
                                    c=self.color_axis[list(self.selected)], 
                                    cmap="rainbow", alpha=1, s=100, picker=1)
        else:
            self.plot.scatter(self.x_axis[self.deselected], 
                              self.y_axis[self.deselected],
                              c='grey', s=100, alpha=0.1, picker=1)
            im = self.plot.scatter(self.x_axis[list(self.selected)], 
                                   self.y_axis[list(self.selected)],
                                    # alpha=1, s=100, picker=1)
                                    c=self.color_axis[list(self.selected)], 
                                    cmap='rainbow', alpha=1, s=100, picker=1)
        
        self.master.update_idletasks()
        """
        labels, legends, ticks
        """
        if self.colorbar is not None:
            self.colorbar.remove()
        self.colorbar = self.fig.colorbar(im, ax=self.plot)
        self.colorbar.ax.tick_params(labelsize=15)
        self.colorbar.ax.set_ylabel(self.axis_names_dict["color"], fontsize=15, 
                                    rotation=270, labelpad=30)
        
        self.plot.set_xlabel(self.axis_names_dict["x"], fontsize=15, labelpad=10)
        self.plot.set_ylabel(self.axis_names_dict["y"], fontsize=15, labelpad=10)
        
        if self.check_var.get() == "3D":
            self.plot.set_xlabel(self.axis_names_dict["x"], fontsize=15, labelpad=15)
            self.plot.set_ylabel(self.axis_names_dict["y"], fontsize=15, labelpad=15)
            self.plot.set_zlabel(self.axis_names_dict["z"], fontsize=15, labelpad=15)
            xmin = min(self.df[self.axis_names_dict["x"]])
            xmax = max(self.df[self.axis_names_dict["x"]])
            ymin = min(self.df[self.axis_names_dict["y"]])
            ymax = max(self.df[self.axis_names_dict["y"]])
            zmin = min(self.df[self.axis_names_dict["z"]])
            zmax = max(self.df[self.axis_names_dict["z"]])
            self.plot.set_xlim([xmin, xmax])
            self.plot.set_ylim([ymin, ymax])
            self.plot.set_zlim([zmin, zmax])
            
        self.plot.tick_params(labelsize=15)
        
    def onpick(self, event):
        if isinstance(event.artist, PathCollection):
            # index of selected point
            ind = event.ind[0]
            alpha = event.artist.get_alpha()
            if (alpha == 1):
                val = list(self.selected)[ind]
            else:
                val = self.deselected[ind]
            self.current_solution = self.df.loc[val]
            self.current_solution_dt = [self.df[col][val] for col in self.df.columns]
            self.current_solution_ind = val
            self.update_current_solution()
            
    def update_current_solution(self):
        x_ax = self.axis_names_dict["x"]
        y_ax = self.axis_names_dict["y"]
        z_ax = self.axis_names_dict["z"]
        x_val = self.current_solution[x_ax]
        y_val = self.current_solution[y_ax]
        z_val = self.current_solution[z_ax]
        
        self.plot2D.cla()
        self.plot3D.cla()
        
        if self.check_var.get() == "3D":
            self.fig = self.fig3D
            self.canvas = self.canvas3D
            self.plot = self.plot3D
        else:
            self.fig = self.fig2D
            self.canvas = self.canvas2D
            self.plot = self.plot2D
            
            
        """
        circle current solution in plot
        """
        self.plot_data()
        if self.check_var.get() == "3D":
            self.plot.scatter(x_val, y_val, z_val, facecolors='k', edgecolors='k',
                              linewidth=3, s=150, alpha=1)
        else:
            self.plot.scatter(x_val, y_val, facecolors='none', edgecolors='k',
                          linewidth=3, s=150, alpha=1)
            
        self.plot3D.mouse_init()
        self.canvas.draw()
        """
        update slider position and entry value of current solution
        """
        for param in self.df.columns:
            slider = self.slider_dict[param]
            val = self.current_solution[param]
            pos = slider.position_values[slider.values.index(val)]
            slider.update_cursor_pos("current_solution", pos)
            self.currentsolution_dict[param].config(text=val)

    def launch_solution(self):
        evaluation_name = DataStorage.get_evaluation_name()
        use_ROS = DataStorage.get_useROS_bool()
        time_scale = DataStorage.get_time_scale()
        framework_path = parser.get_path_to_objectives(evaluation_name, use_ROS).parents[2]
        sub_dirs = ["bin", "Release", "netcoreapp3.1"]
        exe_path = framework_path / sub_dirs[0] / sub_dirs[1] / sub_dirs[2]
        if (use_ROS):
            exe_name = "EvaluationFrameworkROS.exe"
        else:
            exe_name = "EvaluationFramework.exe"
        args = str(exe_path / exe_name)
        """ 
        send id of currently selected parameter set 
        (i.e. of current solution)
        """
        args += " --demonstration -timeScale {0}".format(time_scale)
        args += " -evaluationName {0}".format(evaluation_name)
        args += " -parameterID {0}".format(self.current_solution_ind)
        
        subprocess.run(args, capture_output=False)
        
    def setup_dropdown_components(self):
        self.x_Dropdown = NestedAxisDropdown(self.xdropdown_frame, self.df.columns, 
                                       self.array_names, self.default[0], "x")
        self.x_Dropdown.label.pack(side=tk.LEFT)
        self.x_Dropdown.popupMenu.pack(side=tk.LEFT, anchor=tk.NE)
        
        self.y_Dropdown = NestedAxisDropdown(self.ydropdown_frame, self.df.columns,
                                       self.array_names, self.default[1], "y")
        self.y_Dropdown.label.pack(side=tk.LEFT)
        self.y_Dropdown.popupMenu.pack(side=tk.LEFT)
        
        self.z_Dropdown = NestedAxisDropdown(self.zdropdown_frame, self.df.columns,
                                       self.array_names, self.default[1], "z")
        self.z_Dropdown.label.pack(side=tk.LEFT)
        self.z_Dropdown.popupMenu.pack(side=tk.LEFT)
        
        self.color_Dropdown = NestedAxisDropdown(self.color_frame, self.df.columns, 
                                           self.array_names, self.default[1], "color")
        self.color_Dropdown.label.pack(side=tk.LEFT)
        self.color_Dropdown.popupMenu.pack(side=tk.LEFT)
        
        # link function to change dropdown
        self.x_Dropdown.tkvar.trace('w', lambda a,b,c: 
                                    self.on_dropdown_change(tkvar=self.x_Dropdown.tkvar,
                                                            axis=self.x_Dropdown.axis))
        self.y_Dropdown.tkvar.trace('w', lambda a,b,c: 
                                    self.on_dropdown_change(tkvar=self.y_Dropdown.tkvar,
                                                            axis=self.y_Dropdown.axis))
        self.z_Dropdown.tkvar.trace('w', lambda a,b,c: 
                                    self.on_dropdown_change(tkvar=self.z_Dropdown.tkvar,
                                                            axis=self.z_Dropdown.axis))
        self.color_Dropdown.tkvar.trace('w', lambda a,b,c: 
                                        self.on_dropdown_change(tkvar=self.color_Dropdown.tkvar,
                                                                axis=self.color_Dropdown.axis))
            
    def on_dropdown_change(self, *args, tkvar, axis):
            val = self.df[tkvar.get()]
            if axis == "x":
                self.x_axis = val
                self.axis_names_dict["x"] = tkvar.get()
            elif axis == "y":
                self.y_axis = val  
                self.axis_names_dict["y"] = tkvar.get()
            elif axis == "z":
                self.z_axis = val
                self.axis_names_dict["z"] = tkvar.get()
            elif axis == "color":
                self.color_axis = val
                self.axis_names_dict["color"] = tkvar.get()
            self.plot_data()
            self.update_current_solution()
        
    def setup_slider_components(self):
        self.slider_dict = {}
        self.lowerentry_dict = {}
        self.upperentry_dict = {}
        self.currentsolution_dict = {}
        for i, param in enumerate(self.df.columns):
            lbl = tk.Label(self.slider_frame.interior, text=param, font='Helvetica 12 bold',
                           bg="slate gray")
            lbl.grid(row=i, column=0, padx=10, sticky=tk.W+tk.E)
            
            self.slider_dict[param] = EvaluationParameterSlider(self.slider_frame.interior, 
                                                                self.df, 
                                                                param,
                                                                self.on_slider_change)
            self.lowerentry_dict[param] = ValueEntry(self.slider_frame.interior,
                                                     self.on_entry_change, 
                                                     "center", "lower", i, 1,
                                                     param)
            self.slider_dict[param].grid(row=i, column = 2, padx=10, pady=10)
            self.upperentry_dict[param] = ValueEntry(self.slider_frame.interior,
                                                     self.on_entry_change,
                                                     "center", "upper", i, 3,
                                                     param)
            lbl = tk.Label(self.slider_frame.interior,
                           text=self.slider_dict[param].values[0], 
                           font='Helvetica 12 bold',
                           fg="red", bg="white")
            lbl.grid(row=i, column=4, padx=10, sticky=tk.W+tk.E)
            self.currentsolution_dict[param] = lbl
            
            self.lowerentry_dict[param].set_default(self.slider_dict[param].tkvar_values_dict["cursor_l"])
            self.upperentry_dict[param].set_default(self.slider_dict[param].tkvar_values_dict["cursor_u"])
            
    def on_slider_change(self, *args):
        tkvar = args[0]
        param = None
        for slider in self.slider_dict.values():
            for var in slider.tkvar_dict.values():
                if str(var) == tkvar:
                    param = slider.evalparam
                    break
        
        borders = list(self.slider_dict[param].tkvar_values_dict.values())
        lower_ind = self.slider_dict[param].values.index(borders[0]) 
        upper_ind = self.slider_dict[param].values.index(borders[-1])
        if (lower_ind < upper_ind):
            values = self.slider_dict[param].values[lower_ind:upper_ind+1]
        else:
            values = self.slider_dict[param].values[upper_ind:lower_ind+1]
            
        self.lowerentry_dict[param].update(min(values))
        self.upperentry_dict[param].update(max(values))
        
        selection = set(self.df[param][self.df[param].isin(values)].index)
        self.selected_vals_dict[param] = selection
        self.selected = set.intersection(*self.selected_vals_dict.values())
        self.plot_data()
        self.update_current_solution()
        
    def on_entry_change(self, *args, entry, tkvar, location, param):
        value = 0
        slider = self.slider_dict[param]
        if (location == "lower"):
            cursor = "cursor_l"
        elif (location == "upper"):
            cursor = "cursor_u"
        
        try:
            value = float(entry.tkvar.get())
        except ValueError:
            self.lowerentry_dict[param].update(slider.tkvar_values_dict[cursor])
            return
        
        # get possible value closest to entered value
        closest = min(list(slider.values), 
                      key=lambda x:abs(x-value))
        slider.tkvar_values_dict[cursor] = closest
        slider.tkvar_dict[cursor].set(closest)
        new_pos = slider.position_values[slider.values.index(closest)]
        slider.update_cursor_pos(cursor, new_pos)
        
    def update_plot_shape(self):
        if self.check_var.get() == "2D":
            self.figure_frame3D.grid_forget()
            self.figure_frame2D.grid(row=0, column=0, 
                                     sticky=tk.W+tk.N+tk.S+tk.E)
            self.toolbar_frame2D.grid(row=0, column=0, sticky=tk.W+tk.S, 
                                      padx=4, pady=4)
            self.toolbar_frame3D.grid_forget()
            # hide z-axis dropdown
            self.zdropdown_frame.grid_forget()
        elif self.check_var.get() == "3D":
            self.figure_frame2D.grid_forget()
            self.figure_frame3D.grid(row=0, column=0, 
                                     sticky=tk.W+tk.N+tk.S+tk.E)
            self.toolbar_frame3D.grid(row=0, column=0, sticky=tk.W+tk.S, 
                                      padx=4, pady=4)
            self.toolbar_frame2D.grid_forget()
            # show z-axis dropdown
            self.zdropdown_frame.grid(row=0, column=0, sticky=tk.S, 
                                      padx=4, pady=4)
        
        self.update_current_solution()
        
        
def run_gui(evaluation_name, use_ROS, time_scale):
    # fill data
    DataStorage.fill_data_storage(evaluation_name, use_ROS, time_scale)
    
    root = tk.Tk()
    gui = GUIRoot(root)
    # start main GUI loop
    root.mainloop()
