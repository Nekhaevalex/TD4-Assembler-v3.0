using Assembler;
using System;

namespace Utilities
{
    public class Utilities
    {
        public static int debug_x_pos;
        public static int debug_y_pos;
        public static int last_debug_x;
        public static int last_debug_y;
        public static void VerbouseOut(string message)
        {
            if (Program.verboseMode)
            {
                Console.WriteLine(message);
            }
        }
        public static void VerbouseOut(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (Program.verboseMode)
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
        }

        public static void VerbouseOut(string process, string message)
        {
            if (Program.verboseMode)
            {
                Console.WriteLine("[" + process + "]: " + message);
            }
        }
        public static void VerbouseOut(string process, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (Program.verboseMode)
            {
                Console.WriteLine("[" + process + "]: " + message);
            }
            Console.ResetColor();
        }

        public static void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("MESSAGE:" + message);
            Console.ResetColor();
        }
        public static void Warning(string process, string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Warning:" + "[" + process + "]: " + message);
            Console.ResetColor();
        }
        public static void Error(string process, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR in [" + process + "] " + message);
            Console.ResetColor();
        }
    }
}
