using System;
using System.Collections.Generic;
using System.Text;

using Castle.Windsor;

namespace HFM.Framework
{
   public static class InstanceProvider
   {
      private static IWindsorContainer _container;
   
      public static void SetContainer(IWindsorContainer container)
      {
         _container = container;
      }

      public static T GetInstance<T>()
      {
         return (T)_container[typeof(T)];
      }
   }
}
