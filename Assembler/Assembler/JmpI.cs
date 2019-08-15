using Opcode;

namespace Assembler
{
    internal class JmpI : IOpcode
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
        public string Arg2
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int Page { get; set; }
        public int Word { get; set; }

        public JmpI(string arg1)
        {
            Name = "jmp";
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
        public JmpI(int arg1)
        {
            Name = "jmp";
            fastAdd = new FastAdd(arg1);
        }

        public MachineWord toMachineCode()
        {
            return new MachineWord(0b1111, FastAdd);
        }

        public override string ToString()
        {
            return "jmp " + FastAdd.ToString();
        }
    }
}