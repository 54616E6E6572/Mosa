using System.Collections.Generic;

namespace System.Threading.Parallel
{
	/// <summary>
	/// 	<para>A parallel while loop over iteration values of type <typeparamref name="T" />.</para>
	/// </summary>
	/// <remarks>
	/// 	Description
	/// 		<para>The <see cref="T:System.Threading.Parallel.ParallelWhile{T}" /> class provides a simple way to establish a pool of work to be distributed among multiple threads, and to wait for the work to complete before proceeding.</para>
	/// 		<para>A freshly constructed <see cref="T:System.Threading.Parallel.ParallelWhile{T}" /> has an empty pool of work items. <see cref="M:System.Threading.Parallel.ParallelWhile{T}.Add" /> adds a work item to the pool. <see cref="M:System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> activates processing of the pool. the inherited <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method waits until all work in the pool completes. The inherited  <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Run" /> method is a shorthand that combines <see cref="M:System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> and <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" />. New work can be added to the pool while it is active, hence the class corresponds roughly to a parallel while loop that continually chops away at a (possibly growing) collection until the collection becomes empty. Once the loop is running, implementations are free to make <see cref="M:System.Threading.Parallel.ParallelWhile{T}.Add" /> process the work item instead of putting it in the pool, for sake of limiting the size of the work pool. (The pool is typically a small multiple of the number of threads.) Once the pool is activated, one or more worker threads pull work items from the pool and apply the callback to each. The implementation is free to process work items in any order. The inherited  <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method blocks until the pool is empty and all pending invocations of the callback have returned. An iteration should not cause <see cref="M:System.Threading.Parallel.ParallelWhile{T}.Add" /> to be called after the iteration finishes (e.g. by use of yet another thread), otherwise a race condition ensues in which <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> might return prematurely even though there is more work to be done.</para>
	/// 		<para>A conforming implementation is allowed to execute serially, by using the thread that calls <see cref="System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> to process all pending work items that are added before <see cref="System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> returns, and using the thread that calls <see cref="System.Threading.Parallel.ParallelLoop{T}.EndRun" /> to process all pending work items that are added after <see cref="System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> returned and before <see cref="System.Threading.Parallel.ParallelWhile{T}.EndRun" /> returns.</para>
	/// </remarks>
	[Serializable]
	public sealed class ParallelWhile<T> : ParallelLoop<T>
	{
		private Stack<T> lifo;		// items can be added to the list, so execute newest data first, as it gives higher chance of finishing first
		private int limit;			// some systems may have to limit the stack...
		
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelWhile{T}" /> with an initially empty collection of work items.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>The loop does not start executing until at least method <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> is called and possibly not until method <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> is called.</para>
		/// </remarks>
		public ParallelWhile() : this(0)
		{
			
		}
		
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelWhile{T}" /> with an initially empty collection of work items.</para>
		/// </summary>
		/// <param name="numThreads">The maximum number of threads to use.</param>
		/// <remarks>
		/// 	<para>The loop does not start executing until at least method <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" /> is called and possibly not until method <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> is called.</para>
		/// 	<para>If <paramref name="numThreads" /> is 0, then up to <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" /> are used instead. The value includes the thread that created the <see cref="T:System.Threading.Parallel.ParallelFor{T}" />, hence using <paramref name="numThreads" />=1 causes sequential execution.</para>
		/// </remarks>
		public ParallelWhile(int numThreads) : base(numThreads)
		{
			lifo = new Stack<T>();
			limit = Int32.MaxValue;
			// TODO: this should be based off of available memory resources
			//		possibly determine through garbage pressure?
		}
		
		/// <summary>
		/// 	<para>Add a work item.</para>
		/// </summary>
		/// <param name="item">The value for an iteration.</param>
		/// <remarks>
		/// 	<para>This method can be called before or after <see cref="M:System.Threading.Parallel.ParallelWhile{T}.BeginRun" /> is called.</para>
		/// 	<para>This method is always thread safe.</para>
		/// </remarks>
		public void Add(T item)
		{
			int size = 0;
			lock(lifo)					// lock the stack
			{
				size = lifo.Count;		// get the current size
				if(size < limit)		// if we have room... 
					lifo.Push(item);		// push it onto the todo list
			}
			if(action != null)
			{
				if(size < limit)
					NewThreadTask(false);		// the number of work items is dynamic, so determine if we need another thread
				else
					action(item);			// if not, process the latest item
			}
		}
		
		/// <summary>
		/// 	<para>Begin processing work items.</para>
		/// </summary>
		/// <param name="action">The <see cref="T:System.Delegate" /> that processes each work item.</param>
		/// <remarks>
		/// 	<para>This method is not thread safe. It should be called only once for a given instance of <see cref="T:System.Threading.Parallel.ParallelWhile{T}" />.</para>
		/// 	<para>[Note: Implementations, particularly on single-threaded hardware, are free to employ the calling thread to execute all loop iterations.]</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="action" /> is null.</exception>
		public override void BeginRun(Action<T> action)
		{
			if(maxThreads > 1)
				limit = 4 * Math.Min(maxThreads, Int32.MaxValue / 4);		// Determine the best stack size limit, based off existing threads
			else
				limit = 0;													// no threads... no stack...
			
			BaseBeginRun(action, lifo.Count);
		}
		
		/// <summary>
		/// 	<para>Cancel any interations that have not yet started.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method is safe to call concurrently on the same instance.</para>
		/// 	<para>It does not cancel any future iterations that can be added.</para>
		/// </remarks>
		public override void Cancel()
		{
			threadLimit = 0;;	// finish the currently executing work items
			lock(lifo)
			{
				lifo.Clear();	// and clear the stack of future items
			}
		}
		
		/// <summary>
		/// 	<para>Process the while-loop using parallel threads and OpenMP 'Guided Scheduling'.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method overrides <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Process" />.</para>
		/// </remarks>
		internal sealed override void Process()
		{
			for(;;)				// do until complete
			{
				T item;
				lock(lifo)		// lock the stack
				{
					if(lifo.Count == 0)		// if no more work items, quit
						return;
					item = lifo.Pop();		// otherwise, get next item
				}
				action(item);				// and process it
			}
		}
	}
}
