// NOTE: This should be in mscorlib.dll

namespace System.Threading.Parallel
{
	/// <summary>
	/// 	<para>Provides the current settings for, and information about, the parallel-loop execution environment.</para>
	/// </summary>
	public sealed class ParallelEnvironment
	{
		private static int maxThreads;
		private static int recommendedMaxThreads;
		
		static ParallelEnvironment()
		{
			// TODO: this should get the number of hardware threads from the processor and put it in recommendedMaxThreads
			//		this can be done using the Kernel API or CPUID
			//		but for now, set it to the number of processors
			recommendedMaxThreads = Environment.ProcessorCount;
			maxThreads = recommendedMaxThreads;
		}
		
		/// <summary>
		/// 	<para>Default upper bound on the number of threads employed by a parallel loop.</para>
		/// </summary>
		/// <value>
		/// 	<para>A <see cref="T:System.Int32" /> that limits the number of worker threads employed by parallel loop constructs that do not explicitly specify an upper bound on the number of threads. The bound includes the thread that calls <see cref="M:System.Threading.Parallel.ParallelLoop{T}.BeginRun" />, and hence MaxThreads must be positive.</para>
		/// </value>
		/// <remarks>
		/// 	Description
		/// 		<para>Setting <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" /> to 1 causes deterministic sequential execution of all parallel loop constructs that do not explicitly specify an upper bound on the number of threads. This is useful for debugging of code. Ordinarily, <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" /> should not be set in production code because it affects parallel loops everywhere in a program.</para>
		/// 		<para>The initial value is <see cref="P:System.Threading.Parallel.ParallelEnvironment.RecommendedMaxThreads" />.</para>
		/// </remarks>
		public static int MaxThreads
		{
			get
			{
				return maxThreads;
			}
			set
			{
				if(value < 1)
					throw new ArgumentException("MaxThreads must be positive");
				maxThreads = value;
			}
		}
		
		/// <summary>
		/// 	<para>Recommended value for <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" />.</para>
		/// </summary>
		/// <value>
		/// 	<para>A <see cref="T:System.Int32" /> that is the initial value for <see cref="P:System.Threading.Parallel.ParallelEnvironment.MaxThreads" />.</para>
		/// </value>
		/// <remarks>
		/// 	<para>Values between 1x and 2x the number of physical threads on the platform are recommended.</para>
		/// </remarks>
		public static int RecommendedMaxThreads
		{
			get
			{
				return maxThreads;
			}
		}
	}
}
