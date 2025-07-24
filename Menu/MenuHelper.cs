using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentationCreator.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace DocumentationCreator.Menu
{
    public static class MenuHelper
    {
        public static List<string> GetExclusionFolders()
        {
            var exclusions = new List<string>();
            bool exclusionState = true;

            while (exclusionState)
            {
                var folder = AnsiConsole.Ask<string>("Indtast mappenavn du vil ekskludere (eller tom for at afslutte):");
                if (string.IsNullOrWhiteSpace(folder)) break;

                exclusions.Add(folder);

                exclusionState = Beautifier.CoolMenu("Vil du ekskludere flere mapper?", "Ja", "Nej") == "Ja";
            }

            return exclusions;
        }
    }
}
