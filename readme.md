UnityEngineAnalyzer
===================

This project has been archived. You should use and contribute to the Microsoft supported analyzers ([Microsoft.Unity.Analyzers](https://github.com/microsoft/Microsoft.Unity.Analyzers))

UnityEngineAnalyzer is a set of Roslyn analyzers that aim to detect common problems in Unity3D C# code. Unity3D makes it easy for us to make cross platform games, but there are hidden rules about performance and AOT, which might only come with experience, testing or reading the forums. It is hoped that such problems can be caught before compilation.


Comand Line Interface
---------------------

In order to use the Command Line Interface (CLI), download the latest release of UnityEngineAnalyzer then unzip the archive.

1. Open a Command Prompt or Powershell Window
1. Run `UnityEngineAnalyzer.CLI.exe <project path>`
1. Observe the analysis results
1. (Optional) In the same location as the project file are `report.json` and `UnityReport.html` files containig the results of the analysis  


Example:

`> UnityEngineAnalyzer.CLI.exe C:\Code\MyGame.CSharp.csproj` 


Visual Studio Integration
-------------------------

In Visual Studio 2015, go to `Tools > Nuget Package Manager > Manage Nuget Packages for Solution...`. Search for and install `UnityEngineAnalyzer`

Configuration
-------------

Right-click `Analyzers` to modify the severity or to disable the rule completely.

![](https://raw.githubusercontent.com/meng-hui/UnityEngineAnalyzer/master/Documents/configuration.png)

Limitations
-----------

- HTML Report requires FireFox or XOR (Corss Origin Request) enabled in other browsers
- It doesn't have rules for all of [Mono's AOT Limitations](https://developer.xamarin.com/guides/ios/advanced_topics/limitations/)
- IL2CPP might change the limitations of AOT compilation

Below is a sample of all the rules available in this analyzer

```C#
// AOT0001: System.Runtime.Remoting is not suppported
using System.Runtime.Remoting;
// AOT0002: System.Reflection.Emit is not supported
using System.Reflection.Emit;
using UnityEngine;

class FooBehaviour : MonoBehaviour 
{
	void Start()
	{
		// AOT0003: Reflection only works for looking up existing types
		Type.GetType("");

		// UEA0002: Using string methods can lead to code that is hard to maintain
		SendMessage("");

		// UEA0006: Use of coroutines cause some allocations
		StartCoroutine("");
	}

	// UEA0001: Using OnGUI causes allocations and GC spikes
	void OnGUI()
	{

	}

	// UEA0003: Empty MonoBehaviour methods are executed and incur a small overhead
	void FixedUpdate()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		// UEA0004: Using CompareTag for tag comparison does not cause allocations
		if (other.tag == "")
		{

		}
	}

	void Update()
	{
		// UEA0005: Warning to cache the result of find in Start or Awake
		GameObject.Find("");
	}
}
```

License
-------

See [LICENSE](https://raw.githubusercontent.com/meng-hui/UnityEngineAnalyzer/master/LICENSE)
