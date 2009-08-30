
using System;

namespace Abc.CommonProjects.Security.Tests
{
   class Program
   {
      static void Main(string[] args)
      {
         SecurityTests unit = new SecurityTests();

         unit.Setup();
         unit.Hashes();
         unit.SaltedHashes();
         unit.HashFile();
         unit.Symmetric();
         unit.SymmetricFile();
         unit.Asymmetric();
      }
   }
}
