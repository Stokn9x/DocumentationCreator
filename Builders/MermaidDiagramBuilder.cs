using System.Text;
using System.Text.RegularExpressions;
using DocumentationCreator.Models;

namespace DocumentationCreator.Builders
{
    public static class MermaidDiagramBuilder
    {
        public static string GenerateClassDiagram(IEnumerable<CodeFile> codeFiles)
        {
            var sb = new StringBuilder();

            sb.AppendLine("```mermaid");
            sb.AppendLine("classDiagram");
            sb.AppendLine("direction TB");

            var classNames = new Dictionary<CodeFile, string>();

            foreach (var file in codeFiles)
            {
                var className = GetClassName(file.Content);
                if (string.IsNullOrWhiteSpace(className)) continue;

                classNames[file] = className;

                sb.AppendLine($"class {className} {{");

                var methods = GetPublicMethods(file.Content);
                if (methods.Any())
                {
                    foreach (var method in methods)
                    {
                        sb.AppendLine($"  +{method}()");
                    }
                }
                else
                {
                    sb.AppendLine("  +<tom>()");
                }

                sb.AppendLine("}");
            }

            foreach (var file in codeFiles)
            {
                if (!classNames.ContainsKey(file)) continue;

                var fromClass = classNames[file];
                var dependencies = GetConstructorDependencies(file.Content);

                foreach (var dep in dependencies)
                {
                    var toClass = classNames.Values.FirstOrDefault(c => c == dep);
                    if (!string.IsNullOrWhiteSpace(toClass))
                    {
                        sb.AppendLine($"{fromClass} --> {toClass} : bruger");
                    }
                }
            }

            sb.AppendLine("```");
            return sb.ToString();
        }

        private static string GetClassName(string content)
        {
            var match = Regex.Match(content, @"class\s+(\w+)");
            return match.Success ? match.Groups[1].Value : "";
        }

        private static List<string> GetPublicMethods(string content)
        {
            var matches = Regex.Matches(content, @"public\s+(?:async\s+)?(?:\w+<[^>]+>|\w+)\s+(\w+)\s*\(");
            return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
        }

        private static List<string> GetConstructorDependencies(string content)
        {
            var constructorMatches = Regex.Matches(content, @"public\s+\w+\s*\(([^\)]*)\)");
            var dependencies = new List<string>();

            foreach (Match match in constructorMatches)
            {
                var parameters = match.Groups[1].Value;
                var types = Regex.Matches(parameters, @"\b(\w+)\s+\w+\b");
                foreach (Match typeMatch in types)
                {
                    dependencies.Add(typeMatch.Groups[1].Value);
                }
            }

            return dependencies.Distinct().ToList();
        }

        public static string GenerateErDiagramFromDbContext(string dbContextContent, Dictionary<string, string> entityContents)
        {
            var sb = new StringBuilder();

            var entityMatches = Regex.Matches(dbContextContent, @"DbSet<(\w+)>");
            var entities = entityMatches.Select(m => m.Groups[1].Value).Distinct().ToList();

            foreach (var entity in entities)
            {
                sb.AppendLine($"{entity} {{");

                if (entityContents.TryGetValue(entity, out var content))
                {
                    var propertyMatches = Regex.Matches(content, @"public\s+(\w+(?:<.*?>)?)\s+(\w+)\s*{\s*get;\s*set;\s*}");

                    foreach (Match match in propertyMatches)
                    {
                        var type = match.Groups[1].Value;
                        var name = match.Groups[2].Value;

                        if (Regex.IsMatch(type, @"^(ICollection|List|HashSet)<.*>$")) continue;

                        sb.AppendLine($"    {type} {name}");
                    }
                }
                else
                {
                    sb.AppendLine("    int id");
                }

                sb.AppendLine("}");
            }

            foreach (var entity in entities)
            {
                if (!entityContents.TryGetValue(entity, out var content)) continue;

                var relations = Regex.Matches(content, @"ICollection<(\w+)>");

                foreach (Match relation in relations)
                {
                    var target = relation.Groups[1].Value;
                    if (entities.Contains(target))
                    {
                        sb.AppendLine($"{entity} ||--o{{ {target} : has");
                    }
                }
            }

            return sb.ToString();
        }

    }
}
