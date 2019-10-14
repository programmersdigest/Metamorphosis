using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Metamorphosis.Exceptions;
using programmersdigest.Metamorphosis.Modelling;
using programmersdigest.Metamorphosis.Tests.Stubs;

namespace programmersdigest.Metamorphosis.Tests.Modelling
{
    [TestClass]
    public class ProxyGeneratorTests
    {
        [TestMethod]
        public void GenerateProxyTypes_ValidComponentDefinitions_ProxyTypesAreCreated()
        {
            var generator = new ProxyGenerator();
            var componentDefinitions = new List<ComponentDefinition>();

            var signalStub = ProxyGeneratorTestsSimpleSignalStub.GetComponentDefinition();
            var triggerStub = ProxyGeneratorTestsSimpleTriggerStub.GetComponentDefinition();

            componentDefinitions.Add(signalStub);
            componentDefinitions.Add(triggerStub);

            Assert.IsNull(signalStub.ProxyType);
            Assert.IsNull(triggerStub.ProxyType);

            generator.GenerateProxyTypes(componentDefinitions);

            Assert.IsNotNull(signalStub.ProxyType);
            Assert.IsNotNull(triggerStub.ProxyType);
        }

        [TestMethod]
        public void GenerateProxyTypes_ComponentDefinitionWithLoop_ThrowsException()
        {
            var generator = new ProxyGenerator();
            var componentDefinitions = new List<ComponentDefinition>();

            var stubA = ProxyGeneratorTestsLoopStubA.GetComponentDefinition();
            var stubB = ProxyGeneratorTestsLoopStubB.GetComponentDefinition();

            componentDefinitions.Add(stubA);
            componentDefinitions.Add(stubB);

            Assert.ThrowsException<ComponentDefinitionLoopException>(() => generator.GenerateProxyTypes(componentDefinitions));
        }

        [TestMethod]
        public void GenerateProxyTypes_ComponentDefinitionWithMissingTrigger_ThrowsException()
        {
            var generator = new ProxyGenerator();
            var componentDefinitions = new List<ComponentDefinition>();

            var stubA = ProxyGeneratorTestsMissingTriggerStubA.GetComponentDefinition();
            var stubB = ProxyGeneratorTestsMissingTriggerStubB.GetComponentDefinition();

            componentDefinitions.Add(stubA);
            componentDefinitions.Add(stubB);

            Assert.ThrowsException<MissingTriggerException>(() => generator.GenerateProxyTypes(componentDefinitions));
        }

        [TestMethod]
        public void GenerateProxyTypes_ComponentDefinitionWithMissingConnection_ThrowsException()
        {
            var generator = new ProxyGenerator();
            var componentDefinitions = new List<ComponentDefinition>();

            var stubA = ProxyGeneratorTestsMissingConnectionStubA.GetComponentDefinition();
            var stubB = ProxyGeneratorTestsMissingConnectionStubB.GetComponentDefinition();

            componentDefinitions.Add(stubA);
            componentDefinitions.Add(stubB);

            Assert.ThrowsException<MissingConnectionException>(() => generator.GenerateProxyTypes(componentDefinitions));
        }
    }
}
