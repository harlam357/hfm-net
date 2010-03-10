/*
 * HFM.NET - Client Instance Factory
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */
 
using HFM.Framework;

namespace HFM.Instances
{
   public class ClientInstanceFactory
   {
      private readonly IPreferenceSet _Prefs;
      
      private readonly IProteinCollection _ProteinCollection;
      
      private readonly IProteinBenchmarkContainer _BenchmarkContainer;
      
      public ClientInstanceFactory(IPreferenceSet Prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer)
      {
         _Prefs = Prefs;
         _ProteinCollection = proteinCollection;
         _BenchmarkContainer = benchmarkContainer;
      }

      public ClientInstance Create()
      {
         return new ClientInstance(_Prefs, _ProteinCollection, _BenchmarkContainer);
      }
   }
}
