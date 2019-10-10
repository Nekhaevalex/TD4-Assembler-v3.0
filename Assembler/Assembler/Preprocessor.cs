using AST;
using Opcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler
{

    public struct pextData
    {
        public string pextName;
        public int mountPoint;
        public pextData(string[] parsedData)
        {
            pextName = parsedData[1];
            mountPoint = int.Parse(parsedData[2]);
        }
    }
    class Preprocessor
    {
        Dictionary<string, Macros> imports = new Dictionary<string, Macros>();
        Dictionary<string, Pext> pexts = new Dictionary<string, Pext>();
        readonly Dictionary<string, string> definitions = new Dictionary<string, string>();

        public int CountMacroses()
        {
            return imports.Count + pexts.Count;
        }

        public Preprocessor(ArrayList importsList)
        {
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Reading macroses list");
            foreach (string macrosName in importsList)
            {
                imports = imports.Concat(CodeIO.LoadMacros(macrosName)).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
            }
        }

        public void ImportPexts(ArrayList pextsList)
        {
            Utilities.Utilities.VerbouseOut("PREPROCESSOR", "Reading pext list");
            foreach (pextData pextName in pextsList)
            {
                pexts = pexts.Concat(CodeIO.LoadPext(pextName.pextName, pextName.mountPoint)).GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
            }
        }

        public void AddDefinition(string name, string value)
        {

            definitions.Add(name, definitions.ContainsKey(value) ? definitions[value] : value);
        }

        public void RemoveDefinition(string name)
        {
            if (definitions.ContainsKey(name))
            {
                definitions.Remove(name);
            }
        }

        public string GetDefinition(string name)
        {
            if (definitions.ContainsKey(name))
            {
                return definitions[name];
            }
            else
            {
                return null;
            }
        }

        private void MapDefine(string name, string toMap, int amount)
        {
            toMap = definitions.ContainsKey(toMap) ? GetDefinition(toMap) : toMap;
            if (toMap[0] == '\"' && toMap[toMap.Length - 1] == '\"')
            {
                string clear = toMap.Substring(1, toMap.Length - 2);
                amount = clear.Length;
                int step = clear.Length / amount;
                for (int i = 0; i < clear.Length; i += step)
                {
                    AddDefinition(name + "[" + (i / step).ToString() + "]", '\'' + (clear).Substring(i, step) + '\'');
                }
                AddDefinition(name + ".length", clear.Length.ToString());
                Utilities.Utilities.VerbouseOut("\tMapped: " + name + " into " + clear.Length.ToString() + " parts", ConsoleColor.Green);
            }
            else
            {
                int additionalNulls = amount - ((toMap.Length - 1) % amount);
                string a = new string('0', additionalNulls);
                int step = (a + toMap).Length / amount;
                for (int i = 0; i < (toMap + a).Length; i += step)
                {
                    AddDefinition(name + "[" + (i / step).ToString() + "]", (toMap + a).Substring(i, step));
                }
                AddDefinition(name + ".length", (toMap + a).Length.ToString());
                Utilities.Utilities.VerbouseOut("\tMapped: " + name + " into " + (toMap + a).Length.ToString() + " parts", ConsoleColor.Green);
            }
        }

        public string[][] CatchDefines(string[][] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != null)
                {
                    if (text[i][0] == "#define")
                    {
                        AddDefinition(text[i][1], text[i][2]);
                        Utilities.Utilities.VerbouseOut("\tDefined: " + text[i][1] + " = " + text[i][2], ConsoleColor.Green);
                    }
                    else if (text[i][0] == "#ifdef")
                    {
                        string testval = text[i][1];
                        int cacheIter = i;
                        if (definitions.ContainsKey(testval))
                        {
                            text[i] = null;
                            i++;
                            while (text[i][0] != "#endif" && text[i][0] != "#else")
                            {
                                i++;
                            }
                            if (text[i][0] == "#else")
                            {
                                text[i] = null;
                                i++;
                                while (text[i][0] != "#endif")
                                {
                                    text[i] = null;
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            text[i] = null;
                            i++;
                            while (text[i][0] != "#endif" && text[i][0] != "#else")
                            {
                                text[i] = null;
                                i++;
                            }
                            if (text[i][0] == "#else")
                            {
                                text[i] = null;
                                i++;
                                while (text[i][0] != "#endif")
                                {
                                    i++;
                                }
                            }
                        }
                        i = cacheIter;
                    }
                    else if (text[i][0] == "#ifndef")
                    {
                        string testval = text[i][1];
                        int cacheIter = i;
                        if (!definitions.ContainsKey(testval))
                        {
                            text[i] = null;
                            i++;
                            while (text[i][0] != "#endif" && text[i][0] != "#else")
                            {
                                i++;
                            }
                            if (text[i][0] == "#else")
                            {
                                text[i] = null;
                                i++;
                                while (text[i][0] != "#endif")
                                {
                                    text[i] = null;
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            text[i] = null;
                            i++;
                            while (text[i][0] != "#endif" && text[i][0] != "#else")
                            {
                                text[i] = null;
                                i++;
                            }
                            if (text[i][0] == "#else")
                            {
                                text[i] = null;
                                i++;
                                while (text[i][0] != "#endif")
                                {
                                    i++;
                                }
                            }
                        }
                        i = cacheIter;
                    }
                    else if (text[i][0] == "#line")
                    {
                        int stringNum = int.Parse(text[i][1]);
                        string filename = text[i][2].Substring(1, text[i][2].Length - 2);
                        text[i] = Assembly.SplitCode(CodeIO.LoadFile(filename))[stringNum];
                    }
                    else if (text[i][0] == "#error")
                    {
                        Utilities.Utilities.Error("COMPILATION_WORKFLOW", text[i][1]);
                        Environment.Exit(1);
                    }
                    else if (text[i][0] == "#message")
                    {
                        Utilities.Utilities.Message(text[i][1]);
                    }
                    else if (text[i][0] == "#pragma")
                    {
                        switch (text[i][1])
                        {
                            case "8_BIT":
                                AddDefinition(text[i][1], "TRUE");
                                Program.eightBit = true;
                                break;
                            case "NO_OPT":
                                AddDefinition(text[i][1], "TRUE");
                                Program.optimize = false;
                                break;
                            case "4_BIT":
                                AddDefinition(text[i][1], "TRUE");
                                Program.eightBit = false;
                                break;
                            case "USE_TRACE":
                                AddDefinition(text[i][1], "TRUE");
                                Program.useTracer = true;
                                break;
                            case "NO_TRACE":
                                AddDefinition(text[i][1], "TRUE");
                                Program.useTracer = false;
                                break;
                            case "CONFIG":
                                if (!Program.configExists)
                                {
                                    Utilities.Utilities.Error("PREPROCESSOR", "Configuration file was not specified.");
                                }
                                AddDefinition("HAS_CONFIG", "TRUE");
                                for (int j = 0; j < Program.config.Length; j++)
                                {
                                    AddDefinition("config." + Program.config[j][0], Program.config[j][1]);
                                }
                                break;
                            case "UNDEF_ALL":
                                definitions.Clear();
                                break;
                            case "DEF_SYS":
                                if (Program.eightBit)
                                {
                                    AddDefinition("8_BIT", "TRUE");
                                }
                                else
                                {
                                    AddDefinition("4_BIT", "TRUE");
                                }
                                if (Program.makeBinary)
                                {
                                    AddDefinition("BINARY", "TRUE");
                                }
                                else
                                {
                                    AddDefinition("TEXT", "TRUE");
                                }
                                if (Program.optimize)
                                {
                                    AddDefinition("OPTIMIZE", "TRUE");
                                }
                                else
                                {
                                    AddDefinition("NO_OPT", "TRUE");
                                }
                                if (Program.useTracer)
                                {
                                    AddDefinition("USE_TRACE", "TRUE");
                                }
                                else
                                {
                                    AddDefinition("NO_TRACE", "TRUE");
                                }
                                break;
                        }
                    }
                    else if (text[i][0] == "#sumdef")
                    {
                        string val = text[i][1];
                        string addval = text[i][2];
                        int oldVal = FastAdd.IsFastAdd(val) ? int.Parse(val) : new FastAdd(definitions[val]).toInt();
                        int toAdd = FastAdd.IsFastAdd(addval) ? int.Parse(addval) : new FastAdd(definitions[addval]).toInt();
                        definitions[val] = (oldVal + toAdd).ToString();
                        Utilities.Utilities.VerbouseOut("\tRedefined: " + text[i][1] + ": " + (oldVal + toAdd).ToString(), ConsoleColor.Yellow);
                    }
                    else if (text[i][0] == "#resdef")
                    {
                        string val = text[i][1];
                        string resval = text[i][2];
                        int oldVal = FastAdd.IsFastAdd(val) ? int.Parse(val) : new FastAdd(definitions[val]).toInt();
                        int toSub = FastAdd.IsFastAdd(resval) ? int.Parse(resval) : new FastAdd(definitions[resval]).toInt();
                        definitions[val] = (oldVal - toSub).ToString();
                        Utilities.Utilities.VerbouseOut("\tRedefined: " + text[i][1] + ": " + (oldVal - toSub).ToString(), ConsoleColor.Yellow);
                    }
                    else if (text[i][0] == "#undef")
                    {
                        RemoveDefinition(text[i][1]);
                        Utilities.Utilities.VerbouseOut("\tUndefined: " + text[i][1], ConsoleColor.Red);
                    }
                    else if (text[i][0] == "#map")
                    {
                        if (text[i].Length > 3)
                        {
                            MapDefine(text[i][2], text[i][1], int.Parse(text[i][3]));
                        }
                        else
                        {
                            MapDefine(text[i][2], text[i][1], Program.eightBit ? 256 : 16);
                        }
                    }
                    else if (text[i][0] == "#fordef")
                    {
                        List<string[]> dictLocal = new List<string[]>();
                        int j = i + 1;
                        int pastePos = i;
                        string iterator = text[i][1];
                        int initialValue = FastAdd.IsFastAdd(text[i][2]) ? int.Parse(text[i][2]) : new FastAdd(definitions[text[i][2]]).toInt();
                        int endingVale = FastAdd.IsFastAdd(text[i][3]) ? int.Parse(text[i][3]) : int.Parse(definitions[text[i][3]]);
                        int step = int.Parse(text[i][4]);
                        text[i] = null;
                        while (text[j][0] != "#endfor")
                        {
                            dictLocal.Add(text[j]);
                            text[j] = null;
                            j++;
                        }
                        text[j] = null;
                        for (int l = initialValue; l < endingVale; l += step)
                        {
                            for (int k = 0; k < dictLocal.Count; k++)
                            {
                                text = InsertCopySplitLine(text, dictLocal[k], pastePos, iterator, l.ToString());
                                Assembly.InsertSplitLine(dictLocal[k], pastePos, iterator, l.ToString());
                                pastePos++;
                            }
                        }
                        i--;
                    }
                    else
                    {
                        for (int j = 0; j < text[i].Length; j++)
                        {
                            var toPaste = GetDefinition(text[i][j]);
                            if (toPaste != null)
                            {
                                text[i][j] = toPaste;
                                Assembly.ReplaceLexem(i, j, toPaste);
                            }
                        }
                    }
                }
            }
            if (Program.verboseMode)
            {
                if (definitions.Keys.Count > 0)
                {
                    Utilities.Utilities.Warning("PREPROCESSOR", "WARNING: NOT ALL DEFINITIONS WERE UNDEFINED");
                    foreach (string s in definitions.Keys)
                    {
                        Console.WriteLine("\t{0}\t:\t{1}", s, definitions[s]);
                    }
                    Utilities.Utilities.Warning("PREPROCESSOR", "Undefined variables may cause assembly errors");
                }
            }
            return text;
        }

        private string[][] InsertCopySplitLine(string[][] parsed, string[] toPaste, int addr, string toReplace, string Replacer)
        {
            List<string[]> vs = new List<string[]>();
            var toPasteCopy = new string[toPaste.Length];
            for (int i = 0; i < toPaste.Length; i++)
            {
                toPasteCopy[i] = toPaste[i].Replace(toReplace, Replacer);
            }
            for (int i = 0; i < parsed.Length; i++)
            {
                if (i == addr)
                {
                    vs.Add(toPasteCopy);
                }
                string[] vs1 = parsed[i];
                vs.Add(vs1);
            }
            return vs.ToArray();
        }

        internal ASTree LookUpPext(string opcode, string[] args)
        {
            try
            {
                Pext toReplace = pexts[opcode];
                return toReplace.GeneratePextCode(args);
            }
            catch (KeyNotFoundException e)
            {
                Utilities.Utilities.VerbouseOut("ERROR", "Unknown command: " + opcode, ConsoleColor.Red);
                Environment.Exit(1);
                return null;
            }
        }
        internal string[][] LookUpMacro(string opcode, string[] args)
        {
            if (imports.ContainsKey(opcode))
            {
                Macros toReplace = imports[opcode];
                return toReplace.GenerateMacroCode(args);
            }
            else
            {
                return null;
            }
        }
    }
}
