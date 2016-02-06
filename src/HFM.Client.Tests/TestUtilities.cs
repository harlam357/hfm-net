
using System;
using System.Text;

using Rhino.Mocks;

namespace HFM.Client.Tests
{
   internal static class TestUtilities
   {
      internal static IAsyncResult DoBeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state, string data)
      {
         return DoBeginRead(buffer, offset, size, callback, state, Encoding.ASCII.GetBytes(data));
      }

      internal static IAsyncResult DoBeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state, byte[] messageBytes)
      {
         for (int i = 0; i < messageBytes.Length; i++)
         {
            buffer[i] = messageBytes[i];
         }
         var ar = MockRepository.GenerateStub<IAsyncResult>();
         ar.Stub(x => x.AsyncState).Return(state);
         callback.BeginInvoke(ar, null, null);
         return ar;
      }
   }
}
