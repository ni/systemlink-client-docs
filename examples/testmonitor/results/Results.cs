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
        /// <summary>
        /// Creates a <see cref="StepData"/> object and
        /// populates it to match the TestStand data model.
        /// </summary>
        /// <param name="name">The test step's name.</param>
        /// <param name="stepType">The test step's type.</param>
        /// <param name="current">The value of the measured electrical current.</param>
        /// <param name="voltage">The value of the measured electrical voltage.</param>
        /// <param name="power">The value of calculated electrical power.</param>
        /// <param name="lowLimit">The value of the low test limit.</param>
        /// <param name="highLimit">The value of the high test limit.</param>
        /// <returns>The <see cref="StepData"/> used to create a test step.</returns>
        private static StepData GenerateStepData(string name, string stepType, double current, double voltage, double power, double lowLimit, double highLimit)
        {
            Random random = new Random();
            var status = new Status(StatusType.Running);
            var inputs = new List<NamedValue>();

            var outputs = new List<NamedValue>();

            var parameters = new List<Dictionary<string, string>>();
            if (stepType.Equals("NumericLimit"))
            {
                // Add parameters to match the TestStand data model
                var parameter = new Dictionary<string, string>();
                parameter.Add("name", "Power Test");
                parameter.Add("status", "status");
                parameter.Add("measurement", $"{power}");
                parameter.Add("units", null);
                parameter.Add("nominalValue", null);
                parameter.Add("lowLimit", $"{lowLimit}");
                parameter.Add("highLimit", $"{highLimit}");
                parameter.Add("comparisonType", "GELT");
                parameters.Add(parameter);

                // Record electrical current and voltage as inputs
                inputs = new List<NamedValue>()
                {
                    new NamedValue("current", current),
                    new NamedValue("voltage", voltage)
                };

                // Record electrical power as an output
                outputs = new List<NamedValue>()
                {
                    new NamedValue("power", power)
                };

                // Set the step's status based on if the electrical power is within the test limits
                status = (power < lowLimit || power > highLimit) ? new Status(StatusType.Failed) : new Status(StatusType.Passed);
            }

            var stepData = new StepData()
            {
                Inputs = inputs,
                Name = name,
                Outputs = outputs,
                StepType = stepType,
                Status = status,
                TotalTimeInSeconds = random.NextDouble() * 10,
                Parameters = parameters,
                DataModel = "TestStand",
            };

            return stepData;
        }

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
            Random random = new Random();

            // Set test limits
            var lowLimit = 0;
            var highLimit = 70;

            // Initialize a ResultData object
            var resultData = new ResultData()
            {
                Operator = "mvaterla",
                ProgramName = "Power Test",
                Status = new Status(StatusType.Running),
                SerialNumber = Guid.NewGuid().ToString(),
                PartNumber = "NI-ABC-123-PWR",
                FileIds = new List<string> { }
            };
            // Create the test result on the SystemLink server
            var testResult = testDataManager.CreateResult(resultData);
            testResult.AutoUpdateTotalTime = true;

            /*
            * Simulate a sweep across a range of electrical current and voltage.
            * For each value, calculate the electrical power (P=IV).
            * Simulate some random current and voltage loss after each test to
            * provide some randomness the the values.
            */
            var current = 0;
            var voltage = 0;
            var currentLoss = 1 - random.NextDouble();
            var voltageLoss = 1 - random.NextDouble();
            var power = current * currentLoss * voltage * voltageLoss;
            for (current = 0; current < 10; current++)
            {
                currentLoss = 1 - random.NextDouble() * 0.25;
                power = current * currentLoss * voltage * voltageLoss;
                // Generate the step data
                var currentStepData = GenerateStepData($"Current Sweep", "SequenceCall", current, voltage, power, lowLimit, highLimit);
                // Create the step on the SystemLink server
                var currentStep = testResult.CreateStep(currentStepData);

                for (voltage = 0; voltage < 10; voltage++)
                {
                    voltageLoss = 1 - random.NextDouble() * 0.25;
                    power = current * currentLoss * voltage * voltageLoss;
                    // Generate the step data
                    var voltageStepData = GenerateStepData($"Voltage Sweep", "NumericLimit", current, voltage, power, lowLimit, highLimit);
                    // Create the step on the SystemLink server
                    var voltageStep = currentStep.CreateStep(voltageStepData);

                    // Set the parent test step's status if the child test step failed
                    if (voltageStep.Data.Status.StatusType.Equals(StatusType.Failed))
                    {
                        currentStepData.Status = new Status(StatusType.Failed);
                        // Update the parent test step's status on the SystemLink server
                        currentStep.Update(currentStepData);
                    }
                }

                // If none of the child steps failed, mark the step as passed.
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
    }
}
