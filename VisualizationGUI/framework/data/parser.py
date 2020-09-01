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

import definitions
import csv
import pandas as pd
import numpy as np

def get_path_to_root():
    return definitions.ROOT_DIR

def get_path_to_objectives(evaluation_name, use_ROS):
    parent = get_path_to_root().parent
    if (use_ROS):
        framework_folder = "EvaluationFrameworkROS"
    else:
        framework_folder = "EvaluationFramework"
    sub_dirs = [framework_folder, "Evaluations", evaluation_name]
    framework_root = parent / sub_dirs[0] / sub_dirs[1] / sub_dirs[2]
    return framework_root.joinpath("Results")

def parse_data(evaluation_name, use_ROS):
    goal_dir = get_path_to_objectives(evaluation_name, use_ROS)
    objectives_file = goal_dir / "Results.txt"
    
    # read first line to get headers
    with objectives_file.open(encoding="utf-8") as csv_file: 
        csv_reader = csv.reader(csv_file, delimiter='|')
        columns = next(csv_reader)
        evaluation_parameter_names = columns[0].split(";")
        num_params = len(evaluation_parameter_names)
        objective_value_names = columns[1].split(";")
        num_objectives = len(objective_value_names)
                
    # read whole content into pandas dataframe ;|[|]
    data = pd.read_csv(objectives_file, sep="[;|]", engine='python',
                       encoding="utf-8")
    num_values = len(data.index)
        
    # handle array variables in evaluation parameters
    names_to_remove = []
    for evalparam in evaluation_parameter_names:
        if type(data[evalparam].iloc[0]) != str:
            continue
        else:
            temp = pd.DataFrame([[float(idx) for idx in p.split(",")] 
                     for p in data[evalparam]])
            param, unit = evalparam.split(" ")
            columns = [param+str(i+1)+" "+unit for i in range(0, len(temp.columns))]
            temp.columns = columns
            evaluation_parameter_names.extend(columns)
            
            # inserting column with static value in data frame 
            loc = data.columns.get_loc(evalparam)
            for i, col in enumerate(columns):
                data.insert(loc+i, col, temp[col]) 
            
            # delete old series
            del data[evalparam]
            names_to_remove.append(evalparam)
    
    evaluation_parameter_names = [name for name in evaluation_parameter_names
                                  if name not in names_to_remove]
    
    array_names = names_to_remove
    
    # handle array variables in objective values
    names_to_remove = []
    for objval in objective_value_names:
        if type(data[objval].iloc[0]) != str:
            continue
        else:
            temp = pd.DataFrame([[float(idx) for idx in p.split(",")] 
                      for p in data[objval]])
            param, unit = objval.split(" ")
            columns = [param+str(i+1)+" "+unit for i in range(0, len(temp.columns))]
            temp.columns = columns
            objective_value_names.extend(columns)
            
            # inserting column with static value in data frame 
            loc = data.columns.get_loc(objval)
            for i, col in enumerate(columns):
                data.insert(loc+i, col, temp[col]) 
            
            # delete old series
            del data[objval]
            names_to_remove.append(objval)
            
    objective_value_names = [name for name in objective_value_names
                             if name not in names_to_remove]
    
    array_names.extend(names_to_remove)
    
    # round values to 5 decimal places
    data = data.round(8)
        
    dimensions = (num_values, num_params, num_objectives)
    param_names = (evaluation_parameter_names, objective_value_names)
    
    return dimensions, param_names, array_names, data    
