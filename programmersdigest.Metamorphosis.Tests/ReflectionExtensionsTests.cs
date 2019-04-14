using programmersdigest.Metamorphosis.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace programmersdigest.Metamorphosis.Tests
{
    [TestClass]
    public class ReflectionExtensionsTests
    {
        #region Identical Method Tests

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithoutParameters_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.SimpleTestA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithReturnType_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ReturnTypeTestA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithGenericReturnType_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithGenericReturnTypeAndConstraint_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestWithConstraintA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithParameters_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithGenericParameters_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_IdenticalMethodWithGenericParametersAndConstraint_ShouldBeCompatible()
        {
            var method = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintA));

            var result = method.IsMethodDefinitionCompatible(method);
            Assert.IsTrue(result);
        }

        #endregion

        #region Compatible Method Tests

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithoutParameters_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.SimpleTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.SimpleTestB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithReturnType_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ReturnTypeTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ReturnTypeTestB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithGenericReturnType_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithGenericReturnTypeAndConstraint_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestWithConstraintA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestWithConstraintB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithParameters_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithGenericParameters_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_CompatibleMethodsWithGenericParametersAndConstraint_ShouldBeCompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintB));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsTrue(result);
        }

        #endregion

        #region Incompatible Method Tests

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodWithReturnType_MethodWithoutReturnType_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.SimpleTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ReturnTypeTestA));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodWithGenericReturnTypeAndConstraint_MethodWithGenericReturnType_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestWithConstraintA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericReturnTypeTestA));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodsWithIncompatibleParameterCount_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestC));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodsWithIncompatibleParameterTypes_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.ParametersTestD));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodWithGenericParameters_MethodWithGenericParametersAndConstraints_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintA));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodWithTwoGenericParameters_MethodWithThreeGenericParameters_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestC));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodsWithIncompatibleGenericParameterConstraints_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintC));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMethodDefinitionCompatible_MethodsWithIncompatibleGenericParameterConstraints2_ShouldBeIncompatible()
        {
            var methodA = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintA));
            var methodB = typeof(ReflectionExtensionsTestsStub).GetMethod(nameof(ReflectionExtensionsTestsStub.GenericParametersTestWithConstraintD));

            var result = methodA.IsMethodDefinitionCompatible(methodB);
            Assert.IsFalse(result);
        }

        #endregion
    }
}
