using System.Globalization;
using System.Reflection;

namespace Interactive
{
	public static class Program
    {
    	private static void _Init()
    	{
    		var customAssemblies = new List<Assembly>();
    		customAssemblies.Add(Assembly.GetExecutingAssembly());
    
    		var customNamespaces = new List<string>();
    		customNamespaces.Add("Interactive");
    		InteractiveCore.InteractiveCore.Init(customAssemblies, customNamespaces);
    	}
    
    	private static void _UnInit()
    	{
    		InteractiveCore.InteractiveCore.UnInit();
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
    
    	private static string? _ReadUserInput()
    	{
    		return Console.In.ReadLine();
    	}
    
    	private static void _Process(string input)
    	{
		    try
		    {
			    var ret = InteractiveCore.InteractiveCore.Interpret(input);
			    Console.Out.WriteLine($"out: {ret}");
		    }
		    catch (Exception e)
		    {
			    Console.Error.WriteLine(e);
		    }
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
			    _Process(input);
    		}
    
    		_UnInit();
    		_PrintExitMessage();
    	}
    	
    }
}
