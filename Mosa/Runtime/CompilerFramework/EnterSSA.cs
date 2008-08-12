/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mosa.Runtime.CompilerFramework
{
    /// <summary>
    /// Transforms the intermediate representation into a minimal static single assignment form.
    /// </summary>
    /// <remarks>
    /// The minimal form only inserts the really required PHI functions in order to reduce the 
    /// number of live registers used by register allocation.
    /// </remarks>
    public sealed class EnterSSA : IMethodCompilerStage
    {
        #region Constants

        private const string c_ssaConverted = @"EnterSSA.Converted";

        #endregion // Constants

        #region Types

        struct WorkItem
        {
            public WorkItem(BasicBlock block, BasicBlock caller, IDictionary<StackOperand, StackOperand> liveIn)
            {
                this.block = block;
                this.caller = caller;
                this.liveIn = liveIn;
            }

            public BasicBlock block;
            public BasicBlock caller;
            public IDictionary<StackOperand, StackOperand> liveIn;
        }

        #endregion // Types

        #region Data members

        /// <summary>
        /// The architecture to create the PhiInstructions.
        /// </summary>
        private IArchitecture _architecture;

        /// <summary>
        /// Holds the dominance frontier blocks of the stage.
        /// </summary>
        private BasicBlock[] _dominanceFrontierBlocks;

        /// <summary>
        /// Holds the dominance provider.
        /// </summary>
        private IDominanceProvider _dominanceProvider;

        /// <summary>
        /// Holds the operand definitions after each block (array of block count length).
        /// </summary>
        private IDictionary<StackOperand, StackOperand>[] _liveness;

        /// <summary>
        /// Holds the version of the next SSA variable. (We use a single version number for all of them)
        /// </summary>
        private int _ssaVersion;

        #endregion // Data members

        #region Construction

        public EnterSSA()
        {
        }

        #endregion // Construction

        #region IMethodCompilerStage Members

        string IMethodCompilerStage.Name
        {
            get { return @"EnterSSA"; }
        }

        void IMethodCompilerStage.Run(MethodCompilerBase compiler)
        {
            // Retrieve the basic block provider
            IBasicBlockProvider blockProvider = (IBasicBlockProvider)compiler.GetPreviousStage(typeof(IBasicBlockProvider));
            if (null == blockProvider)
                throw new InvalidOperationException(@"SSA Conversion requires basic blocks.");
            _dominanceProvider = (IDominanceProvider)compiler.GetPreviousStage(typeof(IDominanceProvider));
            Debug.Assert(null != _dominanceProvider, @"SSA Conversion requires a dominance provider.");
            if (null == _dominanceProvider)
                throw new InvalidOperationException(@"SSA Conversion requires a dominance provider.");
            _architecture = compiler.Architecture;

            // Allocate space for live outs
            _liveness = new IDictionary<StackOperand,StackOperand>[blockProvider.Count];
            // Retrieve the dominance frontier blocks
            _dominanceFrontierBlocks = _dominanceProvider.GetDominanceFrontier();

            // Transformation worklist 
            Queue<WorkItem> workList = new Queue<WorkItem>();

            /* FIXME: Move parameter operands into the dictionary as version 0,
             * because they are live at entry and maybe referenced. Anyways, an
             * assignment to a parameter is also SSA related.
             */
            IDictionary<StackOperand, StackOperand> liveIn = new Dictionary<StackOperand, StackOperand>(s_comparer);

            // Start with the very first block
            workList.Enqueue(new WorkItem(blockProvider[0], null, liveIn));

            // Iterate until the worklist is empty
            while (0 != workList.Count)
            {
                // Remove the block from the queue
                WorkItem workItem = workList.Dequeue();

                // Transform the block
                BasicBlock block = workItem.block;
                bool schedule = TransformToSsaForm(block, workItem.caller, workItem.liveIn, out liveIn);
                _liveness[block.Index] = liveIn;

                if (true == schedule)
                {
                    // Add all branch targets to the work list
                    foreach (BasicBlock next in block.NextBlocks)
                    {
                        // Only follow backward branches, if we've redefined a variable
                        // this may force us to reinsert a PHI function in a block we
                        // already have completed processing on.
                        workList.Enqueue(new WorkItem(next, block, liveIn));
                    }
                }
            }

        }

        private bool TransformToSsaForm(BasicBlock block, BasicBlock caller, IDictionary<StackOperand, StackOperand> liveIn, out IDictionary<StackOperand, StackOperand> liveOut)
        {
            // Is this another entry for this block?
            IDictionary<StackOperand, StackOperand> liveOutPrev = _liveness[block.Index];
            if (null != liveOutPrev)
            {
                // FIXME: Merge PHIs with new incoming variables, add new incoming variables to out set
                // and schedule the remaining out nodes/quit
                MergePhiInstructions(block, caller, liveIn);                
                liveOut = liveOutPrev;

                return false;
            }
            else
            {
                // Is this a dominance frontier block?
                if (-1 != Array.IndexOf<BasicBlock>(_dominanceFrontierBlocks, block))
                {
                    InsertPhiInstructions(block, caller, liveIn);
                }

                // Create a new live out dictionary
                if (null != liveIn)
                    liveOut = new Dictionary<StackOperand, StackOperand>(liveIn, s_comparer);
                else
                    liveOut = new Dictionary<StackOperand, StackOperand>(s_comparer);

                // Iterate each instruction in the block
                foreach (Instruction instruction in block.Instructions)
                {
                    // Replace all operands with their current SSA version
                    UpdateUses(instruction, liveOut);

                    // Is this an instruction we ignore?
                    if (false == instruction.Ignore)
                    {
                        RenameStackOperands(instruction, liveOut);
                    }
                }
            }

            return true;
        }

        private void MergePhiInstructions(BasicBlock block, BasicBlock caller, IDictionary<StackOperand, StackOperand> liveIn)
        {
            foreach (Instruction instruction in block.Instructions)
            {
                IR.PhiInstruction phi = instruction as IR.PhiInstruction;
                if (null != phi)
                {
                    StackOperand value = liveIn[phi.Result];
                    if (false == phi.Contains(value) && phi.Result.Version != value.Version)
                        phi.AddValue(caller, value);
                }
            }
        }

        private void InsertPhiInstructions(BasicBlock block, BasicBlock caller, IDictionary<StackOperand, StackOperand> liveIn)
        {
            // Iterate all incoming variables
            int pos = 0;
            foreach (StackOperand key in liveIn.Keys)
            {
                IR.PhiInstruction phi = (IR.PhiInstruction)_architecture.CreateInstruction(typeof(IR.PhiInstruction), key);
                phi.AddValue(caller, key);
                block.Instructions.Insert(pos++, phi);
            }
        }

        private void RenameStackOperands(Instruction instruction, IDictionary<StackOperand, StackOperand> liveOut)
        {
            // Create new SSA variables for newly defined operands
            Operand[] ops = instruction.Results;
            for (int opIdx = 0; opIdx < ops.Length; opIdx++)
            {
                // Is this a stack operand?
                StackOperand op = ops[opIdx] as StackOperand, ssa = null;
                if (null != op)
                {
                    Debug.Write("\tStore to " + ops[opIdx].ToString());
                    if (false == liveOut.TryGetValue(op, out ssa))
                        ssa = op;

                    ssa = RedefineOperand(ssa);
                    liveOut[op] = ssa;

                    instruction.SetResult(opIdx, ssa);
                    Debug.WriteLine(" redefined as " + ssa.ToString());
                }
            }
        }

        private void UpdateUses(Instruction instruction, IDictionary<StackOperand, StackOperand> liveOut)
        {
            Operand[] ops = instruction.Operands;
            for (int opIdx = 0; opIdx < ops.Length; opIdx++)
            {
                // Is this a stack operand?
                StackOperand op = ops[opIdx] as StackOperand, ssa = null;
                if (null != op)
                {
                    Debug.Write("Using " + ops[opIdx].ToString());

                    // Determine the most recent version
                    Debug.Assert(true == liveOut.TryGetValue(op, out ssa), @"Stack operand not in live variable list.");
                    ssa = liveOut[op];

                    // Replace the use with the most recent version
                    Debug.WriteLine(" has been replaced by " + ssa.ToString());
                    instruction.SetOperand(opIdx, ssa);
                }
            }
        }



        class StackOperandComparer : IEqualityComparer<StackOperand>
        {
            #region IEqualityComparer<StackOperand> Members

            bool IEqualityComparer<StackOperand>.Equals(StackOperand x, StackOperand y)
            {
                return (Math.Abs(x.Offset.ToInt32()) == Math.Abs(y.Offset.ToInt32()));
            }

            int IEqualityComparer<StackOperand>.GetHashCode(StackOperand obj)
            {
                return Math.Abs(obj.Offset.ToInt32());
            }

            #endregion // IEqualityComparer<StackOperand> Members
        }

        private static readonly IEqualityComparer<StackOperand> s_comparer = new StackOperandComparer();

        private StackOperand RedefineOperand(StackOperand cur)
        {
            StackOperand result = (StackOperand)cur.Clone();
            result.Version = ++_ssaVersion;
            return result;
        }

        #endregion // IMethodCompilerStage Members
    }
}