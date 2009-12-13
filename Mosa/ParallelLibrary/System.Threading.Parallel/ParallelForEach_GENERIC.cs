using System.Collections.Generic;

namespace System.Threading.Parallel
{
	/// <summary>
	/// 	<para>A parallel loop over a collection containing types of <typeparamref name="T" />.</para>
	/// </summary>
	/// <remarks>
	/// 	<para>A <see cref="T:System.Threading.Parallel.ParallelForEach{T}" /> iterates over an enumerable collection. The <see cref="M:System.Threading.Parallel.ParallelForEach{T}.BeginRun" /> method activates processing of the iterations, using a callback provided. The collection shall not change while the <see cref="M:System.Threading.Parallel.ParallelForEach{T}" /> is active, otherwise the behavior is undefined. The inherited <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method blocks until all iterations are finished. The inherited <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Run" /> method is shorthand for <see cref="M:System.Threading.Parallel.ParallelForEach{T}.BeginRun" /> and <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" />.</para>
	/// 	<para>[Note: <see cref="T:System.Threading.Parallel.ParallelForEach{T}" /> is generally non-scalable in terms of parallelism, because the enumerator is inherently sequential. If the collection allows random access, consider using the <see cref="T:System.Threading.Parallel.ParallelFor" /> class instead.]</para>
	/// </remarks>
	[Serializable]
	public sealed class ParallelForEach<T> : ParallelLoop<T>
	{
		private bool isDone;
		private IEnumerator<T> collection;
	
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelForEach{T}" /> for iterating over a collection.</para>
		/// </summary>
		/// <param name="collection">The collection of value over which to iterate.</param>
		/// <remarks>
		/// 	<para>The loop does not start executing until at least the <see cref="M:System.Threading.Parallel.ParallelForEach{T}.BeginRun" /> method is called and possibly not until the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method is called.</para>
		/// </remarks>
		public ParallelForEach(IEnumerable<T> collection) : this(collection, 0)
		{
			
		}
		
		/// <summary>
		/// 	<para>Constructs a <see cref="T:System.Threading.Parallel.ParallelForEach{T}" /> for iterating over a collection.</para>
		/// </summary>
		/// <param name="collection">The collection of values over which to iterate.</param>
		/// <param name="numThreads">The maximum number of threads to use.</param>
		/// <remarks>
		/// 	<para>The loop does not start executing until at least the <see cref="M:System.Threading.Parallel.ParallelForEach{T}.BeginRun" /> method is called and possibly not until the <see cref="M:System.Threading.Parallel.ParallelLoop{T}.EndRun" /> method is called.</para>
		/// 	<para>If <paramref name="numThreads" /> is 0, then up to <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" /> are used instead. The value includes the thread that created the <see cref="T:System.Threading.Parallel.ParallelFor{T}" />, hence using <paramref name="numThreads" />=1 causes sequential execution.</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentException">The value for <paramref name="numThreads" /> is negative.</exception>
		public ParallelForEach(IEnumerable<T> collection, int numThreads) : base(numThreads)
		{
			this.collection = collection.GetEnumerator();
			isDone = false;
		}
		
		/// <summary>
		/// 	<para>Begin executing iterations.</para>
		/// </summary>
		/// <param name="action">The <see cref="T:System.Delegate" /> that processes each work item.</param>
		/// <remarks>
		/// 	<para>This method is not thread safe. It should be called only once for a given instance of <see cref="T:System.Threading.Parallel.ParallelWhile{T}" />.</para>
		/// 	<para>[Note: Implementations, particularly on single-threaded hardware, are free to employ the calling thread to execute all loop iterations.]</para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException" ><paramref name="action" /> is null.</exception>
		public override void BeginRun(Action<T> action)
		{
			BaseBeginRun(action, Int32.MaxValue);	// Start Using The ParallelLoop Base Class
		}
		
		/// <summary>
		/// 	<para>Cancel any interations that have not yet started.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method is safe to call concurrently on the same instance.</para>
		/// 	<para>Does not cancel any future iterations that might be added.</para>
		/// </remarks>
		public override void Cancel()
		{
			isDone = true;		// Mark that we're done
			threadLimit = 0;;	// Cancel future iterations, they are not needed
		}
		
		/// <summary>
		/// 	<para>Process the foreach-loop using parallel threads and OpenMP 'Guided Scheduling'.</para>
		/// </summary>
		/// <remarks>
		/// 	<para>This method overrides <see cref="M:System.Threading.Parallel.ParallelLoop{T}.Process" />.</para>
		/// </remarks>
		internal sealed override void Process()
		{
			for(;;)		// Loop until complete
			{
				T item;
				lock(collection)	// Lock the collection
				{
					if(isDone)		// if another thread completed operation, quit
						return;
					if(!collection.MoveNext())		// otherwise, check if we completed operation
					{
						isDone = true;
						return;						// of so, quit
					}
					item = collection.Current;		// otherwise, get next object to process
				}
				action(item);						// and process it
			}
		}
	}
}
