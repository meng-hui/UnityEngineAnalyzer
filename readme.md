UnityEngineAnalyzer
===================

UnityEngineAnalyzer is a set of Roslyn analyzers that aim to detect common problems in Unity3D C# code. Unity3D makes it easy for us to make cross platform games, but there are hidden rules about performance and AOT, which might only come with experience, testing or reading the forums. It is hoped that such problems can be caught before compilation.


Auto Add UnityEngineAnalyzer to your Unity project
---------------------
Each time your refresh your unity project, the UnityEngineAnalyzer may be removed from the solution.
To auto add it to your solution, you can:
1. copy Editor/AutoAddUnityEngineAnalyzer.cs to your unity project Editor folder;
1. modify `public const string UnityEngineAnalyzerPath = "Tools\\VisualStudio\\UnityEngineAnalyzer\\UnityEngineAnalyzer.dll";` to your actual UnityEngineAnalyzer.dll path 

Implemented Analyzer
---------------------
- Prefered to sealed the inherited class; 应对子类进行sealed
- struct should implement IEquatable, should override GetHashCode(); struct应实现IEquatable、应override GetHashCode()
- should cache delegate to avoid gc; 应cache delegate以防止产生gc
- avoid new object in Update() to avoid gc; 避免在Update()函数里new对象产生gc
- avoid boxing; 调用函数时应避免box产生gc
- should not use enum for generic; 泛型避免使用enum
- should use hash for Animator interfaces; Animator接口避免使用String，应使用Hash
- should use hash for Material interfaces; Material接口避免使用String，应使用Hash
- avoid calling Camera.main in Update-like message function; 避免Update()函数里调用Camera.main
- avoid calling GameObject.Find() in Update-like message function; 避免在Update()函数里GameObject.Find()之类的函数
- should use CompareTag() but not string comparision; 应使用ComareTag()，而不是字符串比较
- avoid using coroutine; 避免使用协程
- avoid empty Update-like message function; 避免Mono空消息函数
- avoid SendMessage(); 避免SendMessage
- detect some infinite recursive call; 错误无限递归检查


Comand Line Interface
---------------------

In order to use the Command Line Interface (CLI), download the latest release of UnityEngineAnalyzer then unzip the archive (https://github.com/vad710/UnityEngineAnalyzer/releases).

1. Open a Command Prompt or Powershell Window
1. Run `UnityEngineAnalyzer.CLI.exe <project path>`
1. Observe the analysis results
1. (Optional) In the same location as the project file are `report.json` and `UnityReport.html` files containig the results of the analysis
    * Use command `-e customexporter exporter2 ...` to load custom exporters
1. (Optional) configuration file path.
    * Use command `-c configureFilePath.json` to load custom configurations
	* Configuration json, allows to enable / disable analyzers
1. (Optional) minimal severity for reports
	* Use command `-s Info/Warning/Error` to defined used minimal severity for reporting
	* Default is Warning
1.	(Optional) Unity version for check
	* Use command `-v UNITY_2017_1/UNITY_5_5/UNITY_4_0/...` to Unity version
	* For default analyzer will try to find ProjectVersion.txt file and parse version automatically.

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
