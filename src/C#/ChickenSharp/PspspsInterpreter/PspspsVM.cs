﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Esoterics.PspspsInterpreter
{
    public class PspspsVM
    {

        public PspspsInstructionSet InstructionSet;
        public FixedStack<int> Memory;
        public FixedStack<int> FunctionStack;

        #region Runtime
        public int InstructionPointer;
        public PspspsCode Code;
        public long TotalInstructionsExecuted;
        #endregion Runtime

        /// <summary>
        /// Creates a new VM that can execute Pspsps Code
        /// </summary>
        /// <param name="MemSize4">The memory capacity (Stack of Integers, so 4 bytes per added capacity)</param>
        public PspspsVM(PspspsInstructionSet set, int MemSize4 = 1024, int FunctionStackSize4 = 1024)
        {
            Memory = new FixedStack<int>(MemSize4);
            FunctionStack = new FixedStack<int>(FunctionStackSize4);
            InstructionSet = set;
        }

        public void Execute(PspspsCode code)
        {
            if (code.InstructionSetKey != InstructionSet.GetKey())
                throw new Exception($"Incompatible sets. The code is from the {code.InstructionSetKey} instruction set, whilst the VM is running {InstructionSet.GetKey()}");
            
            Code = code;                // Initializing
            InstructionPointer = 0;

            while (InstructionPointer < code.Instructions.Length)
            {
                //Debug();
                InstructionSet.Execute(code.Instructions[InstructionPointer], code.Arguments[InstructionPointer], this);
                InstructionPointer++;
                TotalInstructionsExecuted++;
            }

            InstructionPointer = -1;    // Flushing memory
            Code = null;

        }

        private void Debug()
        {
            int instruction = Code.Instructions[InstructionPointer];
            int arg = Code.Arguments[InstructionPointer];
            List<string> memString = new List<string>(Memory.Count());
            foreach (int i in Memory)
                memString.Add(i.ToString());
            string step = fromInt(TotalInstructionsExecuted, 5); // I assume im never gonna go over 5 digits, which is false
            string fsc = fromInt(FunctionStack.Count(), 4);
            string sip = fromInt(InstructionPointer, 4);
            string sarg = arg.ToString().Length < 3 ? fromInt(arg, 3) : arg.ToString();
            PspspsInstruction pspspsInstruction = InstructionSet.Instructions[instruction];
            Console.WriteLine($"STEP {step} FSC[{fsc}] IP[{sip}] : {pspspsInstruction.Name}{(pspspsInstruction.SupportsArgument ? $" {sarg}" : "    ")} | {string.Join(", ", memString)}");

            string fromInt(long i, int size)
            {
                string si = i.ToString();
                char[] ca = new char[size];
                for (int ci = 0; ci < size; ci++)
                {
                    if (ci < si.Length)
                        ca[size - si.Length + ci] = si[ci];
                    else
                        ca[ci - si.Length] = ' ';
                }
                return new string(ca);
            }

        }

    }
}
