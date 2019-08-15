using Opcode;

namespace Assembler
{
    internal class JncI : IOpcode
    {
        public string Arg1 { get; set; }
        public string Name { get; set; }
        private FastAdd fastAdd;
        public FastAdd FastAdd
        {
            get
            {
                return fastAdd;
            }
            set
            {
                fastAdd = value;
            }
        }
        public string Arg2 { get; set; }

        public int Page { get; set; }
        public int Word { get; set; }

        public JncI(string arg1)
        {
            Name = "jnc";
            if (FastAdd.IsFastAdd(arg1))
            {
                FastAdd = new FastAdd(arg1);
                Arg1 = null;
            }
            else
            {
                //JMPL
                Arg1 = arg1;
            }

        }
        public JncI(int arg1)
        {
            Name = "jnc";
            FastAdd = new FastAdd(arg1);
            Arg1 = null;

        }
        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1110, FastAdd);
        }

        public override string ToString()
        {
            return "jnc " + FastAdd.ToString();
        }
    }
}