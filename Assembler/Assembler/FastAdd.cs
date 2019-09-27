using Assembler;
using System;
using System.Text;

namespace Opcode
{
    class FastAdd
    {
        private int value = 0;
        public int GetValue()
        {
            return value;
        }
        public FastAdd(string value)
        {
            if (value.Length > 2)
            {
                if (value[1] == 'x' && value[0] != '\'')
                {
                    this.value = Convert.ToUInt16(value, 16) & 0b11111111;
                }
                else if (value[1] == 'b' && value[0] != '\'')
                {
                    this.value = Convert.ToUInt16(value, 2) & 0b11111111;
                }
                else
                {
                    if (Program.eightBit)
                    {
                        if (value[0] == '\'' && value[value.Length - 1] == '\'' && value.Length == 3)
                        {
                            this.value = Encoding.ASCII.GetBytes(value)[1];
                        }
                    }
                    else
                    {
                        throw new Exception("Unknown base: " + value);
                    }
                }
            }
            else
            {
                this.value = Convert.ToInt16(value, 10) & 0b11111111;
            }
            if (this.value < 0)
            {
                this.value = ~this.value;
            }
        }
        public FastAdd(int value)
        {
            this.value = value;
        }

        public static FastAdd Null => new FastAdd("0");

        public static bool IsFastAdd(string value)
        {
            if (value.Length == 3)
            {
                if (value[0] == '\'' && value[value.Length - 1] == '\'')
                {
                    return true;
                }
            }
            if (value.Length > 1)
            {
                if (char.ToLower(value[1]) == 'x')
                {
                    if (value[0] == '0')
                    {
                        for (int i = 2; i < value.Length; i++)
                        {
                            char c = value[i];
                            if (Char.ToLower(c) < '0' || Char.ToLower(c) > '9')
                            {
                                if (Char.ToLower(c) < 'a' || Char.ToLower(c) > 'f')
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        char c = value[i];
                        if (Char.ToLower(c) < '0' || Char.ToLower(c) > '9')
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else if (Char.ToLower(value[0]) == 'a' || Char.ToLower(value[0]) == 'b')
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public int toInt()
        {
            return value;
        }
        public byte toByte()
        {
            return (byte)value;
        }
    }
}
