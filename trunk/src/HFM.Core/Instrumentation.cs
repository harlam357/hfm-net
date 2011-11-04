
using System;
using System.Diagnostics;

namespace HFM.Core
{
   public static class Instrumentation
   {
      /// <summary>
      /// The function name of the caller function (1 stack level up)
      /// </summary>
      /// <returns>Class.Function name</returns>
      public static string FunctionName
      {
         get
         {
            var sf = new StackFrame(1, true);
            return String.Format("{0}.{1}", sf.GetMethod().DeclaringType, sf.GetMethod().Name);
         }
      }

      /// <summary>
      /// The function name of the parent function (2 stack levels up)
      /// </summary>
      /// <returns>Class.Function name</returns>
      public static string ParentFunctionName
      {
         get
         {
            var sf = new StackFrame(2, true);
            return String.Format("{0}.{1}", sf.GetMethod().DeclaringType, sf.GetMethod().Name);
         }
      }

      /// <summary>
      /// The function name of the grandparent function (3 stack levels up)
      /// </summary>
      /// <returns>Class.Function name</returns>
      public static string GParentFunctionName
      {
         get
         {
            var sf = new StackFrame(3, true);
            return String.Format("{0}.{1}", sf.GetMethod().DeclaringType, sf.GetMethod().Name);
         }
      }

      /// <summary>
      /// The filename in which the function can be found
      /// </summary>
      /// <returns></returns>
      public static string FileName
      {
         get
         {
            var sf = new StackFrame(1, true);
            return System.IO.Path.GetFileName(sf.GetFileName());
         }
      }

      /// <summary>
      /// Returns the execution time of the function
      /// </summary>
      /// <param name="start">The start time as previously returned by ExecStart</param>
      /// <returns>String formatted as "#,##0 ms"</returns>
      public static string GetExecTime(DateTime start)
      {
         TimeSpan t = DateTime.Now.Subtract(start);
         return String.Format("{0:#,##0} ms", t.TotalMilliseconds);
      }

      /// <summary>
      /// Simple wrapper around DateTime to return current time (usable for Start Time of a function or operation)
      /// </summary>
      public static DateTime ExecStart
      {
         get { return DateTime.Now; }
      }
   }
}
