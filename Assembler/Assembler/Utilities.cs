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
            if (last_debug_y != 0)
                Console.SetCursorPosition(last_debug_x, last_debug_y);
            if (Program.verboseMode)
            {
                Console.WriteLine(message);
                if (last_debug_y != 0)
                    last_debug_y++;
            }
        }
        public static void VerbouseOut(string message, ConsoleColor color)
        {
            if (last_debug_y != 0)
                Console.SetCursorPosition(last_debug_x, last_debug_y);
            Console.ForegroundColor = color;
            if (Program.verboseMode)
            {
                Console.WriteLine(message);
                if (last_debug_y != 0)
                    last_debug_y++;
            }
            Console.ResetColor();
        }

        public static void VerbouseOut(string process, string message)
        {
            if (last_debug_y != 0)
            Console.SetCursorPosition(last_debug_x, last_debug_y);

            if (Program.verboseMode)
            {
                Console.WriteLine("[" + process + "]: " + message);
                if (last_debug_y != 0)
                    last_debug_y++;
            }
        }
        public static void VerbouseOut(string process, string message, ConsoleColor color)
        {
            if (last_debug_y != 0)
                Console.SetCursorPosition(last_debug_x, last_debug_y);

            Console.ForegroundColor = color;
            if (Program.verboseMode)
            {
                Console.WriteLine("[" + process + "]: " + message);
                if (last_debug_y != 0)
                    last_debug_y++;
            }
            Console.ResetColor();
        }
    }
}
