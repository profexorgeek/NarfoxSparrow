using System;
using System.Collections.Generic;
using System.Text;

namespace NarfoxSparrow.Services
{
    public class InputService
    {
        static InputService instance;

        public static InputService Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new InputService();
                }
                return instance;
            }
        }

        private InputService() { }

        public string GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            return input;
        }
    }
}
