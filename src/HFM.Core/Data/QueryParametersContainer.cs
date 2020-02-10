/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Linq;

using HFM.Core.DataTypes;
using HFM.Preferences;

namespace HFM.Core.Data
{
    public interface IQueryParametersContainer
    {
        ICollection<QueryParameters> Get();

        void Update(IEnumerable<QueryParameters> collection);
    }

    public sealed class QueryParametersContainer : DataContainer<List<QueryParameters>>, IQueryParametersContainer
    {
        public const string DefaultFileName = "WuHistoryQuery.dat";

        public QueryParametersContainer()
           : this(null)
        {

        }

        public QueryParametersContainer(IPreferenceSet prefs)
        {
            var path = prefs?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }

        #region Properties

        public override Serializers.IFileSerializer<List<QueryParameters>> DefaultSerializer
        {
            get { return new Serializers.ProtoBufFileSerializer<List<QueryParameters>>(); }
        }

        #endregion

        public ICollection<QueryParameters> Get()
        {
            return Data.ToList();
        }

        public void Update(IEnumerable<QueryParameters> collection)
        {
            Data.Clear();
            Data.AddRange(collection);
            Write();
        }
    }
}
