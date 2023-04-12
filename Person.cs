using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloWorld {
    public class Person {
        public string Name { get; private set; }
        public Person(string name) {
            Name = name;
            Speak();
        }
        public string Speak() {
            // this is a coment
            return string.Format("Hello! My name is {0}", Name);
        }

        private string Speak2() {
            return Speak();
        }

        public static void ClassName() {
            Console.WriteLine("Class name is person");
        }
    }
}