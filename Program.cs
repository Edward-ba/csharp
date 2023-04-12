using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using HelloWorld;


namespace SemanticsCS {
    class Program {
        static void Main(string[] args) {
            //string solutionPath = "../samplecs/samplecs.sln";
            // var workspace = MSBuildWorkspace.Create();
            // var solution = workspace.OpenSolutionAsync(solutionPath).Result;
            // foreach (var project in solution.Projects) {
            //     foreach (var document1 in project.Documents) {
            //         Console.WriteLine(project.Name + "\t\t\t" + document1.Name);

            //     }
            // }

            var workspace = new AdhocWorkspace();

            string projName = "NewProject";
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, versionStamp, projName, projName, LanguageNames.CSharp);
            var newProject = workspace.AddProject(projectInfo);
            var programDocument = newProject.AddDocument("Program.cs", SourceText.From(new StreamReader("../samplecs/Program.cs").ReadToEnd()));
            var document = newProject.AddDocument("Person.cs", SourceText.From(new StreamReader("../samplecs/Person.cs").ReadToEnd()));
            
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            var root = (CompilationUnitSyntax)tree.GetRoot();
            

            var compilation = CSharpCompilation.Create("HelloWorld")
                                                .AddReferences(
                                                    MetadataReference.CreateFromFile(
                                                        typeof(object).Assembly.Location))
                                                .AddSyntaxTrees(tree);

            var model = compilation.GetSemanticModel(tree);

            Console.Clear();

            var members = root.DescendantNodes().OfType<MemberDeclarationSyntax>();
            foreach (var member in members) {
                var property = member as PropertyDeclarationSyntax;
                if (property != null)
                    Console.WriteLine("Property: " + property.Identifier);
                var method = member as MethodDeclarationSyntax;
                if (method != null)
                    Console.WriteLine("Method: " + method.Identifier);
                
                var referencesToMethod = SymbolFinder.FindReferencesAsync(model.GetDeclaredSymbol(member), newProject.Solution).Result;
                foreach (var reference in referencesToMethod) {
                    foreach (var location in reference.Locations) {
                        Console.WriteLine(location.Location.GetLineSpan());
                    }
                }
            }
            
        }
    }
}