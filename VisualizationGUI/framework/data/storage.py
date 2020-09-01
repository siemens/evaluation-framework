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

from . import parser

class DataStorage:
    @classmethod
    def fill_data_storage(cls, evaluation_name, use_ROS, time_scale):
        cls.evaluation_name = evaluation_name
        cls.use_ROS = use_ROS
        cls.time_scale = time_scale
        cls.dims, cls.data_names, cls.array_names, cls.data = parser.parse_data(evaluation_name,
                                                                                use_ROS)

    @classmethod
    def get_evaluation_name(cls):
        return cls.evaluation_name
    
    @classmethod
    def get_useROS_bool(cls):
        return cls.use_ROS
    
    @classmethod
    def get_time_scale(cls):
        return cls.time_scale
    
    @classmethod
    def get_data_dimension(cls):
        return cls.dims
    
    @classmethod
    def get_data(cls):
        return cls.data
    
    @classmethod
    def get_parameter_names(cls):
        return cls.data_names[0]
    
    @classmethod
    def get_objective_names(cls):
        return cls.data_names[1]
    
    @classmethod
    def get_array_names(cls):
        return cls.array_names