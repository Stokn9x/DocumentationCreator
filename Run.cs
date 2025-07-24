using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentationCreator.Builders;
using DocumentationCreator.Models;
using DocumentationCreator.Services;
using DocumentationCreator.Utils;

namespace DocumentationCreator
{
    public class Run
    {

        private readonly DocumentationService _docService;
        private readonly ProjectOverviewBuilder _projectOverviewBuilder;
        private readonly ArchitectureDiagramBuilder _architectureDiagramBuilder;
        private readonly IndexBuilder _indexBuilder;
        private readonly ErDiagramBuilder _erDiagramBuilder;
        private readonly FileService _fileService;

        public Run(DocumentationService docService, ProjectOverviewBuilder projectOverviewBuilder, ArchitectureDiagramBuilder architectureDiagramBuilder, IndexBuilder indexBuilder, ErDiagramBuilder erDiagramBuilder, FileService fileService)
        {
            _docService = docService;
            _projectOverviewBuilder = projectOverviewBuilder;
            _architectureDiagramBuilder = architectureDiagramBuilder;
            _indexBuilder = indexBuilder;
            _erDiagramBuilder = erDiagramBuilder;
            _fileService = fileService;
        }

        public async Task Start(string rootPath, string outputPath, string dbContextName, List<string> exclutions)
        {
            List<CodeFile> codeFiles = _fileService.LoadCodeFiles(rootPath, exclutions);

            await _docService.GenerateDocumentationAsync(rootPath, outputPath, codeFiles);

            await _indexBuilder.GenerateIndexFile(outputPath);

            await _architectureDiagramBuilder.Generate(outputPath, codeFiles);

            await _projectOverviewBuilder.GenerateAsync(outputPath);

            if (string.IsNullOrWhiteSpace(dbContextName))
            {
                Console.WriteLine("❌ Ingen DbContext angivet, hopper over ER-diagram generering.");
                return;
            }
            else
            {
                await _erDiagramBuilder.Generate(outputPath, codeFiles, dbContextName);
            }
        }
    }
}
