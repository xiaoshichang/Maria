using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace InteractiveCore
{
	public static partial class InteractiveCore
	{
		
		public static void Init(List<Assembly> customAssemblies, List<string> customNamespaces)
		{
			var scriptOptions = ScriptOptions.Default;
			scriptOptions = scriptOptions.AddImports("System");
			foreach (var ns in customNamespaces)
			{
				scriptOptions = scriptOptions.AddImports(ns);
			}
			scriptOptions = scriptOptions.AddReferences(customAssemblies);
			
			// Initialize script with custom interactive assembly loader
			var script = CSharpScript.Create(@"", scriptOptions, null, null);
			_State = script.RunAsync().Result;
		}

		public static void UnInit()
		{
			_State = null;
		}

		public static void Interpret(string code)
		{
			_State = _State.ContinueWithAsync(code).Result;
		}

		private static ScriptState<object> _State;
	}
}