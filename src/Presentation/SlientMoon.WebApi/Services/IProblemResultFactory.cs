using Microsoft.AspNetCore.Http;

namespace SlientMoon.WebApi.Services;

public interface IProblemResultFactory
{
    public IResult CreateProblem(Result result);
}
