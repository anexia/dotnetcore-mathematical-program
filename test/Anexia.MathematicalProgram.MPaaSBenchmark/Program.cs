using Anexia.MathematicalProgram.MPaaS.Client;
using Anexia.MathematicalProgram.MPaaS.Model;

var mpaasUri = new Uri(args[0]);

var random = new Random(42);
var maxNumberOfItems = args.Length > 1 ? int.Parse(args[1]) : 100;

var minWeight = 1;
var maxWeight = 1e3;

var capacity = maxNumberOfItems * maxWeight;

var itemSelectionVariables = Enumerable.Range(1, maxNumberOfItems).Select(variableIndex =>
    new Variable($"x{variableIndex}", 0, 1)).ToArray();

var capacityConstraint =
    new Constraint(
        itemSelectionVariables
            .Select(variable => new LinearExpression(minWeight + random.NextDouble() * maxWeight, variable.Name))
            .ToArray(), 0,
        capacity);

var model = new PostJobsRequest(itemSelectionVariables, [capacityConstraint],
    new Objective(itemSelectionVariables.Select(variable => new LinearExpression(1.0, variable.Name)).ToArray(), true));

var mpaasClientConfig = new MPaaSClientConfig(TimeSpan.FromMinutes(1));
var mpaasClient = new MPaaSClient(mpaasUri, mpaasClientConfig,
    new HttpClient { Timeout = TimeSpan.FromMinutes(30) });
var solution = await mpaasClient.SolveAsync(model);

Console.WriteLine(solution);