namespace System.Threading.Parallel
{
	/// <summary>
	/// 	<para>A parallel loop over consecutive integers, starting at 0</para>
	/// </summary>
	/// <remarks>
	/// 	<para>[Note: <see cref="T:System.Threading.Parallel.ParallelFor" /> provides basic parallelism over an index space known in advance. The index space is 0..(N-1) for some value of N. This is the common case in -for- loops, and one can easily derive more complex arithmetic sequences via linear transformation of the index variable.]</para>
	/// </remarks>
	[Serializable]
	public sealed class ParallelFor : ParallelLoop<int>
	{
		private int current;
		private int count;
	
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelFor" /> that will iterate over the integers 0..count-1.</para>
		/// </summary>
		/// <param name="count">The number of loop iterations.</param>
		/// <remarks>
		/// 	<para>The loop starts executing when the <see cref="M:System.Threading.Parallel.ParallelFor.BeginRun" /> method is called.</para>
		/// </remarks>
		public ParallelFor(int count) : this(count, 0)
		{
		}
		
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelFor" /> that will iterate over the integers 0..count-1.</para>
		/// </summary>
		/// <param name="count">The number of loop iterations.</param>
		/// <param name="numThreads">The maximum number of threads to use.</param>
		/// <remarks>
		/// 	<para>The loop starts executing when the <see cref="M:System.Threading.Parallel.ParallelFor.BeginRun" /> method is called.</para>
		/// 	<para>If <paramref name="numThreads" /> is 0, then up to <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" /> threads are used instead. The value of <paramref name="numThreads" /> includes the thread that created the <see cref="T:System.Threading.Parallel.ParallelFor{T}" />, hence using <paramref name="numThreads" />=1 forces sequential execution.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">The value for <paramref name="numThreads" /> is negative.</exception>
		public ParallelFor(int count, int numThreads) : base(numThreads)
		{
			if(count < 0)
				throw new ArgumentException("Iteration count must be non-negative");
				
			this.count = count;
		}
		
		/// <summary>
		/// 	<para>Begin executing iterations.</para>
		/// </summary>
		/// <param name="action">The <see cref="T:System.Delegate" /> that processes each work item.</param>
		/// <remarks>
		/// 	<para>This method is not thread safe. It should be called only once for a given instance of a <see cref="T:System.Threading.Parallel.ParallelFor" />.</para>
		/// <para>[Note: Implementations, particularly on single-threaded hardware, are free to employ the calling thread to execute all loop iterations.]</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="action" /> is null.</exception>
		public override void BeginRun(Action<int> action)
		{
			BaseBeginRun(action, count);	// Start Using The ParallelLoop Base Class
		}
		
		/// <summary>
		/// 	<para>Cancel any iterations that have not yet started.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method is safe to call concurrently on the same instance.</para>
		/// </remarks>
		public override void Cancel()
		{
			Thread.VolatileWrite(ref count, 0);		// Mark No Interations Remaining
			threadLimit = 0;					// Stop The Process() Method
		}
		
		/// <summary>
		/// 	<para>Process the for-loop using parallel threads and OpenMP 'Guided Scheduling'.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method overrides <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Process" />.</para>
		/// </remarks>
		internal sealed override void Process()
		{
            // OpenMP 'Guided Scheduling'
            int mp = (workThreads == 1 ? 1 : 4 * workThreads);
            
            for (;;)
            {
                int next;
                int tmpCurrent;
                int tmpCount;

                lock (callback)	// Lock on callback so we don't have to create extra object for lock
                {
                    tmpCurrent = this.current;	// Get temp copy, as Process() can change
                    tmpCount = this.count;		// Get temp copy, as Cancel() can change..
                    if (tmpCount <= tmpCurrent)	// if done, break out
                    	return;

                    next = Math.Max((tmpCount - tmpCurrent) / mp, 1);
                    
                    this.current = tmpCurrent + next;
                }
                for(int i = 0; i < next; ++ i)
                {
                	action(tmpCurrent + i);		// Process the data
                }
            }
        }
	}
}
