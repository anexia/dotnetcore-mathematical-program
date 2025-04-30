// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Interval;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.Result;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;
using Google.OrTools.ModelBuilder;

var allowedSolver = Enum.GetNames(typeof(IlpSolverType));


if (args.Length < 2)
{
    Console.WriteLine("Solver type and model file must be specified");
    Console.WriteLine("dotnet CommandLineTool.dll <SolverType> <ModelFile> [<TimeLimitInMilliseconds>]");
    return;
}

if (allowedSolver.Contains(args[0]) && File.Exists(args[1]) && Enum.TryParse<IlpSolverType>(args[0], out var type))
{
    var timeLimit = 1000u * 60 * 60 * 3;

    if (args.Length == 3 && uint.TryParse(args[2], out var inputTimeLimit))
    {
        timeLimit = inputTimeLimit;
    }

    Console.WriteLine($"Set TimeLimit to {timeLimit} ms. ({TimeSpan.FromMilliseconds(timeLimit):c}))");

    var sw = new Stopwatch();
    sw.Start();

    if (type == IlpSolverType.CbcIntegerProgramming)
    {
        Cbc(args[1], timeLimit);
        return;
    }

    var result = new IlpSolver(type).Solve(new ModelAsMpsFormat(File.ReadAllText(args[1])), new SolverParameter(
        EnableSolverOutput.True,
        RelativeGap.EMinus7, new TimeLimitInMilliseconds(timeLimit), new NumberOfThreads(1), null, "test.txt"));
    sw.Stop();

    Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");
    Console.WriteLine($"Status: {result.SolverResultStatus}");
    Console.WriteLine($"ObjValue: {result.ObjectiveValue}");
    Console.WriteLine($"Gap: {result.OptimalityGap}");
    Console.WriteLine($"IsOptimal: {result.IsOptimal}");
}
else
{
    Console.WriteLine($"Solver type must be one of: {string.Join(",", allowedSolver)}");
    Console.WriteLine($"Model file must exist and be readable.");
}


static void Cbc(string file, uint timeLimit)
{
    var newModel = new OptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar>();

    var vars = new Dictionary<string, IIntegerVariable<IRealScalar>>();
    var model = new Model();

    model.ImportFromMpsString(File.ReadAllText(file));

    for (int i = 0; i < model.VariablesCount(); i++)
    {
        vars.Add(model.VarFromIndex(i).Name,
            newModel.NewVariable<IntegerVariable<IRealScalar>>(new RealInterval(model.VarFromIndex(i).LowerBound,
                model.VarFromIndex(i).UpperBound), model.VarFromIndex(i).Name));
    }

    for (int i = 0; i < model.ConstraintsCount(); i++)
    {
        var ind = model.ConstraintFromIndex(i).Helper.ConstraintVarIndices(i);
        var myVars = ind.Select(index => vars[model.VarFromIndex(index).Name]);


        newModel.AddConstraint(newModel.CreateConstraintBuilder()
            .AddWeightedSum(
                myVars,
                model.ConstraintFromIndex(i).Helper.ConstraintCoefficients(i)
                    .Select(item => new RealScalar(item))
            ).Build(
                new RealInterval(model.ConstraintFromIndex(i).LowerBound, model.ConstraintFromIndex(i).UpperBound)
            ));
    }


    var b = newModel.CreateObjectiveFunctionBuilder();
    for (int i = 0; i < model.VariablesCount(); i++)
    {
        var coeff = model.VarFromIndex(0).Helper.VarObjectiveCoefficient(i);
        var vName = model.VarFromIndex(i).Name;
        b.AddTermToSum(new RealScalar(coeff), vars[vName]);
    }

    var sw = new Stopwatch();
    sw.Start();

    var timer = new System.Timers.Timer(TimeSpan.FromMinutes(10));

    Task.Run(() =>
    {
        timer.Elapsed += (s, a) => { Console.WriteLine("Time since start: " + a.SignalTime.ToShortTimeString()); };
        timer.AutoReset = true;
        timer.Start();
    });


    var result = new IlpCbcSolver().Solve(newModel.SetObjective(b.Build(false)), new SolverParameter(
        EnableSolverOutput.True,
        RelativeGap.EMinus7, new TimeLimitInMilliseconds(timeLimit), new NumberOfThreads(1)));

    sw.Stop();

    Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");
    Console.WriteLine($"Status: {result.SolverResultStatus}");
    Console.WriteLine($"ObjValue: {result.ObjectiveValue}");
    Console.WriteLine($"Gap: {result.OptimalityGap}");
    Console.WriteLine($"IsOptimal: {result.IsOptimal}");
    timer.Stop();
    timer.Dispose();
}

static void ImportLpFile()
{
    var file = File.ReadAllLines("test.txt");

    var model =
        new OptimizationModel<IIntegerVariable<IIntegerScalar>, RealScalar, IIntegerScalar>();


    var variableset = new Dictionary<string, IntegerVariable<IIntegerScalar>>();
    var constraints = false;
    var obj = false;
    var objBuilder = model.CreateObjectiveFunctionBuilder();
    foreach (var line2 in file)
    {
        var line = line2;
        var lineParts = line.Split(' ').Where(item => !item.Equals(" ") && !item.Equals(Environment.NewLine)
                                                                        && !item.Equals("")).ToArray();

        if (lineParts.First() == "Minimize")
        {
            obj = true;
            constraints = false;
        }
        else if (lineParts.First() == "Subject")
        {
            constraints = true;
            obj = false;
        }
        else if (lineParts.First() == "Bounds")
        {
            obj = false;
            constraints = false;
        }
        else if (lineParts.First() == "Binaries")
        {
            obj = false;
            constraints = false;
        }
        else if (constraints)
        {
            var constraintBuilder = model.CreateConstraintBuilder();
            for (int i = lineParts[0].Contains("auto_c") ? 1 : 0; i < lineParts.Length - 1; i += 2)
            {
                if (lineParts[i] == "=")
                {
                    model.AddConstraint(constraintBuilder.Build(new IntegralInterval(lineParts.Last() == "1" ? 1 : 0,
                        lineParts.Last() == "1" ? 1 : 0)));
                    break;
                }


                if (!variableset.ContainsKey(lineParts[i + 1]))
                {
                    var v = model.NewVariable<IntegerVariable<IIntegerScalar>>(
                        new IntegralInterval(new IntegerScalar(0), new IntegerScalar(1)), lineParts[i + 1]);
                    variableset.Add(lineParts[i + 1], v);
                }

                constraintBuilder.AddTermToSum(new RealScalar(
                    double.Parse(lineParts[i])), variableset[lineParts[i + 1]]);
            }
        }
        else if (obj)
        {
            for (int i = lineParts[0] == "Obj:" ? 1 : 0; i < lineParts.Length - 2; i += 2)
            {
                if (!variableset.ContainsKey(lineParts[i + 1]))
                {
                    if (lineParts[i + 1] == "Constant")
                    {
                        var v = model.NewVariable<IntegerVariable<IIntegerScalar>>(
                            new IntegralInterval(new IntegerScalar(1), new IntegerScalar(1)), lineParts[i + 1]);
                        variableset.Add(lineParts[i + 1], v);
                    }
                    else
                    {
                        var v = model.NewVariable<IntegerVariable<IIntegerScalar>>(
                            new IntegralInterval(new IntegerScalar(0), new IntegerScalar(1)), lineParts[i + 1]);
                        variableset.Add(lineParts[i + 1], v);
                    }
                }

                objBuilder.AddTermToSum(new RealScalar(
                    double.Parse(lineParts[i])), variableset[lineParts[i + 1]]);
            }
        }
    }


    model.SetObjective(objBuilder.Build(false));

    var r = new IlpCbcSolver().Solve(model.SetObjective(objBuilder.Build(false)),
        new SolverParameter(EnableSolverOutput.True,
            RelativeGap.EMinus7, new TimeLimitInMilliseconds(1000 * 60 * 60 * 3), new NumberOfThreads(1)));

    Console.WriteLine(r);

    return;
}

class SolutionCallback : ICpSolutionCallback
{
    public void OnSolutionCallback(
        SolutionValues<IIntegerVariable<IIntegerScalar>, IntegerScalar, IIntegerScalar> solutionValues)
    {
        Console.WriteLine($"New solution found:");
        Console.WriteLine(solutionValues);
    }
}