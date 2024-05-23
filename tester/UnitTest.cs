
using Xunit;
using System.Linq;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace MatCom.Tester;

public class UnitTest
{


    public static IEnumerable<object[]> ProblemsData()
    {
        int seed = 2024;
        int amount = 100;
        int minGasolineCost = 1;
        int maxGasolineCost = 100;
        int maxCities = 14;
        int cities = new Random(seed).Next(2, maxCities);

        var gestor = new ProblemGestor(seed);

        var problems = gestor.GetProblems(amount, minGasolineCost, maxGasolineCost, cities);
        var solutions = gestor.GetSolutions();

        for (int i = 0; i < amount; i++)
        {
            yield return new object[] { problems[i].Item1, problems[i].Item2, problems[i].Item3, solutions[i] };
        }

    }

    [Theory]
    [MemberData(nameof(ProblemsData))]
    public void SolvingProblems(int[] glassForCitys, int maxGasol, int[,] map, int[] expeted)
    {
        var result = Utils.SolveProblem(glassForCitys, maxGasol, map);

        int countResult = Utils.CountDemands(glassForCitys, map, result);
        int countExpected = Utils.CountDemands(glassForCitys, map, expeted);

        Assert.Equal(countExpected, countResult);
    }


}

