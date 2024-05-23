
using Xunit;
using System.Linq;
using Weboo.Examen;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MatCom.Tester;

public enum TestType
{
    SolvingProblems
}

public static class Utils
{
    public static int[] SolveProblem(int[] glassForCitys, int maxGasol, int[,] map)
    {
        return Solution.Solve(glassForCitys, maxGasol, map);
    }

    public static int CountDemands(int[] glassForCitys, int[,] map, int[] path)
    {
        bool[] visited = new bool[glassForCitys.Length];
        int count = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            if (!visited[path[i]])
            {
                visited[path[i]] = true;
                count += glassForCitys[path[i]];
            }
        }
        return count;
    }
}

public class ProblemGenerator
{
    public int Seed { get; }
    private Random randomCityGenerator { get; }

    public ProblemGenerator(int seed)
    {
        Seed = seed;
        randomCityGenerator = new Random(seed);

    }

    private int[] GenerateDemands(int cities)
    {
        int[] demands = new int[cities];
        int maxDemand = 1000;
        for (int i = 0; i < cities; i++)
        {
            demands[i] = randomCityGenerator.Next(maxDemand);
        }

        return demands;
    }

    private int[,] GetMap(int minGasolineCost, int maxGasolineCost, int citiesCount)
    {
        var map = new int[citiesCount, citiesCount];
        // Generate a map with random gasoline costs, but 0 for the same city and symmetric values
        for (int i = 0; i < citiesCount; i++)
        {
            for (int j = i; j < citiesCount; j++)
            {
                if (i == j)
                {
                    map[i, j] = 0;
                    continue;
                }
                if (map[i, j] != 0)
                {
                    continue;
                }
                var cost = randomCityGenerator.Next(minGasolineCost, maxGasolineCost);
                map[i, j] = cost;
                map[j, i] = cost;
            }
        }
        int maxDisconnections = randomCityGenerator.Next(1, citiesCount);
        int disconnections = randomCityGenerator.Next(0, maxDisconnections);


        for (int i = 0; i < disconnections; i++)
        {

            int city1, city2;
            do
            {
                city1 = randomCityGenerator.Next(0, citiesCount);
                city2 = randomCityGenerator.Next(0, citiesCount);
            } while (city1 == city2 || AreDisconnectedCities(map));
            map[city1, city2] = -1;
            map[city2, city1] = -1;
        }

        return map;
    }

    private bool AreDisconnectedCities(int[,] map)
    {
        var visited = new bool[map.GetLength(0)];
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;

        while (queue.Count > 0)
        {
            var currentCity = queue.Dequeue();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                if (map[currentCity, i] > 0 && !visited[i])
                {
                    visited[i] = true;
                    queue.Enqueue(i);
                }
            }
        }

        return visited.Any(x => x == false);
    }

    private int GetMaxGasoline(int[,] map)
    {
        //Find the second largest value in the map
        int max = 0;
        int secondMax = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] > max)
                {
                    secondMax = max;
                    max = map[i, j];
                }
                else if (map[i, j] > secondMax && map[i, j] != max)
                {
                    secondMax = map[i, j];
                }
            }
        }

        int maxGasoline = secondMax * (map.GetLength(0) - 1);


        return randomCityGenerator.Next(0, maxGasoline);



    }

    public Tuple<int[], int, int[,]> GetProblem(int minGasolineCost, int maxGasolineCost, int citiesCount)
    {
        var demands = GenerateDemands(citiesCount);
        var map = GetMap(minGasolineCost, maxGasolineCost, citiesCount);
        var maxGasoline = GetMaxGasoline(map);

        return new Tuple<int[], int, int[,]>(demands, maxGasoline, map);
    }
}

public class ProblemGestor
{
    public int Seed { get; }

    private List<Tuple<int[], int, int[,]>> problems { get; }
    public ProblemGestor(int seed)
    {
        Seed = seed;
        problems = new List<Tuple<int[], int, int[,]>>();
    }

    public List<Tuple<int[], int, int[,]>> GetProblems(int amount, int minGasolineCost, int maxGasolineCost, int citiesCount)
    {

        if (problems.Count > 0)
        {
            return CloneProblems(problems);
        }

        for (int i = 0; i < amount; i++)
        {
            var generator = new ProblemGenerator(Seed + i);
            problems.Add(generator.GetProblem(minGasolineCost, maxGasolineCost, citiesCount));
        }
        return CloneProblems(problems);
    }

    private List<Tuple<int[], int, int[,]>> CloneProblems(List<Tuple<int[], int, int[,]>> inProblems)
    {
        var result = new List<Tuple<int[], int, int[,]>>();
        foreach (var element in inProblems)
            result.Add(new Tuple<int[], int, int[,]>((int[])element.Item1.Clone(), element.Item2, (int[,])element.Item3.Clone()));
        return result;
    }

    public void ExportProblems(string path)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("seed", Seed);
        data.Add("problems", problems);
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }

    private static int[] Solve(int[] glassForCitys, int maxGasol, int[,] map)
    {
        bool[] visited = new bool[glassForCitys.Length];
        int maxDemandas = 0;
        return Solve(map, glassForCitys, maxGasol, visited, ref maxDemandas, 0, 0, new List<int>(), new List<int>()).ToArray();
    }

    private static List<int> Solve(int[,] map, int[] glassForCitys, int maxGasol, bool[] visited, ref int maxDemandas,
    int demandas, int actualPos, List<int> actual, List<int> camino)
    {
        actual.Add(actualPos);

        if (actualPos == 0)
        {
            maxDemandas = Math.Max(maxDemandas, demandas);

            if (maxDemandas == demandas)
                CopyList(actual, camino);
        }

        for (int i = 0; i < glassForCitys.Length; i++)
        {
            if (map[actualPos, i] > 0 && map[actualPos, i] <= maxGasol)
            {
                if (!visited[i])
                {
                    visited[i] = true;
                    camino = Solve(map, glassForCitys, maxGasol - map[actualPos, i], visited, ref maxDemandas,
                    demandas + glassForCitys[i], i, actual, camino);
                    visited[i] = false;
                }

                else
                {
                    camino = Solve(map, glassForCitys, maxGasol - map[actualPos, i], visited, ref maxDemandas,
                    demandas, i, actual, camino);
                }
            }
        }

        actual.RemoveAt(actual.Count - 1);

        return camino;
    }

    private static void CopyList(List<int> actual, List<int> camino)
    {
        camino.Clear();

        for (int i = 0; i < actual.Count; i++)
            camino.Add(actual[i]);
    }


    public List<int[]> GetSolutions()
    {
        var results = new List<int[]>();
        foreach (var problem in problems)
        {
            results.Add(Solve(problem.Item1, problem.Item2, problem.Item3));
        }
        return results;
    }
}
