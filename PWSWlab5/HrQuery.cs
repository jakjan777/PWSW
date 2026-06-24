namespace PWSW5;

public record HrQuery(string Question) : IRequest<HrResponse>;

public record HrResponse(string Answer, bool FoundInKnowledgeBase);
