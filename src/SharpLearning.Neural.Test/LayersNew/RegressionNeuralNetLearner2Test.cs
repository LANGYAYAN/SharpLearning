﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpLearning.Containers.Matrices;
using SharpLearning.Metrics.Regression;
using SharpLearning.Neural.LayersNew;
using SharpLearning.Neural.Loss;

namespace SharpLearning.Neural.Test.LayersNew
{
    [TestClass]
    public class RegressionNeuralNetLearner2Test
    {
        [TestMethod]
        public void RegressionNeuralNetLearner2_Learn()
        {
            var numberOfObservations = 500;
            var numberOfFeatures = 5 * 5;

            var random = new Random(32);
            F64Matrix observations;
            double[] targets;
            CreateData(numberOfObservations, numberOfFeatures, random, out observations, out targets);

            var net = new NeuralNet2();

            net.Add(new InputLayer(5, 5, 1));

            net.Add(new Conv2DLayer(5, 5, 5));
            net.Add(new BatchNormalizationLayer());
            net.Add(new ActivationLayer(Neural.Activations.Activation.Relu));

            net.Add(new DenseLayer(10));
            net.Add(new BatchNormalizationLayer());
            net.Add(new ActivationLayer(Neural.Activations.Activation.Relu));
            net.Add(new DropoutLayer(0.5));

            // output layer 
            net.Add(new DenseLayer(1));
            net.Add(new ActivationLayer(Neural.Activations.Activation.MeanSquareError));

            var sut = new RegressionNeuralNetLearner2(net, new SquareLoss());
            var model = sut.Learn(observations, targets);

            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var actual = evaluator.Error(targets, predictions);

            Assert.AreEqual(0.34822374919397991, actual, 0.00001);
        }

        void CreateData(int numberOfObservations, int numberOfFeatures, Random random, out F64Matrix observations, out double[] targets)
        {
            observations = new F64Matrix(numberOfObservations, numberOfFeatures);
            observations.Map(() => random.NextDouble());
            targets = Enumerable.Range(0, numberOfObservations).Select(i => random.NextDouble()).ToArray();
        }
    }
}
