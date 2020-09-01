/*
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
*/

using System.Collections.Generic;
using System.Linq;

namespace EvaluationFrameworkROS
{
    public static partial class ParameterGridExtensions
    {
        /*
         * Author: Eric Lippert
         * Changes: The function was adapted to work return List<List<T>> instead of IEnumerable<IEnumerable<T>>
         * Source: https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/
         */
        public static List<List<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            // base case: 
            IEnumerable<List<T>> result = new[] { new List<T>() };
            foreach (var sequence in sequences)
            {
                var s = sequence; // don't close over the loop variable 
                                  // recursive case: use SelectMany to build the new product out of the old one 
                result =
                    from seq in result
                    from item in s
                    select seq.Concat(new[] { item }).ToList();
            }
            return result.ToList();
        }
    }
}
