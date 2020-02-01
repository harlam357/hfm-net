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

namespace HFM.Core
{
    public interface IFahClientFactory
    {
        FahClient Create();

        void Release(FahClient fahClient);
    }

    internal class BasicFahClientFactory : IFahClientFactory
    {
        public FahClient Create()
        {
            return new FahClient();
        }

        public void Release(FahClient fahClient)
        {
            
        }
    }

    public class ClientFactory
    {
        public IFahClientFactory FahClientFactory { get; set; } = new BasicFahClientFactory();

        public ICollection<IClient> CreateCollection(IEnumerable<ClientSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            return settings.Select(Create).Where(client => client != null).ToList();
        }

        public IClient Create(ClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            // special consideration for obsolete ClientType values that may appear in hfmx configuration files
            if (!settings.IsFahClient()) return null;

            if (!ClientSettings.ValidateName(settings.Name)) throw new ArgumentException($"Client name {settings.Name} is not valid.", nameof(settings));
            if (String.IsNullOrWhiteSpace(settings.Server)) throw new ArgumentException("Client server (host) name is empty.", nameof(settings));
            if (!Validate.ServerPort(settings.Port)) throw new ArgumentException($"Client server (host) port {settings.Port} is not valid.", nameof(settings));

            IClient client = FahClientFactory?.Create();
            if (client != null)
            {
                client.Settings = settings;
            }
            return client;
        }

        public void Release(IClient client)
        {
            if (client is FahClient fahClient)
            {
                FahClientFactory?.Release(fahClient);
            }
        }
    }
}
