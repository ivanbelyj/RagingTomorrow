/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace AuroraFPSRuntime.CoreModules.CommandLine
{
    public sealed class CommandShell
    {
        public struct CommandInfo
        {
            public Action<Argument[]> action;
            public int maxArgCount;
            public int minArgCount;
            public string help;
            public string hint;
        }

        public struct Argument
        {
            private string stringValue;

            public Argument(string stringValue)
            {
                this.stringValue = stringValue;
            }

            public string GetStringValue()
            {
                return stringValue;
            }

            public void SetStringValue(string stringValue)
            {
                this.stringValue = stringValue;
            }

            public int GetIntValue()
            {
                int int_value;

                if (int.TryParse(stringValue, out int_value))
                {
                    return int_value;
                }

                TypeError("int");
                return 0;
            }

            public float GetFloatValue()
            {
                float float_value;

                if (float.TryParse(stringValue, out float_value))
                {
                    return float_value;
                }

                TypeError("float");
                return 0;
            }

            public bool GetBoolValue()
            {
                if (string.Compare(stringValue, "TRUE", ignoreCase: true) == 0)
                {
                    return true;
                }

                if (string.Compare(stringValue, "FALSE", ignoreCase: true) == 0)
                {
                    return false;
                }

                TypeError("bool");
                return false;
            }

            public override string ToString()
            {
                return stringValue;
            }

            private void TypeError(string expected_type)
            {
                Console.Shell.IssueErrorMessage(
                    "Incorrect type for {0}, expected <{1}>",
                    stringValue, expected_type
                );
            }
        }

        private readonly Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
        private readonly Dictionary<string, Argument> variables = new Dictionary<string, Argument>();
        private readonly List<Argument> arguments = new List<Argument>(); // Cache for performance
        private string issuedErrorMessage;

        /// <summary>
        /// Uses reflection to find all RegisterCommand attributes
        /// and adds them to the commands dictionary.
        /// </summary>
        public void RegisterCommands()
        {
            Dictionary<string, CommandInfo> rejectedCommands = new Dictionary<string, CommandInfo>();
            BindingFlags methodFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(methodFlags))
                {
                    ConsoleCommandAttribute attribute = Attribute.GetCustomAttribute(
                        method, typeof(ConsoleCommandAttribute)) as ConsoleCommandAttribute;

                    if (attribute == null)
                    {
                        if (method.Name.StartsWith("FRONTCOMMAND", StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Front-end Command methods don't implement RegisterCommand, use default attribute
                            attribute = new ConsoleCommandAttribute();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    var methodsParams = method.GetParameters();

                    string commandName = InferFrontCommandName(method.Name);
                    Action<Argument[]> action;

                    if (attribute.Name == null)
                    {
                        // Use the method's name as the command's name
                        commandName = InferCommandName(commandName == null ? method.Name : commandName);
                    }
                    else
                    {
                        commandName = attribute.Name;
                    }

                    if (methodsParams.Length != 1 || methodsParams[0].ParameterType != typeof(Argument[]))
                    {
                        // Method does not match expected Action signature,
                        // this could be a command that has a FrontCommand method to handle its arguments.
                        rejectedCommands.Add(commandName.ToUpper(), CommandFromParamInfo(methodsParams, attribute.Help));
                        continue;
                    }

                    // Convert MethodInfo to Action.
                    // This is essentially allows us to store a reference to the method,
                    // which makes calling the method significantly more performant than using MethodInfo.Invoke().
                    action = (Action<Argument[]>)Delegate.CreateDelegate(typeof(Action<Argument[]>), method);
                    AddCommand(commandName, action, attribute.MinArgCount, attribute.MaxArgCount, attribute.Help, attribute.Hint);
                }
            }
            HandleRejectedCommands(rejectedCommands);
        }

        /// <summary>
        /// Parses an input line into a command and runs that command.
        /// </summary>
        public void RunCommand(string line)
        {
            string remaining = line;
            issuedErrorMessage = null;
            arguments.Clear();

            while (remaining != "")
            {
                var argument = EatArgument(ref remaining);

                if (argument.GetStringValue() != "")
                {
                    if (argument.GetStringValue()[0] == '$')
                    {
                        string variableName = argument.GetStringValue().Substring(1).ToUpper();

                        if (variables.ContainsKey(variableName))
                        {
                            // Replace variable argument if it's defined
                            argument = variables[variableName];
                        }
                    }
                    arguments.Add(argument);
                }
            }

            if (arguments.Count == 0)
            {
                // Nothing to run
                return;
            }

            string commandName = arguments[0].GetStringValue().ToUpper();
            arguments.RemoveAt(0); // Remove command name from arguments

            if (!commands.ContainsKey(commandName))
            {
                IssueErrorMessage("Command {0} could not be found", commandName);
                return;
            }

            RunCommand(commandName, arguments.ToArray());
        }

        public void RunCommand(string commandName, Argument[] arguments)
        {
            var command = commands[commandName];
            int argCount = arguments.Length;
            string errorMessage = null;
            int requiredArg = 0;

            if (argCount < command.minArgCount)
            {
                if (command.minArgCount == command.maxArgCount)
                {
                    errorMessage = "exactly";
                }
                else
                {
                    errorMessage = "at least";
                }
                requiredArg = command.minArgCount;
            }
            else if (command.maxArgCount > -1 && argCount > command.maxArgCount)
            {
                // Do not check max allowed number of arguments if it is -1
                if (command.minArgCount == command.maxArgCount)
                {
                    errorMessage = "exactly";
                }
                else
                {
                    errorMessage = "at most";
                }
                requiredArg = command.maxArgCount;
            }

            if (errorMessage != null)
            {
                string pluralFix = requiredArg == 1 ? "" : "s";

                IssueErrorMessage(
                    "{0} requires {1} {2} argument{3}",
                    commandName,
                    errorMessage,
                    requiredArg,
                    pluralFix
                );

                if (command.hint != null)
                {
                    issuedErrorMessage += string.Format("\n    -> Usage: {0}", command.hint);
                }

                return;
            }

            command.action(arguments);
        }

        public void AddCommand(string name, CommandInfo info)
        {
            name = name.ToUpper();

            if (commands.ContainsKey(name))
            {
                IssueErrorMessage("Command {0} is already defined.", name);
                return;
            }

            commands.Add(name, info);
        }

        public void AddCommand(string name, Action<Argument[]> action, int minArgs = 0, int maxArgs = -1, string help = "", string hint = null)
        {
            var info = new CommandInfo()
            {
                action = action,
                minArgCount = minArgs,
                maxArgCount = maxArgs,
                help = help,
                hint = hint
            };

            AddCommand(name, info);
        }

        public void SetVariable(string name, string value)
        {
            SetVariable(name, new Argument(value));
        }

        public void SetVariable(string name, Argument value)
        {
            name = name.ToUpper();

            if (variables.ContainsKey(name))
            {
                variables[name] = value;
            }
            else
            {
                variables.Add(name, value);
            }
        }

        public Argument GetVariable(string name)
        {
            name = name.ToUpper();

            if (variables.ContainsKey(name))
            {
                return variables[name];
            }

            IssueErrorMessage("No variable named {0}", name);
            return new Argument();
        }

        public void IssueErrorMessage(string format, params object[] message)
        {
            issuedErrorMessage = string.Format(format, message);
        }

        private string InferCommandName(string methodName)
        {
            string commandName;
            int index = methodName.IndexOf("COMMAND", StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Method is prefixed, suffixed with, or contains "COMMAND".
                commandName = methodName.Remove(index, 7);
            }
            else
            {
                commandName = methodName;
            }

            return commandName;
        }

        private string InferFrontCommandName(string methodName)
        {
            int index = methodName.IndexOf("FRONT", StringComparison.CurrentCultureIgnoreCase);
            return index >= 0 ? methodName.Remove(index, 5) : null;
        }

        private void HandleRejectedCommands(Dictionary<string, CommandInfo> rejectedCommands)
        {
            foreach (var command in rejectedCommands)
            {
                if (commands.ContainsKey(command.Key))
                {
                    commands[command.Key] = new CommandInfo()
                    {
                        action = commands[command.Key].action,
                        minArgCount = command.Value.minArgCount,
                        maxArgCount = command.Value.maxArgCount,
                        help = command.Value.help
                    };
                }
                else
                {
                    IssueErrorMessage("{0} is missing a front command.", command);
                }
            }
        }

        private CommandInfo CommandFromParamInfo(ParameterInfo[] parameters, string help)
        {
            int optionalArgs = 0;

            foreach (var param in parameters)
            {
                if (param.IsOptional)
                {
                    optionalArgs += 1;
                }
            }

            return new CommandInfo()
            {
                action = null,
                minArgCount = parameters.Length - optionalArgs,
                maxArgCount = parameters.Length,
                help = help
            };
        }

        private Argument EatArgument(ref string s)
        {
            Argument arg = new Argument();
            int spaceIndex = s.IndexOf(' ');

            if (spaceIndex >= 0)
            {
                arg.SetStringValue(s.Substring(0, spaceIndex));
                s = s.Substring(spaceIndex + 1); // Remaining
            }
            else
            {
                arg.SetStringValue(s);
                s = "";
            }

            return arg;
        }

        #region [Getter / Setter]
        public Dictionary<string, CommandInfo> GetCommands()
        {
            return commands;
        }

        public Dictionary<string, Argument> GetVariables()
        {
            return variables;
        }

        public string GetIssuedErrorMessage()
        {
            return issuedErrorMessage;
        }
        #endregion
    }
}
