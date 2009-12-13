namespace System.Threading.Parallel
{
	/// <summary>
	/// 	<para>A parallel loop over iteration values of type <typeparamref name="T" />.</para>
	/// </summary>
	/// <remarks>
	/// 	<para>The abstract generic class, <see cref="T:System.Threading.Parallel.ParallelLoop{T}" />, abstracts common behavior of the the loop classes that iterate over values of type <typeparamref name="T" />. Its derived classes differ in how the iteration space is defined.</para>
	/// 	<para>Iteration commences once the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> method is called. The callback is applied to each iteration value. A conforming implementation can use the thread calling the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> method to execute all iterations, regardless of the value of <see cref="P:System.Threading.Parallel.ParallelLoop{T}.MaxThreads" />. The thread that calls the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> method shall call the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method to block until all iterations complete or are cancelled. When <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> is called, the calling thread can be employed as a worker thread.</para>
	/// 	<para>Calling the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Run" /> method is equivalent to calling <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> followed by calling <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" />.</para>
	/// 	<para>A parallel loop can be cancelled at any time (even before it starts running) by calling <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" />. Cancellation is asynchronous in the sense that <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" /> can return while portions of the loop are still running. Any number of threads can call <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" /> on the same object. Cancellation affects only iterations that have not yet been issued to worker threads. Outstanding iterations are completed normally.</para>
	/// 	<para>If one or more invocations of a callback throws an unhandled exception, the rest of the loop is cancelled. One of the exceptions is saved inside the <see cref="T:System.Threading.Parallel.ParallelLoop<T>" /> until the loop has stopped running, and then the saved exception is rethrown when method <see cref="M:System.Threading.Parallel.ParallelLoop<T>.EndRun" /> is invoked. In the case of multiple exceptions, the implementation can choose any one of the exceptions to save and rethrow.</para>
	/// </remarks>
	[Serializable]
	public abstract class ParallelLoop<T> : IDisposable
	{		
		internal Action<T> action;
		internal int maxThreads;
		internal int workThreads;
		internal int threadLimit;
		internal readonly WaitCallback callback;
		private Exception thrownException;
		private readonly AutoResetEvent reset;
		
		public void Dispose()
		{
			
		}
		
		internal ParallelLoop(int numThreads)
		{
			if(numThreads < 0)
			{
				throw new ArgumentException("numThreads must be non-negative");
			}
			this.maxThreads = (numThreads == 0 ? ParallelEnvironment.MaxThreads : numThreads);
			if(this.maxThreads > 1)
			{
				reset = new AutoResetEvent(false);
			}
			callback = new WaitCallback(ThreadTask);
		}
	
		/// <summary>
		/// 	<para>Begin executing iterations, applying the action delegate to each iteration value.</para>
		/// </summary>
		/// <param name="action">The <see cref="T:System.Delegate" /> to apply to each iteration value.</param>
		/// <remarks>
		/// 	<para>This method is not thread safe. It should be called only once for a given instance of <see cref="T:System.Threading.Parallel.ParallelLoop{T}" />.</para>
		/// 	<para>If one or more invocations of a callback throws an unhandled exception, the rest of the loop is canceled. One of the exceptions is saved inside the <see cref="T:System.Threading.Parallel.ParallelLoop{T}" /> until the loop has stopped running, and then the saved exception is rethrown when the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method is invoked. In the case of multiple exceptions, the implementation can choose any one of the exceptions to save and rethrow.</para>
		/// 	<para>[Note: Implemenation, particularly on single-threaded hardware, are free to employ the calling thread to execute all loop iterations.</para>
		/// 	<para>[Note: The return value is void, not <see cref="T:System.IAsyncResult" />, and there is no callBack or stateObject arguments. This departure from the usual asynchronous call pattern (e.g. <see cref="M:System.IO.FileStream.BeginRead" />) is deliberate, because in typical scenarios the extra complexity would just add pointless burden on the implementation.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="action" /> is null.</exception>
		public abstract void BeginRun(Action<T> action);
		
		/// <summary>
		/// 	<para>Eventually cancel issuance of any further iterations.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>A <see cref="T:System.Threading.Parallel.ParallelLoop{T}" /> can be cancelled at any time (even before it starts running) by calling the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" /> method. Cancellation is asynchronous in the sense that <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" /> can return while portions of the loop are still running. Any number of threads can concurrently call <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Cancel" /> on the same object. Cancellation affects only iterations that have not yet been issued to worker threads. Outstanding iterations are completed normally.</para>
		/// </remarks>
		public abstract void Cancel();
		
		/// <summary>
		/// 	<para>Wait until all iterations are finished (or cancelled).</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method is not thread safe. It should be called exactly once by the thread that called <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" />.</para>
		/// </remarks>
		public void EndRun()
		{
			if( threadLimit > 1 )
			{
				NewThreadTask(true);		// if no threads running yet, start them up
			}
			threadLimit = 0;			// all tasks, complete..
			if (thrownException != null)
			{
				throw thrownException;	// if something failed, throw the last exception recieved
			}
		}
		
		/// <summary>
		/// 	<para>Start processing of loop iterations and wait until done.</para>
		/// </summary>
		/// <param name="action">The <see cref="T:System.Delegate" /> applied to each iteration value</param>
		/// <remarks>
		/// 	<para>This method is equivalent to calling <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> followed by calling <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" />.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException"><paramref name="action" /> is null.</exception>
		public void Run(Action<T> action)
		{
			BeginRun(action);
			EndRun();
		}
		
		internal abstract void Process();
		
		private void ThreadTask(object notUsed)
		{
			try
			{
				try
				{
					Process();			// Process the data
				}
				catch(Exception e)
				{
					lock(callback)		//  if error, lock processing
					{
						thrownException = e;	// get exception thrown
					}
					Cancel();			// cancel remaining work items
				}
			}
			finally
			{
				if(Threading.Interlocked.Decrement(ref workThreads) == 0)
				{
					if(reset != null)
						reset.Set();
				}
			}
		}
		
		internal void NewThreadTask(bool forMasterThread)
		{
			int curWorking;
			int oldWorking = workThreads;
			do
			{
				curWorking = oldWorking;
				int workLimit = (threadLimit - 1) + Convert.ToInt32(forMasterThread);
				if (curWorking >= workLimit) 
				{
					return;
				}
				oldWorking = Interlocked.CompareExchange(ref workThreads, curWorking + 1, curWorking);
			} while (oldWorking != curWorking);
			if (forMasterThread)
			{
				ThreadTask(null);	// Start Up The Task on current thread
				if(reset != null)
					reset.WaitOne();
			} else 
			{
				ThreadPool.QueueUserWorkItem(callback);		// Startup task on new thread
			}
		}

		internal void BaseBeginRun(Action<T> action, int numThreads )
		{
			if (action==null)
				throw new ArgumentNullException("action is null");

			this.action = action;
			
			if (threadLimit != 0)
				throw new InvalidOperationException("Parallel loop is already running");

			threadLimit = Math.Min(maxThreads, numThreads);
			
			if (threadLimit == 1)
			{
				NewThreadTask(true);		// 1 Thread max, no parallel
			} 
			else 
			{
				Thread.VolatileWrite(ref workThreads, threadLimit - 1);
				for (int i = 1; i < threadLimit; ++i)
					ThreadPool.QueueUserWorkItem(callback);		// Start up some threads X-D
			}
		}
	}
}
