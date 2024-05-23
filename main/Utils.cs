
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MatCom.Tester;

namespace MatCom.Utils;

public static class TestResult
{
    public static Dictionary<TestType, List<bool>> GetResults(string path)
    {
        var doc = new XmlDocument();
        doc.Load(path);

        string json = JsonConvert.SerializeXmlNode(doc);

        JObject Json = JObject.Parse(json);

        var list = Json["TestRun"]!["Results"]!["UnitTestResult"]!;

        var dict = new Dictionary<TestType, List<bool>>();
        foreach (var item in list)
        {
            var testName = item["@testName"]!.ToString();

            var splited = testName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            testName = splited[3];

            testName = testName.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries).First();

            var outcome = item["@outcome"]!.ToString();
            var test = (TestType)Enum.Parse(typeof(TestType), testName);
            var result = outcome == "Passed";
            if (dict.ContainsKey(test))
            {
                dict[test].Add(result);
            }
            else
            {
                dict.Add(test, new List<bool> { result });
            }
        }

        int testsToRun = 1;
        if (dict.Count != testsToRun)
        {
            throw new TimeoutException("No se corrieron todos los tests");
        }

        return dict;
    }

    public static bool IsApproved(Dictionary<TestType, List<bool>> dict)
    {
        foreach (var item in dict)
        {
            if (item.Value.Any(x => x == false))
            {
                return false;
            }
        }
        return true;
    }
}
