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

using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public static class CommandLineArgumentParser
{
    private static CultureInfo parsingCultureInfo;

    private static readonly string pipeIDString = "pipeID";
    private static readonly string timeScaleString = "timeScale";
    private static readonly string useROSString = "useROS";
    private static readonly string rosParamString = "rosParam";

    public static string pipeID;
    public static float timeScale = 0;
    public static bool useROSPipe = false;
    public static List<int> arraySizes = new List<int>();

    public static void ParseCommandLineArguments()
    {
        parsingCultureInfo = new CultureInfo("en-US", false);
        parsingCultureInfo.NumberFormat.NumberDecimalSeparator = ".";

        /* Command Line Arguments come in the following form
         *      "-..."
         *      "-pipeID int"
         *      "-timeScale int"
         *      "-batchmode"
         *      "-..."
        */
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            // if argument does not start with a "-" ignore it
            if (!args[i].StartsWith("-"))
                continue;

            // else: replace input argument to same string without '-'
            string argumentString = args[i].Replace("-", "");

            // get pipe ID
            if (argumentString == pipeIDString)
            {
                pipeID = args[i + 1];
                // manually increase i by 1
                i++;
            }
            else if (argumentString == timeScaleString)
            {
                timeScale = float.Parse(args[i + 1], parsingCultureInfo);
                // manually increase i by 1
                i++;
            }
            else if (argumentString == useROSString)
                useROSPipe = true;
            else if (argumentString == rosParamString)
            {
                // get ros parameter array sizes
                // if 1D - array --> one number for array size
                // if 2D - array --> outerArraySize,innerArraySize
                if (args[i + 1].Contains(","))
                {
                    string[] arrSizes = args[i + 1].Split(',');
                    arraySizes.Add(int.Parse(arrSizes[0]));
                    arraySizes.Add(int.Parse(arrSizes[1]));
                }
                else
                    arraySizes.Add(int.Parse(args[i + 1]));

                // manually increase i by 1
                i++;
            }
        }
    }
    public static void SetTimeScale()
    {
        if (timeScale == 0)
        {
            float currentTimeStep = Time.fixedDeltaTime;
            float minimumTimeStep = 0.001f; // 1 ms
            Time.timeScale = currentTimeStep / minimumTimeStep; // scale time up to a timestep of 1ms
        }
        else
            Time.timeScale = timeScale;
    }
}
