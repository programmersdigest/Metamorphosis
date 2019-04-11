using System;

namespace Metamorphosis.Tests.Stubs
{
    internal sealed class ReflectionExtensionsTestsStub
    {
        public void SimpleTestA()
        {

        }

        public void SimpleTestB()
        {

        }

        public string ReturnTypeTestA()
        {
            throw new NotImplementedException();
        }

        public string ReturnTypeTestB()
        {
            throw new NotImplementedException();
        }

        public T GenericReturnTypeTestA<T>()
        {
            throw new NotImplementedException();
        }

        public T GenericReturnTypeTestB<T>()
        {
            throw new NotImplementedException();
        }

        public T GenericReturnTypeTestWithConstraintA<T>() where T : Exception
        {
            throw new NotImplementedException();
        }

        public T GenericReturnTypeTestWithConstraintB<T>() where T : Exception
        {
            throw new NotImplementedException();
        }

        public void ParametersTestA(int param1, string param2)
        {
        }

        public void ParametersTestB(int param1, string param2)
        {
        }

        public void ParametersTestC(int param1)
        {
        }

        public void ParametersTestD(int param1, object param2)
        {
        }

        public void GenericParametersTestA<T, U>(T param1, U param2)
        {
        }

        public void GenericParametersTestB<T, U>(T param1, U param2)
        {
        }

        public void GenericParametersTestC<T, U, V>(T param1, U param2)
        {
        }

        public void GenericParametersTestWithConstraintA<T, U>(T param1, U param2) where T : Exception
        {
        }

        public void GenericParametersTestWithConstraintB<T, U>(T param1, U param2) where T : Exception
        {
        }

        public void GenericParametersTestWithConstraintC<T, U>(T param1, U param2) where T : Exception where U : Exception
        {
        }

        public void GenericParametersTestWithConstraintD<T, U, V>(T param1, U param2) where T : V
        {
        }
    }
}
