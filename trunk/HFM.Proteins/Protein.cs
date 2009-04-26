/*
 * HFM.NET - Protein Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;

namespace HFM.Proteins
{
    public class Protein
    {
        private int _ProjectNumber = 0;
        public int ProjectNumber
        {
            get { return _ProjectNumber; }
            set
            {
                if (value < 0)
                {
                    _ProjectNumber = 0;
                    throw new ArgumentOutOfRangeException("ProjectNumber", "Project Number must be greater than or equal to 0");
                }
                else
                {
                    _ProjectNumber = value;
                }
            }
        }
	
        private String _ServerIP;
        public String ServerIP
        {
            get { return _ServerIP; }
            set { _ServerIP = value; }
        }
	
        private String _WorkUnitName;
        public String WorkUnitName
        {
            get { return _WorkUnitName; }
            set { _WorkUnitName = value; }
        }

        private int _NumAtoms;
        public int NumAtoms
        {
            get { return _NumAtoms; }
            set
            {
                if (value < 0)
                {
                    _NumAtoms = 0;
                    throw new ArgumentOutOfRangeException("NumAtoms", "Number of Atoms must be greater than or equal to 0");
                }
                else
                {
                    _NumAtoms = value;
                }
            }
        }

        private int _PreferredDays;
        public int PreferredDays
        {
            get { return _PreferredDays; }
            set
            {
                if (value < 0)
                {
                    _PreferredDays = 0;
                    throw new ArgumentOutOfRangeException("PreferredDays", "Preferred Days must be greater than 0, or 0 for Timeless units");
                }
                else
                {
                    _PreferredDays = value;
                }
            }
        }

        private int _MaxDays;
        public int MaxDays
        {
            get { return _MaxDays; }
            set
            {
                if (value < 0)
                {
                    _MaxDays = 0;
                    throw new ArgumentOutOfRangeException("MaxDays", "Maximum Days must be greater than 0, or 0 for Timeless units");
                }
                else
                {
                    _MaxDays = value;
                }
            }
        }

        private int _Credit;
        public int Credit
        {
            get { return _Credit; }
            set
            {
                if (value < 0)
                {
                    _Credit = 0;
                    throw new ArgumentOutOfRangeException("Credit", "Credit must be greater than or equal to 0");
                }
                else
                {
                    _Credit = value;
                }
            }
        }

        private int _Frames;
        public int Frames
        {
            get { return _Frames; }
            set
            {
                if (value < 0)
                {
                    _Frames = 0;
                    throw new ArgumentOutOfRangeException("Frames", "Number of frames must be greater than or equal to 0");
                }
                else
                {
                    _Frames = value;
                }
            }
        }

        private String _Core;
        public String Core
        {
            get { return _Core; }
            set { _Core = value; }
        }

        private String _Description;
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private String _Contact;
        public String Contact
        {
            get { return _Contact; }
            set { _Contact = value; }
        }
    }
}
