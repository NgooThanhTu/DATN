using System;
using System.Collections.Generic;

namespace TaskManagement.Application.DTOs.AI
{
    public class AiSubTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double EstHours { get; set; }
        public int Priority { get; set; } = 3;
    }

    public class AiBreakdownRequestDto
    {
        public Guid ProjectId { get; set; }
        public Guid? ParentTaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool CreateSubtasks { get; set; }
    }

    public class AiChatMessageDto
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class AiChatRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public List<AiChatMessageDto>? History { get; set; }
    }

    public class AiGenerateDescriptionRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
        public string? Context { get; set; }
    }

    public class AiUsageDto
    {
        public long UsedTokensThisMonth { get; set; }
        public long MonthlyTokenQuota { get; set; }
        public long RemainingTokens => Math.Max(0, MonthlyTokenQuota - UsedTokensThisMonth);
    }
}
