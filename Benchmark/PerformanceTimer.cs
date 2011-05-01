using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace Benchmark
{
	/// <summary>
	/// Performance timer for benchmarking processes.
	/// </summary>
	internal class PerformanceTimer
	{
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(
			out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(
			out long lpFrequency);

		private long startTime, stopTime;
		private long frequency;

		/// <summary>
		/// Instantiates a new instance of PerformanceTimer.
		/// </summary>
		public PerformanceTimer()
		{
			if (QueryPerformanceFrequency(out frequency) == false)
			{
				// high-performance counter not supported
				throw new Win32Exception();
			}
		}

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void Start()
		{
			startTime = 0;
			stopTime = 0;

			// passing control to the waiting threads, should improve lag?
			Thread.Sleep(0);

			QueryPerformanceCounter(out startTime);
		}

		/// <summary>
		/// Stop the timer and return the duration in seconds.
		/// </summary>
		/// <returns>The duration between Start() and Stop() in seconds.</returns>
		public double Stop()
		{
			QueryPerformanceCounter(out stopTime);

			double d = (stopTime - startTime);
			return d / frequency;
		}
	}
}
