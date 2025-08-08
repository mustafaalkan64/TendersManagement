using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Offers.Helpers
{
    /// <summary>
    /// Helper class for project cleanup and refactoring operations
    /// </summary>
    public static class ProjectCleanupHelper
    {
        private static readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("ProjectCleanup");

        /// <summary>
        /// Common unused using statements that can be safely removed
        /// </summary>
        private static readonly HashSet<string> CommonUnusedUsings = new()
        {
            "using Microsoft.AspNetCore.Hosting;",
            "using System.ComponentModel.DataAnnotations;",
            "using System.Net.Http;",
            "using System.Text.Json;",
            "using System.IO;",
            "using System.Net;",
            "using System.Reflection;"
        };

        /// <summary>
        /// Patterns for code that can be simplified or cleaned up
        /// </summary>
        private static readonly Dictionary<string, string> SimplificationPatterns = new()
        {
            // Simplify string.IsNullOrEmpty checks
            { @"string\.IsNullOrEmpty\(([^)]+)\) == false", "!string.IsNullOrEmpty($1)" },
            { @"string\.IsNullOrEmpty\(([^)]+)\) != true", "!string.IsNullOrEmpty($1)" },
            
            // Simplify boolean comparisons
            { @"== true", "" },
            { @"== false", " == false" },
            
            // Remove redundant .ToString() calls
            { @"\.ToString\(\)\.ToString\(\)", ".ToString()" },
            
            // Simplify LINQ expressions
            { @"\.Where\(x => x != null\)\.ToList\(\)", ".Where(x => x != null).ToList()" }
        };

        /// <summary>
        /// Analyzes a C# file for potential cleanup opportunities
        /// </summary>
        public static CleanupAnalysisResult AnalyzeFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new CleanupAnalysisResult { FilePath = filePath, HasIssues = false };

            var content = File.ReadAllText(filePath);
            var result = new CleanupAnalysisResult { FilePath = filePath };

            // Check for unused using statements
            result.UnusedUsings = FindUnusedUsings(content);
            
            // Check for code simplification opportunities
            result.SimplificationOpportunities = FindSimplificationOpportunities(content);
            
            // Check for duplicate code blocks
            result.DuplicateCodeBlocks = FindDuplicateCodeBlocks(content);
            
            // Check for long methods (over 50 lines)
            result.LongMethods = FindLongMethods(content);
            
            // Check for magic numbers
            result.MagicNumbers = FindMagicNumbers(content);

            result.HasIssues = result.UnusedUsings.Any() || 
                              result.SimplificationOpportunities.Any() || 
                              result.DuplicateCodeBlocks.Any() || 
                              result.LongMethods.Any() || 
                              result.MagicNumbers.Any();

            return result;
        }

        private static List<string> FindUnusedUsings(string content)
        {
            var unusedUsings = new List<string>();
            var lines = content.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("using ") && trimmedLine.EndsWith(";"))
                {
                    var usingStatement = trimmedLine;
                    var namespaceName = usingStatement.Replace("using ", "").Replace(";", "").Trim();
                    
                    // Simple heuristic: if namespace is not referenced elsewhere, it might be unused
                    if (!content.Contains(namespaceName.Split('.').Last()) && 
                        CommonUnusedUsings.Contains(usingStatement))
                    {
                        unusedUsings.Add(usingStatement);
                    }
                }
            }
            
            return unusedUsings;
        }

        private static List<string> FindSimplificationOpportunities(string content)
        {
            var opportunities = new List<string>();
            
            foreach (var pattern in SimplificationPatterns)
            {
                var matches = Regex.Matches(content, pattern.Key);
                foreach (Match match in matches)
                {
                    opportunities.Add($"Line with '{match.Value}' can be simplified");
                }
            }
            
            return opportunities;
        }

        private static List<string> FindDuplicateCodeBlocks(string content)
        {
            // Simple duplicate detection - look for repeated method signatures
            var duplicates = new List<string>();
            var lines = content.Split('\n');
            var methodSignatures = new Dictionary<string, int>();
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Contains("public ") && trimmedLine.Contains("(") && trimmedLine.Contains(")"))
                {
                    var signature = Regex.Replace(trimmedLine, @"\s+", " ");
                    if (methodSignatures.ContainsKey(signature))
                    {
                        methodSignatures[signature]++;
                    }
                    else
                    {
                        methodSignatures[signature] = 1;
                    }
                }
            }
            
            foreach (var kvp in methodSignatures.Where(x => x.Value > 1))
            {
                duplicates.Add($"Potential duplicate method: {kvp.Key}");
            }
            
            return duplicates;
        }

        private static List<string> FindLongMethods(string content)
        {
            var longMethods = new List<string>();
            var lines = content.Split('\n');
            var currentMethod = "";
            var methodLineCount = 0;
            var inMethod = false;
            var braceCount = 0;
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                if (line.Contains("public ") && line.Contains("(") && line.Contains(")"))
                {
                    currentMethod = line;
                    methodLineCount = 0;
                    inMethod = true;
                    braceCount = 0;
                }
                
                if (inMethod)
                {
                    methodLineCount++;
                    braceCount += line.Count(c => c == '{') - line.Count(c => c == '}');
                    
                    if (braceCount == 0 && methodLineCount > 1)
                    {
                        if (methodLineCount > 50)
                        {
                            longMethods.Add($"Method '{currentMethod}' has {methodLineCount} lines");
                        }
                        inMethod = false;
                    }
                }
            }
            
            return longMethods;
        }

        private static List<string> FindMagicNumbers(string content)
        {
            var magicNumbers = new List<string>();
            var matches = Regex.Matches(content, @"\b\d{2,}\b");
            
            foreach (Match match in matches)
            {
                var number = match.Value;
                if (int.TryParse(number, out int value) && value > 10 && value != 100 && value != 1000)
                {
                    magicNumbers.Add($"Magic number found: {number}");
                }
            }
            
            return magicNumbers.Distinct().ToList();
        }
    }

    public class CleanupAnalysisResult
    {
        public string FilePath { get; set; } = string.Empty;
        public bool HasIssues { get; set; }
        public List<string> UnusedUsings { get; set; } = new();
        public List<string> SimplificationOpportunities { get; set; } = new();
        public List<string> DuplicateCodeBlocks { get; set; } = new();
        public List<string> LongMethods { get; set; } = new();
        public List<string> MagicNumbers { get; set; } = new();
    }
}
