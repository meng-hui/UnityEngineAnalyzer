using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.EmptyMonoBehaviourMethods
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EmptyMonoBehaviourMethodsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> MonoBehaviourMethods = ImmutableHashSet.Create(
            "Awake",
            "FixedUpdate",
            "LateUpdate",
            "OnAnimatorIK",
            "OnAnimatorMove",
            "OnApplicationFocus",
            "OnApplicationPause",
            "OnApplicationQuit",
            "OnAudioFilterRead",
            "OnBecameInvisible",
            "OnBecameVisible",
            "OnCollisionEnter",
            "OnCollisionEnter2D",
            "OnCollisionExit",
            "OnCollisionExit2D",
            "OnCollisionStay",
            "OnCollisionStay2D",
            "OnConnectedToServer",
            "OnControllerColliderHit",
            "OnDestroy",
            "OnDisable",
            "OnDisconnectedFromServer",
            "OnDrawGizmos",
            "OnDrawGizmosSelected",
            "OnEnable",
            "OnFailedToConnect",
            "OnFailedToConnectToMasterServer",
            "OnGUI",
            "OnJointBreak",
            "OnLevelWasLoaded",
            "OnMasterServerEvent",
            "OnMouseDown",
            "OnMouseDrag",
            "OnMouseEnter",
            "OnMouseExit",
            "OnMouseOver",
            "OnMouseUp",
            "OnMouseUpAsButton",
            "OnNetworkInstantiate",
            "OnParticleCollision",
            "OnPlayerConnected",
            "OnPlayerDisconnected",
            "OnPostRender",
            "OnPreCull",
            "OnPreRender",
            "OnRenderImage",
            "OnRenderObject",
            "OnSerializeNetworkView",
            "OnServerInitialized",
            "OnTransformChildrenChanged",
            "OnTransformParentChanged",
            "OnTriggerEnter",
            "OnTriggerEnter2D",
            "OnTriggerExit",
            "OnTriggerExit2D",
            "OnTriggerStay",
            "OnTriggerStay2D",
            "OnValidate",
            "OnWillRenderObject",
            "Reset",
            "Start",
            "Update");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.EmptyMonoBehaviourMethod);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
            //NOTE: It might be more officient to find classes and then determine if they are a MonoBehaviour rather than look at every method 
        }

        private static async void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // retrieve method symbol
            var methodSymbol = context.Symbol as IMethodSymbol;
            // check if method name is a MonoBehaviour method name
            if (!MonoBehaviourMethods.Contains(methodSymbol.Name)) { return; }
            // check the syntax that has this method
            if (methodSymbol.DeclaringSyntaxReferences.Length != 1) { return; }

            // retrieve the method syntax from the method symbol
            var methodSyntax = await methodSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync() as MethodDeclarationSyntax;
            // from the method syntax, check if there is a body and if there are statements in it
            if (methodSyntax?.Body?.Statements.Any() ?? true) { return; }

            // at this point, we have a method with a MonoBehaviour method name and an empty body
            // finally, check if this method is contained in a class which extends UnityEngine.MonoBehaviour
            var containingClass = methodSymbol.ContainingType;
            var baseClass = containingClass.BaseType;
            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") &&
                baseClass.Name.Equals("MonoBehaviour"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.EmptyMonoBehaviourMethod, methodSyntax.GetLocation(), containingClass.Name, methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
