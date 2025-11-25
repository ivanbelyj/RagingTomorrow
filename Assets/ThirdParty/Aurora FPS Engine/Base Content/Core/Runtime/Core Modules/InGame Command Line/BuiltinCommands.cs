/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright ? 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Text;
using System.Diagnostics;
using UnityEngine;
using System.Reflection;
using System;

namespace AuroraFPSRuntime.CoreModules.CommandLine
{
    internal static class BuiltinCommands
    {
        [ConsoleCommand(Help = "Clear the command console", MaxArgCount = 0)]
        static void CommandClear(CommandShell.Argument[] args) {
            Console.Buffer.Clear();
        }

        [ConsoleCommand(Help = "History of console commands", MaxArgCount = 0)]
        static void CommandHistory(CommandShell.Argument[] args)
        {
            int count = Console.History.Count();
            if (count == 0)
            {
                Console.Log(ConsoleLog.LogType.Warning, "History is empty");
            }
            else
            {
                Console.Log(ConsoleLog.LogType.Warning, "History has {0} commands", count);
                count = 1;
                foreach (string command in Console.History.Commands)
                {
                    Console.Log("{0}: {1}", count++, command);
                }
            }
        }

        [ConsoleCommand(Help = "Display help information about a command", MaxArgCount = 1)]
        static void CommandHelp(CommandShell.Argument[] args) {
            if (args.Length == 0) {
                foreach (var command in Console.Shell.GetCommands()) {
                    Console.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }
                return;
            }

            string command_name = args[0].GetStringValue().ToUpper();

            if (!Console.Shell.GetCommands().ContainsKey(command_name)) {
                Console.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            var info = Console.Shell.GetCommands()[command_name];

            if (info.help == null) {
                Console.Log("{0} does not provide any help documentation.", command_name);
            } else if (info.hint == null) {
                Console.Log(info.help);
            } else {
                Console.Log("{0}\nUsage: {1}", info.help, info.hint);
            }
        }

        [ConsoleCommand(Help = "Time the execution of a command", MinArgCount = 1)]
        static void CommandTime(CommandShell.Argument[] args) {
            var sw = new Stopwatch();
            sw.Start();

            Console.Shell.RunCommand(JoinArguments(args));

            sw.Stop();
            Console.Log("Time: {0}ms", (double)sw.ElapsedTicks / 10000);
        }

        [ConsoleCommand(Help = "Set time scale.", MinArgCount = 1, MaxArgCount = 1)]
        static void CommandTimeScale(CommandShell.Argument[] args)
        {
            if(args.Length > 0)
            {
                Time.timeScale = args[0].GetFloatValue();
            }
        }

        [ConsoleCommand(Help = "Show stats analyst.", MinArgCount = 1)]
        static void CommandStats(CommandShell.Argument[] args)
        {
            if (args.Length > 0)
            {
                GameStatsAnalyst gameStatsAnalyst = GameStatsAnalyst.GetRuntimeInstance();
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i].GetStringValue();
                    if (arg.Contains("system_refresh"))
                    {
                        arg = arg.Substring(15, arg.Length - 16);
                        arg = arg.Replace('.', ',');
                        if (float.TryParse(arg, out float time))
                        {
                            gameStatsAnalyst.RefreshTime(time);
                        }
                        else
                        {
                            Console.Log("Invalid input refresh time. Format: system_refresh(value)");
                        }
                        continue;
                    }
                    else if (arg.Contains("AI"))
                    {
                        arg = arg.Substring(3, arg.Length - 4);
                        GameObject ai = GameObject.Find(arg);
                        if(ai != null)
                        {
                            AIModules.AIController aiController = ai.GetComponent<AIModules.AIController>();
                            if (aiController != null)
                            {
                                gameStatsAnalyst.ShowAIControllerStats(aiController);
                                continue;
                            }
                        }
                        Console.Log(ConsoleLog.LogType.Error, string.Format("AI controller with target name {0} not found!", arg));
                        continue;
                    }

                    switch (arg)
                    {
                        case "fps":
                            gameStatsAnalyst.ShowFPS(true);
                            break;
                        case "-fps":
                            gameStatsAnalyst.ShowFPS(false);
                            break;
                        case "gpu_time":
                            gameStatsAnalyst.ShowGPUTime(true);
                            break;
                        case "-gpu_time":
                            gameStatsAnalyst.ShowGPUTime(false);
                            break;
                        case "controller":
                            gameStatsAnalyst.ShowControllerStats(true);
                            break;
                        case "-controller":
                            gameStatsAnalyst.ShowControllerStats(false);
                            break;
                        default:
                            Console.Log(ConsoleLog.LogType.Error, string.Format("{0} argument not found for stats command!", arg));
                            break;
                    }
                }
                
            }
        }

        [ConsoleCommand(Help = "Edit game frame rate.", MinArgCount = 1, MaxArgCount = 1)]
        static void CommandFrameRate(CommandShell.Argument[] args)
        {
            int frameRateValue = args[0].GetIntValue();
            Application.targetFrameRate = frameRateValue;
            if(frameRateValue > 0)
            {
                Console.Log(ConsoleLog.LogType.Warning, string.Format("Game frame rate locked to {0}.", frameRateValue));
            }
            else
            {
                Console.Log(ConsoleLog.LogType.Warning, "Game frame rate unlocked.");
            }
        }

        [ConsoleCommand(Help = "Output message")]
        static void CommandPrint(CommandShell.Argument[] args) {
            Console.Log(JoinArguments(args));
        }

        [ConsoleCommand(Help = "Spawn object.", MinArgCount = 1, MaxArgCount = 99)]
        static void CommandSpawn(CommandShell.Argument[] args)
        {
            if(args.Length > 0)
            {
                Transform camera = Camera.main.transform;
                string id = args[0].GetStringValue();
                if(id == "cube")
                {
                    GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gameObject.transform.position = camera.position + (camera.forward * 1.25f);
                    gameObject.transform.rotation = Quaternion.LookRotation(camera.forward);

                    Console.Log(args.Length.ToString());
                    for (int i = 1; i < args.Length; i++)
                    {
                        string argValue = args[i].GetStringValue();
                        if (argValue.StartsWith("name:"))
                        {
                            gameObject.name = argValue.Remove(0, 5);
                        }
                        else if(argValue.StartsWith("comp:"))
                        {
                            string typeID = argValue.Remove(0, 5);
                            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                            for (int j = 0; j < assemblies.Length; j++)
                            {
                                Assembly assembly = assemblies[j];
                                Type[] types = assembly.GetTypes();
                                for (int t = 0; t < types.Length; t++)
                                {
                                    Type type = types[t];
                                    if(type.Name == typeID)
                                    {
                                        gameObject.AddComponent(type);
                                        goto M;
                                    }
                                }
                            }
                        }
                        else if (argValue.StartsWith("pos:"))
                        {
                            string vector = argValue.Remove(0, 4);
                            string[] axes = vector.Split(',');
                            gameObject.transform.position = new Vector3(System.Convert.ToSingle(axes[0]), System.Convert.ToSingle(axes[1]), System.Convert.ToSingle(axes[2]));
                        }
                    M: continue;
                    }
                }
            }
        }

    #if DEBUG
        [ConsoleCommand(Help = "Output the stack trace of the previous message", MaxArgCount = 0)]
        static void CommandTrace(CommandShell.Argument[] args) {
            int log_count = Console.Buffer.Logs.Count;

            if (log_count - 2 < 0) {
                Console.Log("Nothing to trace.");
                return;
            }

            var log_item = Console.Buffer.Logs[log_count - 2];

            if (log_item.stackTrace == "") {
                Console.Log("{0} (no trace)", log_item.message);
            } else {
                Console.Log(log_item.stackTrace);
            }
        }
    #endif

        [ConsoleCommand(Help = "List all variables or set a variable value")]
        static void CommandSet(CommandShell.Argument[] args) {
            if (args.Length == 0) {
                foreach (var kv in Console.Shell.GetVariables()) {
                    Console.Log("{0}: {1}", kv.Key.PadRight(16), kv.Value);
                }
                return;
            }

            string variable_name = args[0].GetStringValue();

            if (variable_name[0] == '$') {
                Console.Log(ConsoleLog.LogType.Warning, "Warning: Variable name starts with '$', '${0}'.", variable_name);
            }

            Console.Shell.SetVariable(variable_name, JoinArguments(args, 1));
        }

        [ConsoleCommand(Help = "No operation")]
        static void CommandNoop(CommandShell.Argument[] args) { }

        [ConsoleCommand(Help = "Quit running application", MaxArgCount = 0)]
        static void CommandQuit(CommandShell.Argument[] args) {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        static string JoinArguments(CommandShell.Argument[] args, int start = 0) {
            var sb = new StringBuilder();
            int arg_length = args.Length;

            for (int i = start; i < arg_length; i++) {
                sb.Append(args[i].GetStringValue());

                if (i < arg_length - 1) {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
