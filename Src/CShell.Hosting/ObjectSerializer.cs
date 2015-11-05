namespace CShell.Hosting
{
    using System;

    using ScriptCs.Contracts;

    public class ObjectSerializer : IObjectSerializer
    {
        public string Serialize(object value)
        {
            throw new NotImplementedException();
        }
    }

    public class MockConsole : IConsole
    {
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public ConsoleColor ForegroundColor
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void ResetColor()
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine()
        {
            throw new NotImplementedException();
        }
    }
}
