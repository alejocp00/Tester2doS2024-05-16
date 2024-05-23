
using MatCom.Utils;
using MatCom.Tester;
using System.Diagnostics;

Directory.CreateDirectory(".output");
File.Delete(Path.Combine(".output", "result.md"));
File.WriteAllLines(Path.Combine(".output", "result.md"), new[]
{
    "# Resultados del TC aplicado el 16-05-2024",
    "",
    "| Estudiante | Aprobado | Problemas Resueltos |",
    "|------------|----------|---------------------|"
});

foreach (var solution in Directory.GetFiles("solutions", "*.cs"))
{
    var oldFiles = Directory
        .EnumerateFiles("tester", "*.*", SearchOption.AllDirectories)
        .Where(f => Path.GetFileName(f) != "tester.csproj")
        .Where(f => Path.GetFileName(f) != "Base.cs")
        .Where(f => Path.GetFileName(f) != "UnitTest.cs")
        .Where(f => Path.GetFileName(f) != "Utils.cs");

    foreach (var oldFile in oldFiles) File.Delete(oldFile);

    File.Copy(solution, Path.Combine("tester", "Solution.cs"));

    var name = Path.GetFileNameWithoutExtension(solution);

    var (student, group) = SplitName(name);

    Console.WriteLine($"--Testing {student}--");

    // Create the arguments for dotnet test, that allow the test run and stop one test passed 2 minutes, but only one test
    var info = new ProcessStartInfo("dotnet", "test --logger trx --blame-hang-timeout 2min");

    var process = Process.Start(info);

    process?.WaitForExit();

    var dict = new Dictionary<TestType, List<bool>>();

    try
    {
        var trx = Directory.GetFiles("tester/TestResults", "*.trx").Single();
        File.Copy(trx, Path.Combine(".output", $"{name}.trx"));
        dict = TestResult.GetResults($".output/{name}.trx");
        //Directory.Delete("Tester/TestResults", true);
    }
    catch (TimeoutException)
    {
        File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
        {
            $"| {student} | {( "🔴" )} | { "⌚" } |"
        });
        continue;
    }
    catch
    {
        File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
        {
            $"| {student} | {( "🔴" )} | { "⚠️" } |"
        });

        continue;
    }
    var solved = SolvedProblems(dict);
    File.AppendAllLines(Path.Combine(".output", "result.md"), new[]
    {
        $"| {student} {group}| {( TestResult.IsApproved(dict) ? "🟢" : "🔴" )} | {(solved.Item2==100?"":"⌚")}{solved} | "

    });

    File.Delete($".output/{name}.trx");

    Console.WriteLine("--Done--");
}

foreach (var file in Directory.GetFiles("solutions/base", "*.cs"))
{
    File.Copy(file, Path.Combine("tester", Path.GetFileName(file)), true);
}

Directory.GetFiles(".output", "*.trx").ToList().ForEach(File.Delete);

static Tuple<int, int> SolvedProblems(Dictionary<TestType, List<bool>> dict)
{
    int total = dict[TestType.SolvingProblems].Count;
    int solved = dict[TestType.SolvingProblems].Count(x => x);

    return new Tuple<int, int>(solved, total);

}

static Tuple<string, string> SplitName(string fileName)
{
    string name = "";
    string group = "";

    for (int i = 0; i < fileName.Length; i++)
    {
        // Verifica si la letra es c
        if ((fileName[i] == 'c' || fileName[i] == 'C') && ((i + 1 < fileName.Length) && (char.IsNumber(fileName[i + 1]) || fileName[i + 1] == '-')))
        {
            group += "C";
        }
        else if (char.IsNumber(fileName[i]))
        {
            group += fileName[i];
        }
        else if (char.IsLetter(fileName[i]))
        {
            if (char.IsUpper(fileName[i]) && name.Length > 0)
            {
                name += " ";
            }
            name += fileName[i];
        }
    }
    return new Tuple<string, string>(name, group);
}