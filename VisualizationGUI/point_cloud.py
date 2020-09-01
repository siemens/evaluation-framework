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

import argparse
from framework.visualization.gui import run_gui

def get_input_args():
    parser = argparse.ArgumentParser()
    
    parser.add_argument('evaluationName',
                        help = 'Name of evaluation to visualize')
    parser.add_argument('--useROS', action='store_true', default = False, 
                        help = 'use ROS alongside Unity')
    parser.add_argument('--timeScale', type = float, default = 1, 
                        help = 'Unity time scale for simulation speed')
    return parser.parse_args()

if __name__ == '__main__':
    in_args = get_input_args()
    run_gui(in_args.evaluationName, in_args.useROS, in_args.timeScale)