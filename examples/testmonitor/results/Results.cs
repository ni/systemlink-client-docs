using System;
using System.Collections.Generic;
using NationalInstruments.SystemLink.Clients.TestMonitor;

namespace NationalInstruments.SystemLink.Clients.Examples.TestMonitor
{
    /// <summary>
    /// Example for the SystemLink Test Monitor API that creates a test result
    /// and associated steps simulating a current and voltage sweep.
    /// </summary>
    class Results
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Create the TestDataManager for communicating with the server.
             */
            var testDataManager = new TestDataManager(configuration);

            // Intialize the random number generator
            var random = new Random();

            // Set test limits
            var lowLimit = 0;
            var highLimit = 70;

            // Initialize a ResultData object
            var resultData = new ResultData()
            {
                Operator = "John Smith",
                ProgramName = "Power Test",
                Status = new Status(StatusType.Running),
                SerialNumber = Guid.NewGuid().ToString(),
                PartNumber = "NI-ABC-123-PWR"
            };
            // Create the test result on the SystemLink server
            var testResult = testDataManager.CreateResult(resultData);
            // Automatically sync the result's runtime with its test steps
            testResult.AutoUpdateTotalTime = true;

            /*
            * Simulate a sweep across a range of electrical current and voltage.
            * For each value, calculate the electrical power (P=IV).
            */
            for (var current = 0; current < 10; current++)
            {
                // Generate a parent step to represent a sweep of voltages at a given current
                var currentStepData = GenerateStepData($"Voltage Sweep", "SequenceCall", null, null, null, new Status(StatusType.Running));
                // Create the step on the SystemLink server
                var currentStep = testResult.CreateStep(currentStepData);

                for (var voltage = 0; voltage < 10; voltage++)
                {
                    // Simulate obtaining a power measurement
                    var (power, inputs, outputs) = MeasurePower(current, voltage);

                    // Testing the power measurement
                    var status = (power < lowLimit || power > highLimit) ? new Status(StatusType.Failed) : new Status(StatusType.Passed);
                    var testParameters = BuildPowerMeasurementParams(power, lowLimit, highLimit, status);

                    // Generate a child step to represent the power output measurement
                    var voltageStepData = GenerateStepData($"Measure Power Output", "NumericLimit", inputs, outputs, test_parameters, status);
                    // Create the step on the SystemLink server
                    var voltageStep = currentStep.CreateStep(voltageStepData);

                    // If a test in the sweep fails, the entire sweep failed.  Mark the parent step accordingly
                    if (status.StatusType.Equals(StatusType.Failed))
                    {
                        currentStepData.Status = new Status(StatusType.Failed);
                        // Update the parent test step's status on the SystemLink server
                        currentStep.Update(currentStepData);
                    }
                }

                // If none of the child steps failed, mark the step as passed
                if (currentStepData.Status.StatusType.Equals(StatusType.Running))
                {
                    currentStepData.Status = new Status(StatusType.Passed);
                    // Update the test step's status on the SystemLink server
                    currentStep.Update(currentStepData);
                }
            }

            // Update the top-level test result's status based on the most severe child step's status
            testResult = testResult.DetermineStatusFromSteps();
        }

        /// <summary>
        /// Simulates taking an electrical power measurement.
        /// This introduces some random current and voltage loss.
        /// </summary>
        /// <param name="current">The electrical current value.</param>
        /// <param name="voltage">The electrical voltage value.</param>
        /// <returns>The a tuple containing the electrical power measurements and the input and output lists.</returns>
        private static (double power, List<NamedValue> inputs, List<NamedValue> outputs) MeasurePower(int current, int voltage = 0)
        {
            var random = new Random();
            var currentLoss = 1 - random.NextDouble() * 0.25;
            var voltageLoss = 1 - random.NextDouble() * 0.25;
            var power = current * currentLoss * voltage * voltageLoss;

            // Record electrical current and voltage as inputs
            var inputs = new List<NamedValue>()
            {
                new NamedValue("current", current),
                new NamedValue("voltage", voltage)
            };

            // Record electrical power as an output
            var outputs = new List<NamedValue>()
            {
                new NamedValue("power", power)
            };

            return (power, inputs, outputs);
        }

        /// <summary>
        /// Builds a Test Monitor measurement parameter object for the power test.
        /// </summary>
        /// <param name="power">The electrical power measurement.</param>
        /// <param name="lowLimit">The value of the low limit for the test.</param>
        /// <param name="highLimit">The value of the high limit for the test.</param>
        /// <param name="status">The measurement's pass/fail status.</param>
        /// <returns>A list of test measurement parameters.</returns>
        private static List<Dictionary<string, string>> BuildPowerMeasurementParams(double power, double lowLimit, double highLimit, Status status)
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("name", $"Power Test");
            parameter.Add("status", status.StatusType.ToString());
            parameter.Add("measurement", $"{power}");
            parameter.Add("units", "Watts");
            parameter.Add("nominalValue", null);
            parameter.Add("lowLimit", $"{lowLimit}");
            parameter.Add("highLimit", $"{highLimit}");
            parameter.Add("comparisonType", "GELE");

            var parameters = new List<Dictionary<String, String>>() { parameter };
            return parameters;
        }

        /// <summary>
        /// Creates a <see cref="StepData"/> object and
        /// populates it to match the TestStand data model.
        /// </summary>
        /// <param name="name">The test step's name.</param>
        /// <param name="stepType">The test step's type.</param>
        /// <param name="inputs">The test step's input values.</param>
        /// <param name="outputs">The test step's output values.</param>
        /// <param name="parameters">The measurement parameters.</param>
        /// <returns>The <see cref="StepData"/> used to create a test step.</returns>
        private static StepData GenerateStepData(string name, string stepType, List<NamedValue> inputs, List<NamedValue> outputs, List<Dictionary<String, String>> parameters, Status status)
        {
            var random = new Random();
            var stepStatus = status ?? new Status(StatusType.Running);

            var stepData = new StepData()
            {
                Name = name,
                Inputs = inputs,
                Outputs = outputs,
                StepType = stepType,
                Status = stepStatus,
                TotalTimeInSeconds = random.NextDouble() * 10,
                Parameters = parameters,
                DataModel = "TestStand",
            };

            return stepData;
        }
    }
}
