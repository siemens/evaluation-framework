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

import tkinter as tk
import math
import numpy as np

"""
Class NestedAxisDropdown adapted from Onkar Raut's answer on 
https://stackoverflow.com/questions/24357256/are-there-any-tkinter-widgets-like-optionmenu-that-can-be-nested
"""
class NestedAxisDropdown():
    def __init__(self, master, param_names, array_names, default, axis):
        self.master = master
        self.param_names = param_names
        self.array_names = array_names
        self.axis = axis
        self.default = default
        
        def create_menu(top_info, top_menu, value_var):
            if isinstance(top_info, dict):
                for key in top_info.keys():
                    menu = tk.Menu(top_menu)
                    if len(top_info[key]) == 0:
                        top_menu.add_radiobutton(label=key, variable=value_var)
                    else:
                        top_menu.add_cascade(label=key, menu=menu)
                        create_menu(top_info[key], menu, value_var)
                return
            else:
                for value in top_info:
                    top_menu.add_radiobutton(label=value, variable=value_var,
                                             value=value)
                return
        
        # Create a Tkinter variable
        self.tkvar = tk.StringVar()
        self.tkvar.set(self.default) # set the default option
        
        self.base_names = []
        self.array_names_dict = dict((name, []) for name in self.array_names)
        for array in self.array_names:
            name, unit = array.split(" ")
            self.array_names_dict[array] = [param for param in self.param_names 
                                            if name in param]

        values = [item for sublist in self.array_names_dict.values() 
                  for item in sublist]
        self.base_names.extend([param for param in self.param_names 
                                    if param not in values])
        
        self.option_dict = dict((name, []) for name in self.base_names)
        self.option_dict.update(self.array_names_dict)

        self.popupMenu = tk.Menubutton(self.master, textvariable=self.tkvar, 
                                       indicatoron=True)
        self.topMenu = tk.Menu(self.popupMenu, tearoff=False)
        self.popupMenu.configure(menu=self.topMenu, font='Helvetica 12',
                                 relief=tk.RAISED, bd=3, bg="white")
        self.label = tk.Label(self.master, text="Choose {}-axis:".format(self.axis),
                              bg="white", font='Helvetica 16 bold')

        create_menu(self.option_dict, self.topMenu, self.tkvar)

class AxisDropdown():
    def __init__(self, master, param_names, array_names, default, axis):
        self.master = master
        self.param_names = param_names
        self.array_names = array_names
        self.axis = axis
        self.default = default
        self.setup_axis_dropdown()
        
    def setup_axis_dropdown(self):
        # Create a Tkinter variable
        self.tkvar = tk.StringVar()
        
        # Dictionary with options
        self.tkvar.set(self.default) # set the default option
        
        self.base_names = []
        self.array_names_dict = dict((name, []) for name in self.array_names)
        for array in self.array_names:
            name, unit = array.split(" ")
            self.array_names_dict[array] = [param for param in self.param_names 
                                            if name in param]

        values = [item for sublist in self.array_names_dict.values() 
                  for item in sublist]
        self.base_names.extend([param for param in self.param_names 
                                    if param not in values])
        
        self.option_dict = dict((name, []) for name in self.base_names)
        self.option_dict.update(self.array_names_dict)
                        
        self.popupMenu = tk.OptionMenu(self.master, self.tkvar, *self.option_dict)
        self.label = tk.Label(self.master, text="Choose {}-axis:".format(self.axis),
                              bg="white", font='Helvetica 16 bold')
        self.popupMenu.config(bg="white", font='Helvetica 12')
        self.popupMenu["menu"].config(bg="white")
        
class ValueEntry():
    def __init__(self, master, callback, justify, location, index, column, 
                 param, fontcolor="black", state="normal"):
        self.tkvar = tk.StringVar()
        self.param = param
        self.entry = tk.Entry(master, font='Helvetica 12', fg=fontcolor, 
                              justify=justify, textvariable=self.tkvar, 
                              state=state, width=15)
        self.entry.grid(row=index, column=column, padx=10)
        self.entry.bind('<Key-Return>', lambda event: callback(entry=self,
                                                               tkvar=self.tkvar, 
                                                               location=location, 
                                                               param=self.param))
            
    def set_default(self, value):
        self.entry.insert(0, value)
        
    def update(self, value):
        self.entry.delete(0, tk.END)
        self.entry.insert(0, value)
        
class EvaluationParameterSlider(tk.Canvas):
    def __init__(self, master, df, evalparam, callback, c_height=50, c_width=800,
                 s_height=25, s_width=800, **kwargs):
        self.master = master
        self.df = df
        self.evalparam = evalparam
        
        # canvas geometry
        self.c_height = c_height
        self.c_width = c_width
        # slider geometry
        self.s_height = s_height
        self.s_width = s_width
        # cursor geometry
        self.cursor_width = 5
        self.cursor_rectangle = 7
                
        tk.Canvas.__init__(self, master, width=c_width, height=c_height, 
                           **kwargs)
        
        self.setup_vals()
        self.draw_ticks()
        self.draw_cursor()
        
        self.tag_bind('cursor_l', '<B1-Motion>', self.on_move)
        self.tag_bind('cursor_u', '<B1-Motion>', self.on_move)
        self.tag_bind('cursor_l', '<ButtonRelease-1>', self.on_release)
        self.tag_bind('cursor_u', '<ButtonRelease-1>', self.on_release)
        
        # link function to change dropdown
        for tag in self.tkvar_dict.keys():
            self.tkvar_dict[tag].trace('w', lambda tkvar,_,__: 
                                       callback(tkvar))
        
    def setup_vals(self):
        self.values = sorted(set(self.df[self.evalparam]))
        self.min = self.values[0]
        self.max = self.values[-1]
        self.range = math.fabs(self.max-self.min)
        if len(self.values) == 1:
            self.tick = self.range
        else:
            self.tick = self.range/(len(self.values) - 1)
        # position values in pixels on canvas
        self.position_values = list(np.linspace(self.cursor_width-1, 
                                                self.s_width-1, 
                                                len(self.values)))
        
        self.tkvar_dict = {"cursor_l": tk.StringVar(), 
                           "cursor_u": tk.StringVar()}
        self.tkvar_values_dict = {"cursor_l": self.values[0], 
                                  "cursor_u": self.values[-1]}
        self.cursor_pos_dict = {"cursor_l": self.position_values[0],
                                "cursor_u": self.position_values[-1]}
        
    def draw_cursor(self):
        """Draw the cursor on val."""
        self.delete("cursor_l")
        self.delete("cursor_u")
        
        x_l = self.position_values[0]
        x_u = self.position_values[-1]

        self.create_rectangle(x_l, 0, x_u, self.s_height, 
                              outline="#f11", fill='#4D4D4D', width=0,
                              tags=('rect'))
        
        # lower cursor
        self.create_line(x_l, 0, x_l, self.s_height, width=self.cursor_rectangle, 
                         fill='white', tags="cursor_l")
        self.create_line(x_l, 0, x_l, self.s_height, width=self.cursor_width, 
                         tags="cursor_l")
        # upper cursor
        self.create_line(x_u, 0, x_u, self.s_height, width=self.cursor_rectangle, 
                         fill='white', tags="cursor_u")
        self.create_line(x_u, 0, x_u, self.s_height, width=self.cursor_width, 
                         tags="cursor_u")
        # current solution cursor
        self.create_line(x_l, self.s_height-10, x_l, self.s_height+10, 
                         width=self.cursor_width, 
                         fill='red', tags="current_solution")

    def on_move(self, event):
        """Make selection cursor follow the cursor."""
        x = event.x
        tag = self.gettags('current')[0]
        
        if x >= 0:
            xpos = min(max(abs(x), self.position_values[0]), 
                       self.position_values[-1])
            snap_xpos = min(self.position_values, 
                            key=lambda x:abs(x-float(xpos)))
            self.update_cursor_pos(tag, snap_xpos)
                
    def on_release(self, event):
        """ Update variable after mouse release. """
        tag = self.gettags('current')[0]
        pos = self.coords(self.find_withtag(tag)[0])[0]
        
        new_val = self.values[self.position_values.index(pos)]
        self.tkvar_values_dict[tag] = new_val
        self.tkvar_dict[tag].set(new_val)
    
    def update_background_col(self):
        self.coords(self.find_withtag('rect'),
                    self.cursor_pos_dict["cursor_l"], 0, 
                    self.cursor_pos_dict["cursor_u"], self.s_height)
        
    def draw_ticks(self):
        large_line = self.c_height - 5
        small_line = self.c_height - 15
        x_l = self.position_values[0]
        x_u = self.position_values[-1]
        
        self.create_line(x_l, self.s_height, x_l, large_line, width=3, 
                         fill='black', tags="tick")
        lbl_l = tk.Label(self, text="{}".format(self.values[0]),
                         font='Helvetica 10 bold')
        lbl_l.place(x=x_l+2, y=small_line)
        self.create_line(x_u, self.s_height, x_u, large_line, width=3, 
                         fill='black', tags="tick")
        lbl_u = tk.Label(self, text="{}".format(self.values[-1]),
                         font='Helvetica 10 bold')
        lbl_u.place(x=x_u, y=small_line)
        self.update_idletasks()
        lbl_u.place(x=x_u-(lbl_u.winfo_width()+1), y=small_line)
        
        max_ticks = 100
        length = len(self.position_values[1:-1])
        if(length<=max_ticks):
            for val in self.position_values[1:-1]:
                self.create_line(val, self.s_height, val, small_line,
                                 width=2, fill='black', tags="tick")
        else:
            step = int(round(length/max_ticks, 0))
            for val in self.position_values[1:-1:step]:
                self.create_line(val, self.s_height, val, small_line,
                                 width=2, fill='black', tags="tick")
                
    def update_cursor_pos(self, tag, pos):
        for s in self.find_withtag(tag):
            if tag == "current_solution":
                self.coords(s, pos, self.s_height-10, pos, self.s_height+10)
            else:
                self.coords(s, pos, 0, pos, self.s_height)
                self.cursor_pos_dict[tag] = pos
                self.update_background_col()
        
        
"""
Class VerticalScrolledFrame taken from Eugene Bakin's GitHub Gist on 
https://gist.github.com/bakineugene/76c8f9bcec5b390e45df
"""        
class VerticalScrolledFrame(tk.Frame):
    """A pure Tkinter scrollable frame that actually works!
    * Use the 'interior' attribute to place widgets inside the scrollable frame
    * Construct and pack/place/grid normally
    * This frame only allows vertical scrolling
    """
    def __init__(self, parent, *args, **kw):
        tk.Frame.__init__(self, parent, *args, **kw)            

        # create a canvas object and scrollbars for scrolling it
        vscrollbar = tk.Scrollbar(self, orient=tk.VERTICAL)
        vscrollbar.pack(fill=tk.Y, side=tk.RIGHT, expand=tk.FALSE)
        hscrollbar = tk.Scrollbar(self, orient=tk.HORIZONTAL)
        hscrollbar.pack(fill=tk.X, side=tk.BOTTOM, expand=tk.FALSE)
        background = kw.pop("bg")
        canvas = tk.Canvas(self, bd=0, highlightthickness=0,
                           yscrollcommand=vscrollbar.set,
                           xscrollcommand=hscrollbar.set,
                           height=kw.pop("height"),
                           width=kw.pop("width"),
                           bg=background)
        canvas.pack(side=tk.LEFT, fill=tk.BOTH, expand=tk.TRUE)
        vscrollbar.config(command=canvas.yview)
        hscrollbar.config(command=canvas.xview)

        # reset the view
        canvas.xview_moveto(0)
        canvas.yview_moveto(0)

        # create a frame inside the canvas which will be scrolled with it
        self.interior = interior = tk.Frame(canvas, bg=background)
        interior_id = canvas.create_window(0, 0, window=interior,
                                           anchor=tk.NW)

        # track changes to the canvas and frame width & height and sync them,
        # also updating the scrollbar
        def _configure_interior(event):
            # update the scrollbars to match the size of the inner frame
            size = (interior.winfo_reqwidth(), interior.winfo_reqheight())
            canvas.config(scrollregion="0 0 %s %s" % size)
            if interior.winfo_reqwidth() != canvas.winfo_width():
                # update the canvas's width to fit the inner frame
                canvas.config(width=interior.winfo_reqwidth())
            if interior.winfo_reqheight() != canvas.winfo_height():
                # update the canvas's height to fit the inner frame
                canvas.config(height=interior.winfo_reqheight())
        interior.bind('<Configure>', _configure_interior)
        
        def _bound_to_mousewheel(event):
            canvas.bind_all("<MouseWheel>", _on_mousewheel)
        canvas.bind('<Enter>', _bound_to_mousewheel)

        def _unbound_to_mousewheel(event):
            canvas.unbind_all("<MouseWheel>") 
        canvas.bind('<Leave>', _unbound_to_mousewheel)
    
        def _on_mousewheel(event):
            canvas.yview_scroll(int(-1*(event.delta/120)), "units")  
        