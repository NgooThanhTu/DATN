using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.AI;
using TaskManagement.Application.DTOs.WorkTask;

namespace TaskManagement.Application.Interfaces
{
    public interface IAiService
    {
        Task<string> ChatAsync(Guid userId, AiChatRequestDto request);
        Task<string> GenerateDescriptionAsync(Guid userId, AiGenerateDescriptionRequestDto request);
        Task<List<AiSubTaskDto>> BreakdownTaskAsync(Guid userId, AiBreakdownRequestDto request);
        Task<List<WorkTaskResponseDto>> BreakdownAndCreateSubtasksAsync(Guid userId, AiBreakdownRequestDto request);
        Task<List<WorkTaskResponseDto>> CreateSubtasksFromPreviewAsync(Guid userId, AiCreateSubtasksFromPreviewRequestDto request);
        Task<AiEstimateSuggestionDto> SuggestEstimateAsync(Guid userId, AiEstimateSuggestionRequestDto request);
        Task<AiAssigneeSuggestionDto> SuggestAssigneesAsync(Guid userId, AiAssigneeSuggestionRequestDto request);
        Task<AiRepositoryAnalysisDto> AnalyzeRepositoryAsync(Guid userId, AiRepositoryAnalysisRequestDto request);
        Task<List<WorkTaskResponseDto>> CreateBacklogItemsFromAnalysisAsync(Guid userId, AiCreateBacklogFromAnalysisRequestDto request);
        Task<AiUsageDto> GetUsageAsync(Guid userId);
    }
}
