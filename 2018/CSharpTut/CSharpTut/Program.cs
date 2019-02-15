using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTut
{
    class Program
    {
        
        static void Main(string[] args)
        {


            /*Console.WriteLine("Hello sunshine");

            for (int i = 0; i < args.Length; i++)  //i++ = i+1
            {
                Console.WriteLine(args[i]);
            }

            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("Arg {0} : {1}", i, args[i] );
            }


            
            string[] myArgs = Environment.GetCommandLineArgs(); // gives us the 
            // location of the exe file C:\Users\Dell-PC\source\repos\CSharpTut\CSharpTut\bin\Debug\CSharpTut.exe

            Console.WriteLine(string.Join(", ", myArgs));

            SayHello();

            -----------------Data types 


*/
            bool canIvote = true;
            if (canIvote = true)
                Console.WriteLine("canIvote its true");
            Console.WriteLine();

            Console.WriteLine("Biggest integer: {0}", int.MaxValue);
            Console.WriteLine("Smallest integer: {0}", int.MinValue);
            Console.WriteLine();

            Console.WriteLine("Biggest Long: {0}", long.MaxValue);
            Console.WriteLine("Smallest Long: {0}", long.MinValue);
            Console.WriteLine();

            decimal decPiVal = 3.1415926535897932384626433832M;
            decimal decBigNum = 3.00000000000000000000000000011M;
            Console.WriteLine("Dec = PI + bigNum = {0}", decPiVal+decBigNum);

            Console.WriteLine("Biggest decimal: {0}", decimal.MaxValue);
            Console.WriteLine("Smallest decimal: {0}", decimal.MinValue);
            Console.WriteLine();

            double  dblPiVal = 3.14159265358979;
            double dbligNum = 3.00000000000002;
            Console.WriteLine("DBL = PI + bigNum = {0}", dblPiVal + dbligNum);

            Console.WriteLine("Biggest double: {0}", double.MaxValue);
            Console.WriteLine("Smallest double: {0}", double.MinValue);
            Console.WriteLine();






            Console.ReadLine();

        }

        private static void SayHello()
        {
            string name = "";
            Console.WriteLine("What is your name:" , name);
            name = Console.ReadLine();
            Console.WriteLine("Hello {0}", name);
        }
    }
}
