/*
 * HFM.NET - Markup Generator Interface
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
 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HFM.Framework
{
   public interface IMarkupGenerator
   {
      /// <summary>
      /// In Progress Flag for Callers
      /// </summary>
      bool GenerationInProgress { get; }
      
      /// <summary>
      /// Contains XML File Paths from most recent XML Generation
      /// </summary>
      ReadOnlyCollection<string> XmlFilePaths { get; }

      /// <summary>
      /// Contains HTML File Paths from most recent HTML Generation
      /// </summary>
      ReadOnlyCollection<string> HtmlFilePaths { get; }

      /// <summary>
      /// Generate HTML Files from the given Client Instance Collection.
      /// </summary>
      /// <param name="instances">Client Instance Collection.</param>
      /// <exception cref="ArgumentNullException">Throws if instances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      void GenerateHtml(ICollection<IClientInstance> instances);

      /// <summary>
      /// Generate XML Files from the given Client Instance Collection.
      /// </summary>
      /// <param name="instances">Client Instance Collection.</param>
      /// <exception cref="ArgumentNullException">Throws if instances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      void GenerateXml(ICollection<IClientInstance> instances);
   }
}