
using System;
using CSharpInteractive.Core;


public static class Program
{
	private static void _Init()
	{
		Interactive.Init();
	}

	private static void _UnInit()
	{
		Interactive.UnInit();
	}
	
	private static void _PrintHelpMessage()
	{
		Console.Out.WriteLine("===================================");
		Console.Out.WriteLine("welcome c# interactive interpreter.");
		Console.Out.WriteLine("===================================");
		Console.Out.WriteLine();
	}

	private static void _PrintPrefix()
	{
		Console.Out.Write("#:-> ");
	}

	private static string _ReadUserInput()
	{
		return Console.In.ReadLine();
	}

	private static string _Process(string input)
	{
		return input;
	}
	
	private static void _PrintResult(string result)
	{
		Console.Out.WriteLine(result);
	}

	private static void _PrintExitMessage()
	{
		Console.Out.WriteLine("\nExiting...");
	}
	
	
	public static void Main()
	{
		_PrintHelpMessage();
		_Init();
		while (true)
		{
			_PrintPrefix();
			var input = _ReadUserInput();
			if (input == null)
			{
				break;
			}
			
			var result = _Process(input);
			_PrintResult(result);
		}

		_UnInit();
		_PrintExitMessage();
	}
	
}