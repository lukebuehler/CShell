using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Caliburn.Micro;

namespace CShell.Modules.Help.ViewModels
{
    [Export]
    public class AboutViewModel : Screen
    {
        public AboutViewModel()
        {
            DisplayName = "About CShell";
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string AboutText
        {
            get
            {
                var txt = "";
                txt += "CShell, A Simple C# Scripting IDE" + Environment.NewLine;
                txt+="Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler" + Environment.NewLine;
                txt+="" + Environment.NewLine;
                txt += "This program is free software: you can redistribute it and/or modify ";
                txt += "it under the terms of the GNU General Public License as published by ";
                txt += "the Free Software Foundation, either version 3 of the License, or ";
                txt+="(at your option) any later version." + Environment.NewLine;
                txt+="" + Environment.NewLine;
                txt += "This program is distributed in the hope that it will be useful, ";
                txt += "but WITHOUT ANY WARRANTY; without even the implied warranty of ";
                txt += "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the ";
                txt+="GNU General Public License for more details." + Environment.NewLine;
                txt += "" + Environment.NewLine;
                txt += "You should have received a copy of the GNU General Public License ";
                txt += "along with this program.  If not, see: ";
                txt+="http://www.gnu.org/licenses/." + Environment.NewLine;
                return txt;
            }
        }
    }
}
