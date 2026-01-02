using System;
using System.Reflection;

namespace ContextBrowser.graph.Tests;

[TestClass]
public class RoslynCodeParserTests
{
    private const string SimpleClassWithContext = @"
            namespace MyApp.Sample;

            // context: create, test
            public class ExampleClass
            {
                // context: validate, test
                public void ValidateSomething() { }
            }
        ";

    private const string ExtendedClassWithContext = @"
        namespace ContextBrowser.Samples.FourContextsSample;

        // context: create, input
        public class InputProcessor
        {
            // context: validate, input
            public bool ValidateInput(string raw)
            {
                return !string.IsNullOrWhiteSpace(raw);
            }

            // context: model, input
            public InputModel Parse(string raw)
            {
                return new InputModel { Text = raw };
            }
        }

        // context: build, transformation
        public class Transformer
        {
            // context: build, transformation
            public TransformedModel Transform(InputModel input)
            {
                return new TransformedModel { Payload = input.Text.ToUpper() };
            }
        }

        // context: update, storage
        public class StorageWriter
        {
            // context: update, storage
            public void Save(TransformedModel model)
            {
                Console.WriteLine($""[STORAGE] Saved: {model.Payload}"");
            }
        }

        // context: share, notification
        public class Notifier
        {
            // context: share, notification
            public void Notify(string channel, TransformedModel model)
            {
                Console.WriteLine($""[NOTIFY:{channel}] {model.Payload}"");
            }
        }

        // context: create, orchestration
        public class FlowOrchestrator
        {
            private readonly InputProcessor _input = new();
            private readonly Transformer _transform = new();
            private readonly StorageWriter _storage = new();
            private readonly Notifier _notifier = new();
            public readonly AnotherService Svc = new();

            // context: create, orchestration, share
            public void Run(string raw)
            {
                if(!_input.ValidateInput(raw))
                    return;

                var input = _input.Parse(raw);
                var transformed = _transform.Transform(input);

                _storage.Save(transformed);
                _notifier.Notify(""default"", transformed);

                Svc.DoSomething();
            }
        }

        // context: share, orchestration
        public class AnotherService
        {
            // context: share, orchestration
            public void DoSomething()
            {
            }
        }

        // context: model, input
        public class InputModel
        {
            public string Text { get; set; } = string.Empty;
        }

        // context: model, transformation
        public class TransformedModel
        {
            public string Payload { get; set; } = string.Empty;
        }
    ";

    private const string TwoClassesSample = @"
            namespace MyApp.Advanced;

            // context: build, alpha
            public class Alpha
            {
                // context: build, alpha
                public void BuildStuff() { }
            }

            // context: build, beta
            public class Beta
            {
                // context: validate, beta
                public void ValidateStuff() { }
            }
        ";

    private const string SimpleClassWithContextForReferenceTest = @"
                namespace Test.Ref;

                // context: create, test
                public class Service
                {
                    // context: validate, test
                    public void A() => B();

                    // context: validate, test
                    public void B() { }
                }
            ";

    // 1. Обязательно: MSTest инжектирует TestContext в это публичное свойство.
    public TestContext TestContext { get; set; }

    // 2. Статическое поле для хранения пути, чтобы к нему можно было обращаться из других методов.
    // Это поле будет установлено в AssemblyInitialize.
    private static string? _testAssemblyDirectory;

    // 3. Статический публичный метод для доступа к пути.
    public static string? GetTestAssemblyLocation()
    {
        // Проверяем, был ли путь уже установлен в AssemblyInitialize
        if (string.IsNullOrEmpty(_testAssemblyDirectory))
        {
            // Если вызывается до AssemblyInitialize или вне тестового контекста,
            // можно попробовать получить путь к текущей выполняющейся сборке.
            // Это будет путь к DLL вашего тестового проекта (например, bin\Debug\netX.0).
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        return _testAssemblyDirectory;
    }

    // 4. Метод, помеченный [AssemblyInitialize], вызывается один раз перед всеми тестами в сборке.
    // Это идеальное место для получения и сохранения пути.
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // TestContext.TestRunDirectory: Каталог, куда были развернуты файлы для тестового запуска.
        // Это самый надежный путь, так как он учитывает развертывание тестовых файлов.
        _testAssemblyDirectory = context.TestRunDirectory;

        Console.WriteLine($"[AssemblyInitialize] Test Run Directory: {_testAssemblyDirectory}");

        // Альтернативно, если вам нужен каталог, куда были развернуты исходные тестовые файлы:
        // _testAssemblyDirectory = context.DeploymentDirectory;
        // Console.WriteLine($"[AssemblyInitialize] Deployment Directory: {_testAssemblyDirectory}");
    }

    private static RoslynCodeParserOptions DefaultOptions(IEnumerable<string> customAssembliesPaths) => new(

        MemberTypes: [RoslynCodeParserMemberType.@class],
        ClassModifierTypes: [RoslynCodeParserAccessorModifierType.@public],
        MethodModifierTypes: [RoslynCodeParserAccessorModifierType.@public],
        CustomAssembliesPaths: customAssembliesPaths
    );

    private static readonly RoslynContextParserOptions ParseAllOptions = new()
    {
        ParseContexts = true,
        ParseReferences = true
    };

    private RoslynCodeParser<ContextInfo> CreateParser(out IContextCollector<ContextInfo> collector)
    {
        collector = new ContextInfoCollector<ContextInfo>();
        var classifier = new ContextClassifier();
        var factory = new ContextInfoFactory<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(classifier);
        var semanticTreeStorage = new RoslynCodeTreeModelStorage();
        var semanticModelBuilder = new SemanticTreeModelBuilder(semanticTreeStorage);

        return new RoslynCodeParser<ContextInfo>(collector, factory, processor, semanticModelBuilder);
    }

    [TestMethod]
    [DataRow(SimpleClassWithContext, 2, "ExampleClass", "create", "ValidateSomething", "validate", DisplayName = "Simple class and method context")]
    [DataRow(ExtendedClassWithContext, 15, "Transformer", "transformation", "FlowOrchestrator", "orchestration", DisplayName = "Extended class and method context")]
    public void ShouldParseContextsCorrectly(string sourceCode, int expectedCount, string class1, string context1, string class2, string context2)
    {
        var parser = CreateParser(out var collector);
        var options = new RoslynCodeParserOptions(
            MemberTypes: [RoslynCodeParserMemberType.@class],
            ClassModifierTypes: [RoslynCodeParserAccessorModifierType.@public],
            MethodModifierTypes: [RoslynCodeParserAccessorModifierType.@public],
            CustomAssembliesPaths: new List<string>() { "D:\\projects\\ascon\\ContextBrowser\\ContextBrowserTests\\bin\\Debug\\net8.0" }
        );

        var contextOptions = new RoslynContextParserOptions
        {
            ParseContexts = true,
            ParseReferences = true
        };

        parser.ParseCode(sourceCode, options, contextOptions);
        var all = collector.GetAll().ToList();

        Assert.AreEqual(expectedCount, all.Count);
        Assert.IsTrue(all.Any(c => c.Name == class1 && c.Contexts.Contains(context1)));
        Assert.IsTrue(all.Any(c => c.Name == class2 && c.Contexts.Contains(context2)));
    }

    [TestMethod]
    [DataRow(ExtendedClassWithContext, "Run", "ValidateInput")]
    [DataRow(SimpleClassWithContextForReferenceTest, "A", "B")]
    public void ShouldTrackReferencesBetweenMethods(string source, string classA, string classB)
    {
        var paths = new List<string?>() { GetTestAssemblyLocation() }.OfType<string>();

        var parser = CreateParser(out var collector);
        parser.ParseCode(source, DefaultOptions(paths), ParseAllOptions);

        var all = collector.GetAll().ToList();
        var methodA = all.FirstOrDefault(c => c.Name == classA);
        var methodB = all.FirstOrDefault(c => c.Name == classB);

        Assert.IsNotNull(methodA);
        Assert.IsNotNull(methodB);
        Assert.IsTrue(methodA.References.Contains(methodB));
        Assert.IsTrue(methodB.InvokedBy.Contains(methodA));
    }
}