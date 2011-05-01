using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting benchmark program for NextMap");
			Console.WriteLine();

			TestBench testBench = new TestBench();

			double manualMappingTime = testBench.ManualMappingPerformance();
			double nextMapTime = testBench.NextMapPerformance();
			double autoMapperTime = testBench.AutoMapperPerformance();

			Console.WriteLine("Manual mapping time was (sec): {0}", manualMappingTime);
			Console.WriteLine("NextMap mapping time was (sec): {0}", nextMapTime);
			Console.WriteLine("AutoMapper mapping time was (sec): {0}", autoMapperTime);
			Console.WriteLine();
			Console.WriteLine("Press any key to close.");
			Console.ReadKey();
		}

        static void PizzaGrammar_SpeechRecognized(
            object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
        }
	}
}
